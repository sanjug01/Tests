using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class DeleteUserViewModelTests
    {
        private TestData _testData;
        private RdDataModel _dataModel;
        private Mock.NavigationService _navService;
        private Mock.PresentableView _view;
        private Credentials _cred;
        private DeleteUserViewModel _vm;
        
        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();            
            _cred = _testData.NewValidCredential();
            _navService = new Mock.NavigationService();
            _view = new Mock.PresentableView();
            
            _dataModel = new RdDataModel();
            //add some credentials to the datamodel
            foreach (Credentials cred in _testData.NewSmallListOfCredentials())
            {
                _dataModel.LocalWorkspace.Credentials.Add(cred);
            }            
            _dataModel.LocalWorkspace.Credentials.Add(_cred); //add this credential to the datamodel
            //add some desktops to the datamodel
            foreach (Desktop desktop in _testData.NewSmallListOfDesktops(_dataModel.LocalWorkspace.Credentials.ToList()))
            {
                _dataModel.LocalWorkspace.Connections.Add(desktop);
            }
            _dataModel.LocalWorkspace.Connections.Add(_testData.NewValidDesktop(_cred.Id)); //Add at least one desktop referencing this credential to the datamodel
            
            _vm = new DeleteUserViewModel();
            _vm.DataModel = _dataModel;
            _vm.PresentableView = _view;
            ((IViewModel)_vm).Presenting(_navService, _cred, null);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _navService.Dispose();
            _dataModel = null;
        }

        [TestMethod]
        public void CredentialPropertyMatchesInputCred()
        {
            Assert.AreEqual(_cred, _vm.Credential);
        }

        [TestMethod]
        public void CancelCommandDismissesView()
        {
            _navService.Expect("DismissModalView", new object[] { _view }, 0);
            _vm.CancelCommand.Execute(null);
        }

        [TestMethod]
        public void DeleteCommandDismissesView()
        {
            _navService.Expect("DismissModalView", new object[] { _view }, 0);
            _vm.CancelCommand.Execute(null);
        }

        [TestMethod]
        public void DeleteCommandDeletesCredFromDataModel()
        {
            CollectionAssert.Contains(_dataModel.LocalWorkspace.Credentials, _cred);
            _navService.Expect("DismissModalView", new object[] { _view }, 0);
            _vm.DeleteCommand.Execute(null);
            CollectionAssert.DoesNotContain(_dataModel.LocalWorkspace.Credentials, _cred);
        }

        [TestMethod]
        public void DeleteCommandRemovesReferencesToThisCredentialFromDesktops()
        {
            Func<bool> desktopsReferenceCred = () => _dataModel.LocalWorkspace.Connections.OfType<Desktop>().Any(d => d.CredentialId.Equals(_cred.Id));
            Assert.IsTrue(desktopsReferenceCred());
            _navService.Expect("DismissModalView", new object[] { _view }, 0);
            _vm.DeleteCommand.Execute(null);
            Assert.IsFalse(desktopsReferenceCred());
        }

        [TestMethod]
        public void DeleteCommandDeletesOnlyOneCredential()
        {
            int credCount = _dataModel.LocalWorkspace.Credentials.Count;
            _navService.Expect("DismissModalView", new object[] { _view }, 0);
            _vm.DeleteCommand.Execute(null);
            Assert.AreEqual(credCount - 1, _dataModel.LocalWorkspace.Credentials.Count);
        }

        [TestMethod]
        public void DeleteCommandDoesNotRemoveReferencesToOtherCredentials()
        {
            //record which credential each desktop references
            Dictionary<Desktop, Guid> previousCreds = new Dictionary<Desktop, Guid>();
            foreach (Desktop desktop in _dataModel.LocalWorkspace.Connections)
            {
                previousCreds.Add(desktop, desktop.CredentialId);
            }
            //delete credential
            _navService.Expect("DismissModalView", new object[] { _view }, 0);
            _vm.DeleteCommand.Execute(null);
            //check that only desktops referencing the deleted credential are changed
            foreach (Desktop desktop in _dataModel.LocalWorkspace.Connections)
            {
                Guid previousCred = previousCreds[desktop];
                //desktops referencing deleted cred should now not reference any cred
                if (previousCred.Equals(_cred.Id))
                {
                    Assert.AreEqual(Guid.Empty, desktop.CredentialId);
                }
                else //desktops not referencing deleted cred should reference the same cred as before
                {
                    Assert.AreEqual(previousCred, desktop.CredentialId);
                }

            }
        }
    }
}
