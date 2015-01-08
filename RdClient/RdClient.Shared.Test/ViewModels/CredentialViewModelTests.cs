using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.Models;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class CredentialViewModelTests
    {
        private TestData _testData;
        private RdDataModel _dataModel;
        private Mock.NavigationService _navService;
        private Credentials _cred;
        private CredentialViewModel _vm;

        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();
            _dataModel = new RdDataModel();
            _navService = new Mock.NavigationService();            
            //add some credentials to the datamodel
            foreach (Credentials cred in _testData.NewSmallListOfCredentials())
            {
                _dataModel.LocalWorkspace.Credentials.Add(cred);
            }
            _cred = _testData.NewValidCredential();
            _dataModel.LocalWorkspace.Credentials.Add(_cred); //add this credential to the datamodel
            //add some desktops to the datamodel
            foreach (Desktop desktop in _testData.NewSmallListOfDesktops(_dataModel.LocalWorkspace.Credentials.ToList()))
            {
                _dataModel.LocalWorkspace.Connections.Add(desktop);
            }
            _dataModel.LocalWorkspace.Connections.Add(_testData.NewValidDesktop(_cred.Id)); //Add at least one desktop referencing this credential to the datamodel
            _vm = new CredentialViewModel(_cred);
            _vm.Presented(_navService, _dataModel);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _navService.Dispose();
            _dataModel = null;
        }

        [TestMethod]
        public void TestCredMatches()
        {
            Assert.AreEqual(_cred, _vm.Credential);
        }

        [TestMethod]
        public void TestEditCommandShowsAddUserView()
        {
            _navService.Expect("PushModalView", new List<object>() { "AddUserView", null, null }, 0);
            _vm.EditCommand.Execute(null);
        }

        [TestMethod]
        public void TestDeleteCommandDeletesCredFromDataModel()
        {
            CollectionAssert.Contains(_dataModel.LocalWorkspace.Credentials, _cred);
            _vm.DeleteCommand.Execute(null);
            CollectionAssert.DoesNotContain(_dataModel.LocalWorkspace.Credentials, _cred);
        }

        [TestMethod]
        public void TestDeleteCommandRemovesReferencesToThisCredentialFromDesktops()
        {
            Func<bool> desktopsReferenceCred = () => _dataModel.LocalWorkspace.Connections.OfType<Desktop>().Any(d => d.CredentialId.Equals(_cred.Id));
            Assert.IsTrue(desktopsReferenceCred());
            _vm.DeleteCommand.Execute(null);
            Assert.IsFalse(desktopsReferenceCred());
        }

        [TestMethod]
        public void TestDeleteCommandDeletesOnlyOneCredential()
        {
            int credCount = _dataModel.LocalWorkspace.Credentials.Count;
            _vm.DeleteCommand.Execute(null);
            Assert.AreEqual(credCount - 1, _dataModel.LocalWorkspace.Credentials.Count);
        }

        [TestMethod]
        public void TestDeleteCommandDoesNotRemoveReferencesToOtherCredentials()
        {
            //record which credential each desktop references
            Dictionary<Desktop, Guid> previousCreds = new Dictionary<Desktop,Guid>();
            foreach(Desktop desktop in _dataModel.LocalWorkspace.Connections)
            {
                previousCreds.Add(desktop, desktop.CredentialId);
            }
            //delete credential
            _vm.DeleteCommand.Execute(null);
            //check that only desktops referencing the deleted credential are changed
            foreach(Desktop desktop in _dataModel.LocalWorkspace.Connections)
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
