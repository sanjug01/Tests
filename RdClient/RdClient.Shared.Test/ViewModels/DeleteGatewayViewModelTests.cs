using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.Data;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.Navigation.Extensions;
using RdClient.Shared.Test.Data;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.Test.UAP;
using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class DeleteGatewayViewModelTests
    {
        private TestData _testData;
        private ApplicationDataModel _dataModel;
        private Mock.NavigationService _navService;
        private Mock.PresentableView _view;
        //private GatewayModel _gateway;
        private IModelContainer<GatewayModel> _gatewayContainer;
        private DeleteGatewayViewModel _vm;

        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();
            _gatewayContainer = _testData.NewValidGateway();
            _navService = new Mock.NavigationService();
            _view = new Mock.PresentableView();

            _dataModel = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer()
            };

            //add some gateways to the datamodel
            foreach (IModelContainer<GatewayModel> gateway in _testData.NewSmallListOfGateways())
            {
                _dataModel.Gateways.AddNewModel(gateway.Model);
            }

            //add this gateway to the datamodel
            _gatewayContainer = TemporaryModelContainer<GatewayModel>.WrapModel(_dataModel.Gateways.AddNewModel(_gatewayContainer.Model), _gatewayContainer.Model);

            //add some desktops to the datamodel
            foreach (DesktopModel desktop in _testData.NewSmallListOfDesktopsWithGateway(_dataModel.Gateways.Models).Select(d => d.Model).ToList())
            {
                _dataModel.LocalWorkspace.Connections.AddNewModel(desktop);
            }

            //Add at least one desktop referencing this gateway to the datamodel
            _dataModel.LocalWorkspace.Connections.AddNewModel(_testData.NewValidDesktopWithGateway(_gatewayContainer.Id).Model); 

            _vm = new DeleteGatewayViewModel();
            ((IDataModelSite)_vm).SetDataModel(_dataModel);
            ((IViewModel)_vm).Presenting(_navService, _gatewayContainer, null);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _navService.Dispose();
            _dataModel = null;
        }

        [TestMethod]
        public void GatewaysDelete_GatewayPropertyMatchesInputGatewa()
        {
            Assert.AreEqual(_gatewayContainer.Model, _vm.Gateway);
        }

        [TestMethod]
        public void GatewaysDelete_CancelCommandDismissesView()
        {
            _navService.Expect("DismissModalView", new object[] { _view }, 0);
            _vm.Cancel.Execute(null);
        }

        [TestMethod]
        public void GatewaysDelete_DeleteCommandDismissesView()
        {
            _navService.Expect("DismissModalView", new object[] { _view }, 0);
            _vm.Cancel.Execute(null);
        }

        [TestMethod]
        // UAP not supported   [ExpectedException(typeof(KeyNotFoundException))]
        public void GatewaysDelete_DeleteCommandDeletesGatewayFromDataModel()
        {
            Assert.AreSame(_gatewayContainer.Model, _dataModel.Gateways.GetModel(_gatewayContainer.Id));
            _navService.Expect("DismissModalView", new object[] { _view }, 0);
            _vm.DefaultAction.Execute(null);

            Assert.IsTrue( ExceptionExpecter.ExpectException<KeyNotFoundException>(() =>
              {
                  GatewayModel model = _dataModel.Gateways.GetModel(_gatewayContainer.Id);
              } ));

        }

        [TestMethod]
        public void GatewaysDelete_DeleteCommandDeletesOnlyOneGateway()
        {
            int gatewayCount = _dataModel.Gateways.Models.Count;
            _navService.Expect("DismissModalView", new object[] { _view }, 0);
            _vm.DefaultAction.Execute(null);
            Assert.AreEqual(gatewayCount - 1, _dataModel.Gateways.Models.Count);
        }

        [TestMethod]
        public void GatewaysDelete_DeleteCommandRemovesReferencesToThisGatewayFromDesktops()
        {
            Func<bool> desktopsReferenceGateway = () =>
                _dataModel.LocalWorkspace.Connections.Models.OfType<IModelContainer<RemoteConnectionModel>>().Any(d => ((DesktopModel)d.Model).GatewayId.Equals(_gatewayContainer.Id));
            Assert.IsTrue(desktopsReferenceGateway());
            _navService.Expect("DismissModalView", new object[] { _view }, 0);
            _vm.DefaultAction.Execute(null);
            Assert.IsFalse(desktopsReferenceGateway());
        }

        [TestMethod]
        public void GatewaysDelete_DeleteCommandDoesNotRemoveReferencesToOtherGateways()
        {
            //record which credential each desktop references
            IDictionary<DesktopModel, Guid> previousGateways = new Dictionary<DesktopModel, Guid>();

            foreach (IModelContainer<RemoteConnectionModel> desktop in _dataModel.LocalWorkspace.Connections.Models)
            {

                previousGateways.Add((DesktopModel)desktop.Model, ((DesktopModel)desktop.Model).GatewayId);
            }

            //delete 
            _navService.Expect("DismissModalView", new object[] { _view }, 0);
            _vm.DefaultAction.Execute(null);

            //check that only desktops referencing the deleted gateway are changed
            foreach (IModelContainer<RemoteConnectionModel> desktop in _dataModel.LocalWorkspace.Connections.Models)
            {
                Guid previousGateway = previousGateways[(DesktopModel)desktop.Model];
                
                if (previousGateway.Equals(_gatewayContainer.Id))
                {
                    //desktops referencing deleted gateway should now not reference any gateway
                    Assert.AreEqual(Guid.Empty, ((DesktopModel)desktop.Model).GatewayId);
                }
                else 
                {
                    //desktops not referencing deleted gateway should reference the same gateway as before
                    Assert.AreEqual(previousGateway, ((DesktopModel)desktop.Model).GatewayId);
                }
            }
        }
    }
}
