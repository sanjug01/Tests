using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class SettingsViewModelTests
    {
        private TestData _testData;
        private RdDataModel _dataModel;
        private Mock.NavigationService _navService;
        private SettingsViewModel _vm;

        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();
            _navService = new Mock.NavigationService();
            _dataModel = new RdDataModel(); 
            _vm = new SettingsViewModel();
            _vm.DataModel = _dataModel;
            ((IViewModel)_vm).Presenting(_navService, null, null); 
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _navService.Dispose();
            _dataModel = null;
        }

        [TestMethod]
        public void TestGoBackCommandNavigatesToConnectionCenter()
        {
            _navService.Expect("NavigateToView", new List<object>() { "ConnectionCenterView", null }, 0);
            _vm.GoBackCommand.Execute(null);
        }

        [TestMethod]
        public void TestSettingsLoadedFromDataModel()
        {
            Assert.AreEqual(_vm.GeneralSettings, _dataModel.Settings);
        }

        [TestMethod]
        public void TestAddUserCommandShowsAddUserView()
        {
            _navService.Expect("PushModalView", new List<object>() { "AddUserView", null }, 0);
            _vm.AddUserCommand.Execute(null);
        }

        [TestMethod]
        public void HasCredentialsFalseWhenDataModelHasNoCredentials()
        {
            Assert.AreEqual(0, _dataModel.LocalWorkspace.Credentials.Count);
            Assert.IsFalse(_vm.HasCredentials);
        }

        [TestMethod]
        public void AddCredentialToDataModelAddsMatchingCredentialViewModel()
        {
            Credentials cred = _testData.NewValidCredential();
            _dataModel.LocalWorkspace.Credentials.Add(cred);
            Assert.AreEqual(cred, _vm.CredentialsViewModels[0].Credential);
        }

        [TestMethod]
        public void HasCredentialsTrueAfterAddingCredentialToDataModel()
        {
            Assert.IsFalse(_vm.HasCredentials);
            _dataModel.LocalWorkspace.Credentials.Add(_testData.NewValidCredential());
            Assert.IsTrue(_vm.HasCredentials);
        }

        [TestMethod]
        public void HasCredentialsFalseAfterRemovingLastCredentialFromDataModel()
        {
            _dataModel.LocalWorkspace.Credentials.Add(_testData.NewValidCredential());
            Assert.IsTrue(_vm.HasCredentials);
            foreach (Credentials cred in _dataModel.LocalWorkspace.Credentials.ToList())
            {
                _dataModel.LocalWorkspace.Credentials.Remove(cred);
            }
            Assert.AreEqual(0, _vm.CredentialsViewModels.Count);
            Assert.IsFalse(_vm.HasCredentials);            
        }
    }
}
