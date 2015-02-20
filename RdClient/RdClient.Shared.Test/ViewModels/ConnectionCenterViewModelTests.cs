﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.ViewModels;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using System.Collections.Generic;
using RdClient.Shared.Navigation.Extensions;
using RdClient.Shared.Data;
using System.Collections;
using RdClient.Shared.Test.Data;
using RdClient.Shared.Helpers;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class ConnectionCenterViewModelTests
    {
        private TestData _testData;
        private ApplicationDataModel _dataModel;
        private SessionFactory _sessionFactory;
        private Mock.NavigationService _navService;
        private ConnectionCenterViewModel _vm;

        private sealed class Dispatcher : IDeferredExecution
        {
            void IDeferredExecution.Defer(Action action)
            {
                throw new NotImplementedException();
            }
        }

        private sealed class SessionFactory : ISessionFactory
        {
            IRemoteSession ISessionFactory.CreateSession(RemoteSessionSetup sessionSetup)
            {
                throw new NotImplementedException();
            }
        }

        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();
            _navService = new Mock.NavigationService();
            _dataModel = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer()
            };
            _sessionFactory = new SessionFactory();
            IList<IModelContainer<CredentialsModel>> creds = _testData.NewSmallListOfCredentials();

            foreach (IModelContainer<CredentialsModel> cred in creds)
            {
                _dataModel.LocalWorkspace.Credentials.AddNewModel(cred.Model);
            }

            foreach(DesktopModel desktop in _testData.NewSmallListOfDesktops(creds))
            {
                _dataModel.LocalWorkspace.Connections.AddNewModel(desktop);
            }
            _vm = new ConnectionCenterViewModel();
            _vm.CastAndCall<IDeferredExecutionSite>(site => site.SetDeferredExecution(new Dispatcher()));
            ((IDataModelSite)_vm).SetDataModel(_dataModel);
            ((ISessionFactorySite)_vm).SetSessionFactory(_sessionFactory);
            ((IViewModel)_vm).Presenting(_navService, null, null);            
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _navService.Dispose();
            _dataModel = null;
            _sessionFactory = null;
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
            foreach (IModelContainer<RemoteConnectionModel> desktop in _dataModel.LocalWorkspace.Connections.Models.ToList())
            {
                _dataModel.LocalWorkspace.Connections.RemoveModel(desktop.Id);
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
            _dataModel.LocalWorkspace.Connections.AddNewModel(_testData.NewValidDesktop(Guid.Empty));
            AssertDesktopViewModelsMatchDesktops();
        }

        [TestMethod]
        public void TestDesktopViewModelRemovedWhenDesktopRemoved()
        {
            IList<IModelContainer<RemoteConnectionModel>> allModels = _dataModel.LocalWorkspace.Connections.Models.ToList<IModelContainer<RemoteConnectionModel>>();

            _dataModel.LocalWorkspace.Connections.RemoveModel(allModels[_testData.RandomSource.Next(allModels.Count)].Id);
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

        [TestMethod]
        public void TestDesktopsSelectableInitiallyFalse()
        {
            Assert.IsFalse(_vm.DesktopsSelectable);
        }

        [TestMethod]
        public void TestToggleDesktopSelectionCommandEnablesSelection()
        {
            _vm.ToggleDesktopSelectionCommand.Execute(null);
            Assert.IsTrue(_vm.DesktopsSelectable);
            _vm.ToggleDesktopSelectionCommand.Execute(null);
            Assert.IsFalse(_vm.DesktopsSelectable);
        }

        [TestMethod]
        public void TestDesktopsSelectableSetsDesktopViewModelsEnableSelection()
        {
            _vm.DesktopsSelectable = true;
            AssertDesktopViewModelSelectionEnabledMatchesDesktopsSelectable();
            _vm.DesktopsSelectable = false;
            AssertDesktopViewModelSelectionEnabledMatchesDesktopsSelectable();
        }

        [TestMethod]
        public void TestAddDesktopWhenSelectionEnabledEnablesSelectionOnNewDesktopViewModel()
        {
            _vm.DesktopsSelectable = true;
            DesktopModel newDesktop = _testData.NewValidDesktop(Guid.Empty);
            _dataModel.LocalWorkspace.Connections.AddNewModel(newDesktop);
            IDesktopViewModel dvm = _vm.DesktopViewModels.Single(d => object.ReferenceEquals(newDesktop, d.Desktop));
            Assert.IsTrue(dvm.SelectionEnabled);
        }

        [TestMethod]
        public void TestAddDesktopWhenSelectionDisabledDisablesSelectionOnNewDesktopViewModel()
        {
            _vm.DesktopsSelectable = false;
            DesktopModel newDesktop = _testData.NewValidDesktop(Guid.Empty);
            _dataModel.LocalWorkspace.Connections.AddNewModel(newDesktop);
            IDesktopViewModel dvm = _vm.DesktopViewModels.Single(d => object.ReferenceEquals(newDesktop, d.Desktop));
            Assert.IsFalse(dvm.SelectionEnabled);
        }

        [TestMethod]
        public void TestGoToSettingsCommandNavigatesToSettingsView()
        {
            _navService.Expect("NavigateToView", new List<object>() { "SettingsView", null }, 0);
            _vm.GoToSettingsCommand.Execute(null);
        }

        [TestMethod]
        public void ConnectionCenterViewModel_DifferentHostNames_SortedAlphabetically()
        {
            DesktopModel dm1 = new DesktopModel() { HostName = "alpha" };
            DesktopModel dm2 = new DesktopModel() { HostName = "bravo" };
            IList<Guid> ids = new List<Guid>();

            foreach (IModelContainer<RemoteConnectionModel> c in _dataModel.LocalWorkspace.Connections.Models)
                ids.Add(c.Id);
            foreach (Guid id in ids)
                _dataModel.LocalWorkspace.Connections.RemoveModel(id);

            _dataModel.LocalWorkspace.Connections.AddNewModel(dm1);
            _dataModel.LocalWorkspace.Connections.AddNewModel(dm2);

            Assert.AreEqual(2, _vm.DesktopViewModels.Count);
            Assert.AreSame(dm1, _vm.DesktopViewModels[0].Desktop);
            Assert.AreSame(dm2, _vm.DesktopViewModels[1].Desktop);
        }

        [TestMethod]
        public void ConnectionCenterViewModel_SameHostNamesDifferentFriendlyNames_SortedAlphabetically()
        {
            DesktopModel dm1 = new DesktopModel() { HostName = "alpha", FriendlyName = "zod" };
            DesktopModel dm2 = new DesktopModel() { HostName = "alpha", FriendlyName = "bod" };
            IList<Guid> ids = new List<Guid>();

            foreach (IModelContainer<RemoteConnectionModel> c in _dataModel.LocalWorkspace.Connections.Models)
                ids.Add(c.Id);
            foreach (Guid id in ids)
                _dataModel.LocalWorkspace.Connections.RemoveModel(id);

            _dataModel.LocalWorkspace.Connections.AddNewModel(dm1);
            _dataModel.LocalWorkspace.Connections.AddNewModel(dm2);

            Assert.AreEqual(2, _vm.DesktopViewModels.Count);
            Assert.AreSame(dm2, _vm.DesktopViewModels[0].Desktop);
            Assert.AreSame(dm1, _vm.DesktopViewModels[1].Desktop);
        }

        [TestMethod]
        public void ConnectionCenterViewModel_SameHostNamesNoFriendlyName1_SortedAlphabetically()
        {
            DesktopModel dm1 = new DesktopModel() { HostName = "alpha" };
            DesktopModel dm2 = new DesktopModel() { HostName = "alpha", FriendlyName = "bod" };
            IList<Guid> ids = new List<Guid>();

            foreach (IModelContainer<RemoteConnectionModel> c in _dataModel.LocalWorkspace.Connections.Models)
                ids.Add(c.Id);
            foreach (Guid id in ids)
                _dataModel.LocalWorkspace.Connections.RemoveModel(id);

            _dataModel.LocalWorkspace.Connections.AddNewModel(dm1);
            _dataModel.LocalWorkspace.Connections.AddNewModel(dm2);

            Assert.AreEqual(2, _vm.DesktopViewModels.Count);
            Assert.AreSame(dm1, _vm.DesktopViewModels[0].Desktop);
            Assert.AreSame(dm2, _vm.DesktopViewModels[1].Desktop);
        }

        [TestMethod]
        public void ConnectionCenterViewModel_SameHostNamesNoFriendlyName2_SortedAlphabetically()
        {
            DesktopModel dm1 = new DesktopModel() { HostName = "alpha", FriendlyName = "zod" };
            DesktopModel dm2 = new DesktopModel() { HostName = "alpha" };
            IList<Guid> ids = new List<Guid>();

            foreach (IModelContainer<RemoteConnectionModel> c in _dataModel.LocalWorkspace.Connections.Models)
                ids.Add(c.Id);
            foreach (Guid id in ids)
                _dataModel.LocalWorkspace.Connections.RemoveModel(id);

            _dataModel.LocalWorkspace.Connections.AddNewModel(dm1);
            _dataModel.LocalWorkspace.Connections.AddNewModel(dm2);

            Assert.AreEqual(2, _vm.DesktopViewModels.Count);
            Assert.AreSame(dm2, _vm.DesktopViewModels[0].Desktop);
            Assert.AreSame(dm1, _vm.DesktopViewModels[1].Desktop);
        }

        [TestMethod]
        public void ConnectionCenterViewModel_ChangeHostName_ResortedAlphabetically()
        {
            DesktopModel dm1 = new DesktopModel() { HostName = "alpha", FriendlyName = "zod" };
            DesktopModel dm2 = new DesktopModel() { HostName = "bravo", FriendlyName = "bod" };
            IList<Guid> ids = new List<Guid>();

            foreach (IModelContainer<RemoteConnectionModel> c in _dataModel.LocalWorkspace.Connections.Models)
                ids.Add(c.Id);
            foreach (Guid id in ids)
                _dataModel.LocalWorkspace.Connections.RemoveModel(id);

            _dataModel.LocalWorkspace.Connections.AddNewModel(dm1);
            _dataModel.LocalWorkspace.Connections.AddNewModel(dm2);
            dm1.HostName = "delta";
            //
            // Save the data model to apply all pending changes and trigger re-sorting
            // of all ordered observable collections.
            //
            _dataModel.Save.Execute(null);

            Assert.AreEqual(2, _vm.DesktopViewModels.Count);
            Assert.AreSame(dm2, _vm.DesktopViewModels[0].Desktop);
            Assert.AreSame(dm1, _vm.DesktopViewModels[1].Desktop);
        }

        [TestMethod]
        public void ConnectionCenterViewModel_ChangeFriendlyName_ResortedAlphabetically()
        {
            DesktopModel dm1 = new DesktopModel() { HostName = "alpha", FriendlyName = "zod" };
            DesktopModel dm2 = new DesktopModel() { HostName = "alpha", FriendlyName = "bod" };
            IList<Guid> ids = new List<Guid>();

            foreach (IModelContainer<RemoteConnectionModel> c in _dataModel.LocalWorkspace.Connections.Models)
                ids.Add(c.Id);
            foreach (Guid id in ids)
                _dataModel.LocalWorkspace.Connections.RemoveModel(id);

            _dataModel.LocalWorkspace.Connections.AddNewModel(dm1);
            _dataModel.LocalWorkspace.Connections.AddNewModel(dm2);
            dm2.FriendlyName = "zug";
            //
            // Save the data model to apply all pending changes and trigger re-sorting
            // of all ordered observable collections (sorting is triggered by the change
            // of the "Status" property of IModelContainer  to Clean in the original collection of
            // remote connections in the data model.
            //
            _dataModel.Save.Execute(null);

            Assert.AreEqual(2, _vm.DesktopViewModels.Count);
            Assert.AreSame(dm1, _vm.DesktopViewModels[0].Desktop);
            Assert.AreSame(dm2, _vm.DesktopViewModels[1].Desktop);
        }

        private void AssertDesktopViewModelSelectionEnabledMatchesDesktopsSelectable()
        {
            foreach (DesktopViewModel dvm in _vm.DesktopViewModels)
            {
                Assert.AreEqual(_vm.DesktopsSelectable, dvm.SelectionEnabled);
            }
        }

        private void AssertDesktopViewModelsMatchDesktops()
        {
            //checking there are the same number of DesktopViewModels as Desktops and that each Desktop is represented by a DesktopViewModel is sufficient
            Assert.AreEqual(_dataModel.LocalWorkspace.Connections.Models.Count, _vm.DesktopViewModels.Count);

            foreach (IModelContainer<RemoteConnectionModel> container in _dataModel.LocalWorkspace.Connections.Models)
            {
                if (container.Model is DesktopModel)
                {
                    Assert.IsTrue(_vm.DesktopViewModels.Any((dvm) => dvm.Desktop.Equals((DesktopModel)container.Model)));
                }
            }
        }
    }
}
