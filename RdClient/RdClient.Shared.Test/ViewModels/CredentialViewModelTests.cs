using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.Models;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.ViewModels;
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
            _cred = _testData.NewValidCredential();
            _dataModel.LocalWorkspace.Credentials.Add(_cred);
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
            List<Desktop> desktops = _testData.NewSmallListOfDesktops(_dataModel.LocalWorkspace.Credentials.ToList());
            foreach(Desktop desktop in desktops)
            {
                _dataModel.LocalWorkspace.Connections.Add(desktop);
            }
            _vm.DeleteCommand.Execute(null);
            Assert.IsFalse(desktops.Any(d => d.CredentialId.Equals(_cred.Id)));
        }
    }
}
