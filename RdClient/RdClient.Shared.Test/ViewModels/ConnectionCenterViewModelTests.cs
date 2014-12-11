using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.ViewModels;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class ConnectionCenterViewModelTests
    {
        private TestData _testData;
        private Mock.DataModel _dataModel;
        private Mock.NavigationService _navService;
        private ConnectionCenterViewModel _vm;

        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();
            _dataModel = new Mock.DataModel();
            _dataModel.Desktops = new ModelCollection<Desktop>();
            _vm = new ConnectionCenterViewModel();
            ((IViewModel)_vm).Presenting(_navService, null, null);
            _vm.DataModel = _dataModel;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _navService.Dispose();
            _dataModel.Dispose();
        }

        [TestMethod]
        public void TestAddDesktopExecute()
        {
            
        }
    }
}
