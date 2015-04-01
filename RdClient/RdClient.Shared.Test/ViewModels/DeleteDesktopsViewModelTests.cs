using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.Data;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.Navigation.Extensions;
using RdClient.Shared.Test.Data;
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
        private IList<IModelContainer<DesktopModel>> _emptyDesktopsSelection;
        private IList<IModelContainer<DesktopModel>> _singleDesktopSelection;
        private IModelContainer<DesktopModel> _singleDesktop;
        private IList<IModelContainer<DesktopModel>> _multiDesktopsSelection;

        private ApplicationDataModel _dataModel;

        class TestDeleteDesktopsViewModel : DeleteDesktopsViewModel
        {
            public TestDeleteDesktopsViewModel(ApplicationDataModel dataModel)
            {
                Assert.IsNotNull(dataModel);

                this.DialogView = new Mock.PresentableView();
                ((IDataModelSite)this).SetDataModel(dataModel);
            }
        }

        private TestDeleteDesktopsViewModel _deleteDesktopsViewModel;


        [TestInitialize]
        public void TestSetUp()
        {
            DesktopModel dtm;

            _testData = new TestData();

            // data model contains the selection plus additional random test data
            _dataModel = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer()
            };

            _emptyDesktopsSelection = new List<IModelContainer<DesktopModel>>();

            // can pass a single desktop or a selection with a single element
            dtm = _testData.NewValidDesktop(Guid.Empty);
            _singleDesktop = TemporaryModelContainer<DesktopModel>.WrapModel(_dataModel.LocalWorkspace.Connections.AddNewModel(dtm), dtm);
            _singleDesktopSelection = new List<IModelContainer<DesktopModel>>() { _singleDesktop };

            _multiDesktopsSelection = new List<IModelContainer<DesktopModel>>();

            dtm = _testData.NewValidDesktop(Guid.Empty);
            _multiDesktopsSelection.Add(TemporaryModelContainer<DesktopModel>.WrapModel(_dataModel.LocalWorkspace.Connections.AddNewModel(dtm), dtm));
            dtm = _testData.NewValidDesktop(Guid.Empty);
            _multiDesktopsSelection.Add(TemporaryModelContainer<DesktopModel>.WrapModel(_dataModel.LocalWorkspace.Connections.AddNewModel(dtm), dtm));
            dtm = _testData.NewValidDesktop(Guid.Empty);
            _multiDesktopsSelection.Add(TemporaryModelContainer<DesktopModel>.WrapModel(_dataModel.LocalWorkspace.Connections.AddNewModel(dtm), dtm));

            _deleteDesktopsViewModel = new TestDeleteDesktopsViewModel(_dataModel);
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

                string hostName = _singleDesktopSelection[0].Model.HostName;

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

                string hostName = _singleDesktop.Model.HostName;

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
                string hostName = _multiDesktopsSelection[0].Model.HostName;
                string hostName2 = _multiDesktopsSelection[2].Model.HostName;

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
                ((IDataModelSite)_deleteDesktopsViewModel).SetDataModel(_dataModel);
                initialCount = _dataModel.LocalWorkspace.Connections.Models.Count;

                ((IViewModel)_deleteDesktopsViewModel).Presenting(navigation, args, null);
                Assert.IsTrue(_deleteDesktopsViewModel.DesktopsCount > 0);


                _deleteDesktopsViewModel.CancelCommand.Execute(null);
                finalCount = _dataModel.LocalWorkspace.Connections.Models.Count;

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

                ((IDataModelSite)_deleteDesktopsViewModel).SetDataModel(_dataModel);
                initialCount = _dataModel.LocalWorkspace.Connections.Models.Count;

                ((IViewModel)_deleteDesktopsViewModel).Presenting(navigation, args, null);
                deletedCount = _deleteDesktopsViewModel.DesktopsCount;
                Assert.IsTrue(deletedCount > 0);


                _deleteDesktopsViewModel.DeleteCommand.Execute(null);
                finalCount = _dataModel.LocalWorkspace.Connections.Models.Count;

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

                ((IDataModelSite)_deleteDesktopsViewModel).SetDataModel(_dataModel);
                initialCount = _dataModel.LocalWorkspace.Connections.Models.Count;

                ((IViewModel)_deleteDesktopsViewModel).Presenting(navigation, args, null);
                Assert.IsTrue(_deleteDesktopsViewModel.DesktopsCount > 0);


                _deleteDesktopsViewModel.DeleteCommand.Execute(null);
                finalCount = _dataModel.LocalWorkspace.Connections.Models.Count;

                Assert.AreEqual(initialCount, finalCount + 1);
            }
        }
    }
}
