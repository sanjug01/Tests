using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.Data;
using RdClient.Shared.Models;
using RdClient.Shared.Test.Data;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.ViewModels;
using System.Collections.Generic;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class CredentialViewModelTests
    {
        private TestData _testData;
        private ApplicationDataModel _dataModel;
        private Mock.NavigationService _navService;
        private IModelContainer<CredentialsModel> _container;
        private CredentialsModel _cred;
        private CredentialViewModel _vm;

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
            _container = _testData.NewValidCredential();
            _vm = new CredentialViewModel(_container);
            _cred = _container.Model;
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
            Assert.AreEqual(_cred, _vm.Credentials);
        }

        [TestMethod]
        public void TestEditCommandShowsAddUserView()
        {
            _navService.Expect("PushModalView", new List<object>() { "AddUserView", null, null }, 0);
            _vm.EditCommand.Execute(null);
        }

        [TestMethod]
        public void TestDeleteCommandShowsDeleteUserView()
        {
            _navService.Expect("PushModalView", new List<object>() { "DeleteUserView", _container, null }, 0);
            _vm.DeleteCommand.Execute(null);
        }
    }
}
