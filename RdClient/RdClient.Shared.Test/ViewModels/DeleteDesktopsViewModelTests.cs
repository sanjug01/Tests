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
using RdClient.Shared.Test.Mock;
using RdClient.Shared.Helpers;

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

        private TestDeleteDesktopsViewModel _deleteDesktopsViewModel;
        private Mock.NavigationService _nav;
        private PresentableView _view;
        private ModalPresentationContext _context;

        class TestDeleteDesktopsViewModel : DeleteDesktopsViewModel
        {
            public TestDeleteDesktopsViewModel(ApplicationDataModel dataModel)
            {
                Assert.IsNotNull(dataModel);

                ((IDataModelSite)this).SetDataModel(dataModel);
            }
        }

        [TestInitialize]
        public void TestSetUp()
        {
            DesktopModel dtm;

            _testData = new TestData();

            // data model contains the selection plus additional random test data
            _dataModel = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer(),
                DataScrambler = new Mock.DummyDataScrambler()
            };
            _dataModel.Compose();

            _emptyDesktopsSelection = new List<IModelContainer<DesktopModel>>();

            // can pass a single desktop or a selection with a single element
            dtm = _testData.NewValidDesktop(Guid.Empty).Model;
            _singleDesktop = TemporaryModelContainer<DesktopModel>.WrapModel(_dataModel.LocalWorkspace.Connections.AddNewModel(dtm), dtm);
            _singleDesktopSelection = new List<IModelContainer<DesktopModel>>() { _singleDesktop };

            _multiDesktopsSelection = new List<IModelContainer<DesktopModel>>();

            dtm = _testData.NewValidDesktop(Guid.Empty).Model;
            _multiDesktopsSelection.Add(TemporaryModelContainer<DesktopModel>.WrapModel(_dataModel.LocalWorkspace.Connections.AddNewModel(dtm), dtm));
            dtm = _testData.NewValidDesktop(Guid.Empty).Model;
            _multiDesktopsSelection.Add(TemporaryModelContainer<DesktopModel>.WrapModel(_dataModel.LocalWorkspace.Connections.AddNewModel(dtm), dtm));
            dtm = _testData.NewValidDesktop(Guid.Empty).Model;
            _multiDesktopsSelection.Add(TemporaryModelContainer<DesktopModel>.WrapModel(_dataModel.LocalWorkspace.Connections.AddNewModel(dtm), dtm));

            _deleteDesktopsViewModel = new TestDeleteDesktopsViewModel(_dataModel);

            _nav = new Mock.NavigationService();
            _view = new Mock.PresentableView();
            _context = new Mock.ModalPresentationContext();
        }
        
        [TestCleanup]
        public void TestTearDown()
        {
            _nav.Dispose();
            _view.Dispose();
            _context.Dispose();
        }


        [TestMethod]
        public void DeleteDesktops_ShouldUpdateDataForEmptySelection()
        {
            DeleteDesktopsArgs args = new DeleteDesktopsArgs(_emptyDesktopsSelection);
            ((IViewModel)_deleteDesktopsViewModel).Presenting(_nav, args, null);

            Assert.AreEqual(0, _deleteDesktopsViewModel.DesktopsCount);
            Assert.IsFalse(_deleteDesktopsViewModel.IsSingleSelection);
        }

        [TestMethod]
        public void DeleteDesktops_ShouldUpdateDataForSingleSelection()
        {
            DeleteDesktopsArgs args = new DeleteDesktopsArgs(_singleDesktopSelection);

            string hostName = _singleDesktopSelection[0].Model.HostName;

            ((IViewModel)_deleteDesktopsViewModel).Presenting(_nav, args, null);

            Assert.AreEqual(1, _deleteDesktopsViewModel.DesktopsCount);
            Assert.IsTrue(_deleteDesktopsViewModel.IsSingleSelection);
        }

        [TestMethod]
        public void DeleteDesktops_ShouldUpdateDataForSingleDesktop()
        {
            DeleteDesktopsArgs args = new DeleteDesktopsArgs(_singleDesktop);

            string hostName = _singleDesktop.Model.HostName;

            ((IViewModel)_deleteDesktopsViewModel).Presenting(_nav, args, null);

            Assert.AreEqual(1, _deleteDesktopsViewModel.DesktopsCount);
            Assert.IsTrue(_deleteDesktopsViewModel.IsSingleSelection);
        }

        [TestMethod]
        public void DeleteDesktops_ShouldUpdateDataForMultiSelection()
        {
            DeleteDesktopsArgs args = new DeleteDesktopsArgs(_multiDesktopsSelection);
            string hostName = _multiDesktopsSelection[0].Model.HostName;
            string hostName2 = _multiDesktopsSelection[2].Model.HostName;

            ((IViewModel)_deleteDesktopsViewModel).Presenting(_nav, args, null);

            Assert.AreEqual(_multiDesktopsSelection.Count, _deleteDesktopsViewModel.DesktopsCount);
            Assert.IsFalse(_deleteDesktopsViewModel.IsSingleSelection);
        }

        [TestMethod]
        public void DeleteDesktops_DeleteShouldDismissDialog()
        {
            DeleteDesktopsArgs args = new DeleteDesktopsArgs(_singleDesktopSelection);
            ((IViewModel)_deleteDesktopsViewModel).Presenting(_nav, args, _context);

            _context.Expect("Dismiss", p => { return null; });
            _deleteDesktopsViewModel.DefaultAction.Execute(null);
        }

        [TestMethod]
        public void DeleteDesktops_CancelShouldDismissDialog()
        {
            DeleteDesktopsArgs args = new DeleteDesktopsArgs(_singleDesktopSelection);
            ((IViewModel)_deleteDesktopsViewModel).Presenting(_nav, args, _context);

            _context.Expect("Dismiss", p => { return null; });
            _deleteDesktopsViewModel.Cancel.Execute(null);
        }

        [TestMethod]
        public void DeleteDesktops_CancelShouldNotRemoveDesktops()
        {
            int initialCount, finalCount;
            DeleteDesktopsArgs args = new DeleteDesktopsArgs(_multiDesktopsSelection);

            ((IDataModelSite)_deleteDesktopsViewModel).SetDataModel(_dataModel);
            initialCount = _dataModel.LocalWorkspace.Connections.Models.Count;

            ((IViewModel)_deleteDesktopsViewModel).Presenting(_nav, args, null);
            Assert.IsTrue(_deleteDesktopsViewModel.DesktopsCount > 0);


            _deleteDesktopsViewModel.Cancel.Execute(null);
            finalCount = _dataModel.LocalWorkspace.Connections.Models.Count;

            Assert.AreEqual(initialCount, finalCount);
        }

        [TestMethod]
        public void DeleteDesktops_DeleteShouldRemoveDesktops()
        {
            int initialCount, finalCount, deletedCount;
            DeleteDesktopsArgs args = new DeleteDesktopsArgs(_multiDesktopsSelection);

            ((IDataModelSite)_deleteDesktopsViewModel).SetDataModel(_dataModel);
            initialCount = _dataModel.LocalWorkspace.Connections.Models.Count;

            ((IViewModel)_deleteDesktopsViewModel).Presenting(_nav, args, null);
            deletedCount = _deleteDesktopsViewModel.DesktopsCount;
            Assert.IsTrue(deletedCount > 0);


            _deleteDesktopsViewModel.DefaultAction.Execute(null);
            finalCount = _dataModel.LocalWorkspace.Connections.Models.Count;

            Assert.AreEqual(initialCount, finalCount + deletedCount);
        }

        [TestMethod]
        public void DeleteDesktops_DeleteShouldRemoveSingleDesktop()
        {
            int initialCount, finalCount;
            DeleteDesktopsArgs args = new DeleteDesktopsArgs(_singleDesktop);

            ((IDataModelSite)_deleteDesktopsViewModel).SetDataModel(_dataModel);
            initialCount = _dataModel.LocalWorkspace.Connections.Models.Count;

            ((IViewModel)_deleteDesktopsViewModel).Presenting(_nav, args, null);
            Assert.IsTrue(_deleteDesktopsViewModel.DesktopsCount > 0);


            _deleteDesktopsViewModel.DefaultAction.Execute(null);
            finalCount = _dataModel.LocalWorkspace.Connections.Models.Count;

            Assert.AreEqual(initialCount, finalCount + 1);
        }
    }
}
