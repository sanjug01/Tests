using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.Data;
using RdClient.Shared.Helpers;
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
    public class DeleteUserViewModelTests
    {
        private TestData _testData;
        private ApplicationDataModel _dataModel;
        private Mock.NavigationService _navService;
        private Mock.PresentableView _view;
        private Mock.ModalPresentationContext _context;
        private IModelContainer<CredentialsModel> _cred;
        private DeleteUserViewModel _vm;
        
        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();            
            _cred = _testData.NewValidCredential();
            _navService = new Mock.NavigationService();
            _view = new Mock.PresentableView();
            _context = new Mock.ModalPresentationContext();

            _dataModel = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer(),
                //
                // Set the data scrambler to use the local user's key
                //
                DataScrambler = new Rc4DataScrambler()
            };

            //add some credentials to the datamodel
            foreach (IModelContainer<CredentialsModel> cred in _testData.NewSmallListOfCredentials())
            {
                _dataModel.Credentials.AddNewModel(cred.Model);
            }            

            //add this credential to the datamodel
            _cred = TemporaryModelContainer<CredentialsModel>.WrapModel(_dataModel.Credentials.AddNewModel(_cred.Model), _cred.Model);

            //add some desktops to the datamodel
            foreach (DesktopModel desktop in _testData.NewSmallListOfDesktops(_dataModel.Credentials.Models.ToList()).Select(d => d.Model))
            {
                _dataModel.LocalWorkspace.Connections.AddNewModel(desktop);
            }
            _dataModel.LocalWorkspace.Connections.AddNewModel(_testData.NewValidDesktop(_cred.Id).Model); //Add at least one desktop referencing this credential to the datamodel
            
            _vm = new DeleteUserViewModel();
            ((IDataModelSite)_vm).SetDataModel(_dataModel);
            ((IViewModel)_vm).Presenting(_navService, _cred, null);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _navService.Dispose();
            _context.Dispose();
            _dataModel = null;
        }

        [TestMethod]
        public void CredentialPropertyMatchesInputCred()
        {
            Assert.AreEqual(_cred.Model, _vm.Credentials);
        }

        [TestMethod]
        public void CancelCommandDismissesView()
        {
            ((IViewModel)_vm).Presenting(_navService, _cred, _context);
            _context.Expect("Dismiss", p => { return null; });
            _vm.Cancel.Execute(null);
        }

        [TestMethod]
        public void DeleteCommandDismissesView()
        {
            ((IViewModel)_vm).Presenting(_navService, _cred, _context);
            _context.Expect("Dismiss", p => { return null; });
            _vm.DefaultAction.Execute(null);
        }

        [TestMethod]
        public void DeleteCommandDeletesCredFromDataModel()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<KeyNotFoundException>(() =>
            {
                Assert.AreSame(_cred.Model, _dataModel.Credentials.GetModel(_cred.Id));
                _vm.DefaultAction.Execute(null);
                CredentialsModel model = _dataModel.Credentials.GetModel(_cred.Id);
            }));
        }

        [TestMethod]
        public void DeleteCommandRemovesReferencesToThisCredentialFromDesktops()
        {
            Func<bool> desktopsReferenceCred = () =>
                _dataModel.LocalWorkspace.Connections.Models.OfType<IModelContainer<RemoteConnectionModel>>().Any(d => ((DesktopModel)d.Model).CredentialsId.Equals(_cred.Id));
            Assert.IsTrue(desktopsReferenceCred());
            _vm.DefaultAction.Execute(null);
            Assert.IsFalse(desktopsReferenceCred());
        }

        [TestMethod]
        public void DeleteCommandDeletesOnlyOneCredential()
        {
            int credCount = _dataModel.Credentials.Models.Count;
            _vm.DefaultAction.Execute(null);
            Assert.AreEqual(credCount - 1, _dataModel.Credentials.Models.Count);
        }

        [TestMethod]
        public void DeleteCommandDoesNotRemoveReferencesToOtherCredentials()
        {
            //record which credential each desktop references
            IDictionary<DesktopModel, Guid> previousCreds = new Dictionary<DesktopModel, Guid>();

            foreach (IModelContainer<RemoteConnectionModel> desktop in _dataModel.LocalWorkspace.Connections.Models)
            {

                previousCreds.Add((DesktopModel)desktop.Model, ((DesktopModel)desktop.Model).CredentialsId);
            }
            //delete credential
            _vm.DefaultAction.Execute(null);
            //check that only desktops referencing the deleted credential are changed
            foreach (IModelContainer<RemoteConnectionModel> desktop in _dataModel.LocalWorkspace.Connections.Models)
            {
                Guid previousCred = previousCreds[(DesktopModel)desktop.Model];

                //desktops referencing deleted cred should now not reference any cred
                if (previousCred.Equals(_cred.Id))
                {
                    Assert.AreEqual(Guid.Empty, ((DesktopModel)desktop.Model).CredentialsId);
                }
                else //desktops not referencing deleted cred should reference the same cred as before
                {
                    Assert.AreEqual(previousCred, ((DesktopModel)desktop.Model).CredentialsId);
                }

            }
        }
    }
}
