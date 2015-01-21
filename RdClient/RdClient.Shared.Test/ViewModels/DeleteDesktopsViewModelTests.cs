using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class DeleteDesktopsViewModelTests
    {
        private TestData _testData;
        private List<Desktop> _emptyDesktopsSelection;
        private List<Desktop> _singleDesktopSelection;
        private Desktop _singleDesktop;
        private List<Desktop> _multiDesktopsSelection;

        private RdDataModel _dataModel;

        class TestDeleteDesktopsViewModel : DeleteDesktopsViewModel
        {
            public TestDeleteDesktopsViewModel()
            {
                DialogView = new Mock.PresentableView();
                DataModel = new RdDataModel();
            }
        }

        private TestDeleteDesktopsViewModel _deleteDesktopsViewModel;


        [TestInitialize]
        public void TestSetUp()
        {
            _testData = new TestData();

            _emptyDesktopsSelection = new List<Desktop>();

            // can pass a single desktop or a selection with a single element
            _singleDesktopSelection = new List<Desktop>();
            _singleDesktop = _testData.NewValidDesktop(Guid.Empty);
            _singleDesktopSelection.Add(_singleDesktop);

            _multiDesktopsSelection = new List<Desktop>();
            _multiDesktopsSelection.Add(_testData.NewValidDesktop(Guid.Empty));
            _multiDesktopsSelection.Add(_testData.NewValidDesktop(Guid.Empty));
            _multiDesktopsSelection.Add(_testData.NewValidDesktop(Guid.Empty));

            // data model contains the selection plus additional random test data
            _dataModel = new RdDataModel();

            _dataModel.LocalWorkspace.Connections.Add(_singleDesktopSelection[0] as Desktop);
            _dataModel.LocalWorkspace.Connections.Add(_testData.NewValidDesktop(Guid.Empty));

            _dataModel.LocalWorkspace.Connections.Add(_multiDesktopsSelection[0] as Desktop);
            _dataModel.LocalWorkspace.Connections.Add(_multiDesktopsSelection[1] as Desktop);
            _dataModel.LocalWorkspace.Connections.Add(_multiDesktopsSelection[2] as Desktop);
            _dataModel.LocalWorkspace.Connections.Add(_testData.NewValidDesktop(Guid.Empty));

            _deleteDesktopsViewModel = new TestDeleteDesktopsViewModel();
        }
        
        [TestCleanup]
        public void TestTearDown()
        {
            _singleDesktopSelection.Clear();
            _multiDesktopsSelection.Clear();
            _deleteDesktopsViewModel = null;
        }


        [TestMethod]
        public void DeleteDesktops_ShouldUpdateDataForEmptySelection()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                DeleteDesktopsArgs args = new DeleteDesktopsArgs(_emptyDesktopsSelection);
                ((IViewModel)_deleteDesktopsViewModel).Presenting(navigation, args, null);

                Assert.AreEqual(0,_deleteDesktopsViewModel.DesktopsCount);
                Assert.IsFalse(_deleteDesktopsViewModel.IsSingleSelection);
            }
        }

        [TestMethod]
        public void DeleteDesktops_ShouldUpdateDataForSingleSelection()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                DeleteDesktopsArgs args = new DeleteDesktopsArgs(_singleDesktopSelection);
                string hostName = (_singleDesktopSelection[0] as Desktop).HostName;

                ((IViewModel)_deleteDesktopsViewModel).Presenting(navigation, args, null);

                Assert.AreEqual(1, _deleteDesktopsViewModel.DesktopsCount);
                Assert.IsTrue(_deleteDesktopsViewModel.IsSingleSelection);                
            }
        }

        [TestMethod]
        public void DeleteDesktops_ShouldUpdateDataForSingleDesktop()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                DeleteDesktopsArgs args = new DeleteDesktopsArgs(_singleDesktop);
                string hostName = _singleDesktop.HostName;

                ((IViewModel)_deleteDesktopsViewModel).Presenting(navigation, args, null);

                Assert.AreEqual(1, _deleteDesktopsViewModel.DesktopsCount);
                Assert.IsTrue(_deleteDesktopsViewModel.IsSingleSelection);
            }
        }

        [TestMethod]
        public void DeleteDesktops_ShouldUpdateDataForMultiSelection()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                DeleteDesktopsArgs args = new DeleteDesktopsArgs(_multiDesktopsSelection);
                string hostName = (_multiDesktopsSelection[0] as Desktop).HostName;
                string hostName2 = (_multiDesktopsSelection[2] as Desktop).HostName;

                ((IViewModel)_deleteDesktopsViewModel).Presenting(navigation, args, null);

                Assert.AreEqual(_multiDesktopsSelection.Count, _deleteDesktopsViewModel.DesktopsCount);
                Assert.IsFalse(_deleteDesktopsViewModel.IsSingleSelection);
            }
        }

        [TestMethod]
        public void DeleteDesktops_DeleteShouldDismissDialog()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                DeleteDesktopsArgs args = new DeleteDesktopsArgs(_singleDesktopSelection);
                _deleteDesktopsViewModel.DialogView = view;
                navigation.Expect("DismissModalView", new List<object> { view }, 0);

                ((IViewModel)_deleteDesktopsViewModel).Presenting(navigation, args, null);
                _deleteDesktopsViewModel.DeleteCommand.Execute(null);
            }
        }

        [TestMethod]
        public void DeleteDesktops_CancelShouldDismissDialog()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                DeleteDesktopsArgs args = new DeleteDesktopsArgs(_singleDesktopSelection);
                _deleteDesktopsViewModel.DialogView = view;
                navigation.Expect("DismissModalView", new List<object> { view }, 0);

                ((IViewModel)_deleteDesktopsViewModel).Presenting(navigation, args, null);
                _deleteDesktopsViewModel.CancelCommand.Execute(null);
            }
        }

        [TestMethod]
        public void DeleteDesktops_CancelShouldNotRemoveDesktops()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                int initialCount, finalCount;
                DeleteDesktopsArgs args = new DeleteDesktopsArgs(_multiDesktopsSelection);

                _deleteDesktopsViewModel.DialogView = view;
                navigation.Expect("DismissModalView", new List<object> { view }, 0);
                _deleteDesktopsViewModel.DataModel = _dataModel;
                initialCount = _dataModel.LocalWorkspace.Connections.Count;

                ((IViewModel)_deleteDesktopsViewModel).Presenting(navigation, args, null);
                Assert.IsTrue(_deleteDesktopsViewModel.DesktopsCount > 0);


                _deleteDesktopsViewModel.CancelCommand.Execute(null);
                finalCount = _dataModel.LocalWorkspace.Connections.Count;

                Assert.AreEqual(initialCount, finalCount);
            }
        }

        [TestMethod]
        public void DeleteDesktops_DeleteShouldRemoveDesktops()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                int initialCount, finalCount, deletedCount;
                DeleteDesktopsArgs args = new DeleteDesktopsArgs(_multiDesktopsSelection);
                _deleteDesktopsViewModel.DialogView = view;
                navigation.Expect("DismissModalView", new List<object> { view }, 0);

                _deleteDesktopsViewModel.DataModel = _dataModel;
                initialCount = _dataModel.LocalWorkspace.Connections.Count;

                ((IViewModel)_deleteDesktopsViewModel).Presenting(navigation, args, null);
                deletedCount = _deleteDesktopsViewModel.DesktopsCount;
                Assert.IsTrue(deletedCount > 0);


                _deleteDesktopsViewModel.DeleteCommand.Execute(null);
                finalCount = _dataModel.LocalWorkspace.Connections.Count;

                Assert.AreEqual(initialCount, finalCount + deletedCount);
            }
        }

        [TestMethod]
        public void DeleteDesktops_DeleteShouldRemoveSingleDesktop()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                int initialCount, finalCount;
                DeleteDesktopsArgs args = new DeleteDesktopsArgs(_singleDesktop);

                _deleteDesktopsViewModel.DialogView = view;
                navigation.Expect("DismissModalView", new List<object> { view }, 0);

                _deleteDesktopsViewModel.DataModel = _dataModel;
                initialCount = _dataModel.LocalWorkspace.Connections.Count;

                ((IViewModel)_deleteDesktopsViewModel).Presenting(navigation, args, null);
                Assert.IsTrue(_deleteDesktopsViewModel.DesktopsCount > 0);


                _deleteDesktopsViewModel.DeleteCommand.Execute(null);
                finalCount = _dataModel.LocalWorkspace.Connections.Count;

                Assert.AreEqual(initialCount, finalCount + 1);
            }
        }
    }
}
