using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.ViewModels;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using System.Collections.Generic;
using RdClient.Shared.Navigation.Extensions;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class ConnectionCenterViewModelTests
    {
        private TestData _testData;
        private RdDataModel _dataModel;
        private Mock.NavigationService _navService;
        private ConnectionCenterViewModel _vm;

        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();
            _navService = new Mock.NavigationService();
            _dataModel = new RdDataModel();            
            List<Credentials> creds = _testData.NewSmallListOfCredentials();
            foreach(Credentials cred in creds)
            {
                _dataModel.LocalWorkspace.Credentials.Add(cred);
            }
            foreach(Desktop desktop in _testData.NewSmallListOfDesktops(creds))
            {
                _dataModel.LocalWorkspace.Connections.Add(desktop);
            }
            _vm = new ConnectionCenterViewModel();
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
        public void TestAddDesktopCommandExecute()
        {
            _navService.Expect("PushModalView", new List<object> { "AddOrEditDesktopView", null, null }, 0);
            _vm.AddDesktopCommand.Execute(null);
        }

        [TestMethod]
        public void TestEditDesktopCommandDoesNotBringUpEditViewIfNoDesktopSelected()
        {
            _vm.EditDesktopCommand.Execute(null);
        }

        [TestMethod]
        public void TestEditDesktopBringsUpEditViewIfDesktopSelected()
        {
            _vm.DesktopsSelectable = true;
            _vm.DesktopViewModels[_testData.RandomSource.Next(_vm.DesktopViewModels.Count)].IsSelected = true;
            _navService.Expect("PushModalView", new List<object> { "AddOrEditDesktopView", null, null }, 0);
            _vm.EditDesktopCommand.Execute(null);
        }

        [TestMethod]
        public void TestDeleteDesktopCommandBringsUpDeleteDesktopsViewIfDesktopSelected()
        {
            _vm.DesktopsSelectable = true;
            _vm.DesktopViewModels[_testData.RandomSource.Next(_vm.DesktopViewModels.Count)].IsSelected = true;
            _navService.Expect("PushModalView", new List<object> { "DeleteDesktopsView", null, null }, 0);
            _vm.DeleteDesktopCommand.Execute(null);
        }

        [TestMethod]
        public void TestDeleteDesktopCommandDoesNotBringUpDeleteDesktopsViewIfNoDesktopsSelected()
        {
            _vm.DeleteDesktopCommand.Execute(null);
        }

        [TestMethod]
        public void TestHasDesktopsReturnsTrueIfThereAreDesktops()
        {
            Assert.IsTrue(_vm.DesktopViewModels.Count > 0);
            Assert.IsTrue(_vm.HasDesktops);
        }

        [TestMethod]
        public void TestHasDesktopsReturnsFalseIfThereAreNoDesktops()
        {
            foreach (Desktop desktop in _dataModel.LocalWorkspace.Connections.ToList())
            {
                _dataModel.LocalWorkspace.Connections.Remove(desktop);
            }
            Assert.IsFalse(_vm.HasDesktops);
        }

        [TestMethod]
        public void TestSelectedCountReturnsZeroWhenNoneSelected()
        {
            Assert.AreEqual(0, _vm.SelectedCount);
        }

        [TestMethod]
        public void TestSelectedCountIncrementsAndDecrements()
        {
            _vm.DesktopsSelectable = true;
            foreach(DesktopViewModel dvm in _vm.DesktopViewModels)
            {
                dvm.IsSelected = true;
            }
            Assert.AreEqual(_vm.DesktopViewModels.Count, _vm.SelectedCount);
            foreach (DesktopViewModel dvm in _vm.DesktopViewModels)
            {
                dvm.IsSelected = false;
            }
            Assert.AreEqual(0, _vm.SelectedCount);
        }

        [TestMethod]
        public void TestDesktopViewModelsMatchDataModelDesktops()
        {
            AssertDesktopViewModelsMatchDesktops();
        }

        [TestMethod]
        public void TestDesktopViewModelAddedWhenDesktopAdded()
        {
            _dataModel.LocalWorkspace.Connections.Add(_testData.NewValidDesktop(Guid.Empty));
            AssertDesktopViewModelsMatchDesktops();
        }

        [TestMethod]
        public void TestDesktopViewModelRemovedWhenDesktopRemoved()
        {
            _dataModel.LocalWorkspace.Connections.RemoveAt(_testData.RandomSource.Next(_dataModel.LocalWorkspace.Connections.Count));
            AssertDesktopViewModelsMatchDesktops();
        }

        [TestMethod]
        public void TestHasCorrectApplicationBarItems()
        {
            IEnumerable<BarItemModel> barItems = (_vm as IApplicationBarItemsSource).GetItems(null);
            IEnumerable<SegoeGlyphBarButtonModel> barButtons = barItems.OfType<SegoeGlyphBarButtonModel>();
            Assert.AreEqual(2, barButtons.Count());
            Assert.IsTrue(barButtons.Any((b) => b.Command.Equals(_vm.EditDesktopCommand)), "ApplicationBarItems should contain a button linked to EditDesktopCommand");
            Assert.IsTrue(barButtons.Any((b) => b.Command.Equals(_vm.DeleteDesktopCommand)), "ApplicationBarItems should contain a button linked to DeleteDesktopCommand");
        }

        private void AssertDesktopViewModelsMatchDesktops()
        {
            //checking there are the same number of DesktopViewModels as Desktops and that each Desktop is represented by a DesktopViewModel is sufficient
            Assert.AreEqual(_dataModel.LocalWorkspace.Connections.Count, _vm.DesktopViewModels.Count);
            foreach (Desktop desktop in _dataModel.LocalWorkspace.Connections)
            {
                Assert.IsTrue(_vm.DesktopViewModels.Any((dvm) => dvm.Desktop.Equals(desktop)));
            }
        }
    }
}
