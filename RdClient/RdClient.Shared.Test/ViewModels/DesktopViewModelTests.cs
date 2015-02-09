using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.Data;
using RdClient.Shared.Models;
using RdClient.Shared.Test.Data;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Media.Imaging;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class DesktopViewModelTests
    {
        private TestData _testData;
        private ApplicationDataModel _dataModel;
        private Mock.NavigationService _navService;
        private DesktopModel _desktop;
        private CredentialsModel _cred;
        private IDesktopViewModel _vm;

        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();
            _dataModel = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer()
            };
            _navService = new Mock.NavigationService();
            _cred = _testData.NewValidCredential().Model;
            _desktop = _testData.NewValidDesktop(_dataModel.LocalWorkspace.Credentials.AddNewModel(_cred));

            _dataModel.LocalWorkspace.Connections.AddNewModel(_desktop);

            _vm = DesktopViewModel.Create(_dataModel.LocalWorkspace.Connections.Models[0], _dataModel, null);
            ((DesktopViewModel)_vm).NavigationService = _navService;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _navService.Dispose();
            _dataModel = null;
        }

        [TestMethod]
        public void TestDesktopMatches()
        {
            Assert.AreEqual(_desktop, _vm.Desktop);
        }

        [TestMethod]
        public void TestCredentialReturnsNullIfDesktopHasNoCredential()
        {
            _desktop.CredentialsId = Guid.Empty;
            Assert.IsNull(_vm.Credentials);
        }

        [TestMethod]
        public void TestCredentialReturnsCredentialForDesktop()
        {
            Assert.AreEqual(_cred, _vm.Credentials);
        }

        [TestMethod]
        public void TestIsSelectedInitiallyFalse()
        {
            Assert.IsFalse(_vm.IsSelected);
        }

        [TestMethod]
        public void TestIsSelectedSetCorrectly()
        {
            _vm.SelectionEnabled = true;
            _vm.IsSelected = true;
            Assert.IsTrue(_vm.IsSelected);
            _vm.IsSelected = false;
            Assert.IsFalse(_vm.IsSelected);
        }

        [TestMethod]
        public void TestThumbnailCreatedCorrectly()
        {
            Assert.IsNotNull(_vm.Thumbnail);
            Assert.AreSame(_vm.Thumbnail, _desktop.Thumbnail);
        }

        [TestMethod]
        public void TestEditCommandExecute()
        {

            _navService.Expect("PushModalView", new List<object> { "AddOrEditDesktopView", null, null }, 0);
            _vm.EditCommand.Execute(null);
        }

        [TestMethod]
        public void TestConnectCommandExecuteNavigatesToSessionViewIfCredentialsExist()
        {
            _navService.Expect("NavigateToView", new List<object> { "SessionView", null }, 0);
            _vm.ConnectCommand.Execute(null);
        }

        [TestMethod]
        public void TestConnectCommandExecuteShowsAddUserViewIfNoCredentials()
        {
            _vm.Desktop.CredentialsId = Guid.Empty;
            _navService.Expect("PushModalView", new List<object> { "AddUserView", null, null }, 0);
            _vm.ConnectCommand.Execute(null);
        }

        [TestMethod]
        public void TestDeleteCommandExecute()
        {
            _navService.Expect("PushModalView", new List<object> { "DeleteDesktopsView", null, null }, 0);
            _vm.DeleteCommand.Execute(null);
        }

        [TestMethod]
        public void TestSelectionEnabledInitiallyFalse()
        {
            Assert.IsFalse(_vm.SelectionEnabled);
        }

        [TestMethod]
        public void TestSelectingFailsIfSelectionIsNotEnabled()
        {
            _vm.SelectionEnabled = false;
            Assert.IsFalse(_vm.IsSelected);
            _vm.IsSelected = true;
            Assert.IsFalse(_vm.IsSelected);
        }

        [TestMethod]
        public void TestSelectingSucceedsIfSelectionIsEnabled()
        {
            _vm.SelectionEnabled = true;
            Assert.IsFalse(_vm.IsSelected);
            _vm.IsSelected = true;
            Assert.IsTrue(_vm.IsSelected);
        }

        [TestMethod]
        public void TestDisablingSelectionSetsSelectedToFalse()
        {
            _vm.SelectionEnabled = true;            
            _vm.IsSelected = true;
            Assert.IsTrue(_vm.IsSelected);
            _vm.SelectionEnabled = false;
            Assert.IsFalse(_vm.IsSelected);
        }
    }
}