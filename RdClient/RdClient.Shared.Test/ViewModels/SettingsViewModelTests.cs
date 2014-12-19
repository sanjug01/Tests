using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.ViewModels;
using System.Collections.Generic;

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
    }
}
