namespace RdClient.Shared.Test.ViewModels
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.ViewModels;
    using System.Collections.Generic;

    [TestClass]
    public class MainPageViewModelTests
    {
        private MainPageViewModel _vm;
        private IList<BarItemModel> _visibleModels;

        private class TestBarItemModel : BarItemModel
        {
            public TestBarItemModel() { }
        }

        [TestInitialize]
        public void SetUpTest()
        {
            _vm = new MainPageViewModel();
            _visibleModels = new List<BarItemModel>() { new TestBarItemModel() };
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _vm = null;
            _visibleModels = null;
        }

        [TestMethod]
        public void NewViewModel_CorrectApplicationBarProperties()
        {
            Assert.IsNull(_vm.BarItems);
            Assert.IsFalse(_vm.IsShowBarButtonVisible);
            Assert.IsFalse(_vm.IsBarVisible);
            Assert.IsFalse(_vm.IsBarSticky);
        }

        [TestMethod]
        public void NewViewModel_ChangeIsBarVisible_ChangeReported()
        {
            IList<string> reportedProperties = new List<string>();

            _vm.PropertyChanged += (s, e) => reportedProperties.Add(e.PropertyName);
            _vm.IsBarVisible = true;
            Assert.IsTrue(_vm.IsBarVisible);
            Assert.IsTrue(1 == reportedProperties.Count);
            Assert.AreEqual(reportedProperties[0], "IsBarVisible");
        }

        [TestMethod]
        public void NewViewModel_ChangeIsBarSticky_ChangeReported()
        {
            IList<string> reportedProperties = new List<string>();

            _vm.PropertyChanged += (s, e) => reportedProperties.Add(e.PropertyName);
            _vm.IsBarSticky = true;
            Assert.IsTrue(_vm.IsBarSticky);
            Assert.IsTrue(1 == reportedProperties.Count);
            Assert.AreEqual(reportedProperties[0], "IsBarSticky");
        }

        [TestMethod]
        public void NewViewModel_ChangeBarItems_ChangeReported()
        {
            IList<string> reportedProperties = new List<string>();

            _vm.PropertyChanged += (s, e) => reportedProperties.Add(e.PropertyName);
            _vm.BarItems = new List<BarItemModel>();
            Assert.IsNotNull(_vm.BarItems);
            Assert.IsTrue(1 == reportedProperties.Count);
            Assert.AreEqual(reportedProperties[0], "BarItems");
        }

        [TestMethod]
        public void NewViewModel_CannotExecuteShowBar()
        {
            Assert.IsFalse(_vm.ShowBar.CanExecute(null));
        }

        [TestMethod]
        public void NewBar_SetVisibleModels_ShowButtonVisible()
        {
            IList<string> reportedProperties = new List<string>();

            _vm.PropertyChanged += (s, e) => reportedProperties.Add(e.PropertyName);
            _vm.BarItems = _visibleModels;

            Assert.IsTrue(_vm.IsShowBarButtonVisible);
            Assert.AreEqual(2, reportedProperties.Count);
            Assert.AreEqual("BarItems", reportedProperties[0]);
            Assert.AreEqual("IsShowBarButtonVisible", reportedProperties[1]);
        }

        [TestMethod]
        public void NewBar_SetAndClearVisibleModels_AllUIHidden()
        {
            IList<string> reportedProperties = new List<string>();

            _vm.PropertyChanged += (s, e) => reportedProperties.Add(e.PropertyName);
            _vm.BarItems = _visibleModels;
            Assert.IsTrue(_vm.IsShowBarButtonVisible);
            Assert.IsFalse(_vm.IsBarVisible);
            _vm.BarItems = null;
            Assert.IsFalse(_vm.IsShowBarButtonVisible);
            Assert.IsFalse(_vm.IsBarVisible);
        }

        [TestMethod]
        public void HasVisibleModels_HideAllModels_AllUIHidden()
        {
            _vm.BarItems = _visibleModels;
            Assert.IsTrue(_vm.IsShowBarButtonVisible);
            Assert.IsFalse(_vm.IsBarVisible);
            _visibleModels[0].IsVisible = false;
            Assert.IsFalse(_vm.IsShowBarButtonVisible);
            Assert.IsFalse(_vm.IsBarVisible);
        }

        [TestMethod]
        public void HasVisibleModels_ShowBarHideAllModels_AllUIHidden()
        {
            _vm.BarItems = _visibleModels;
            _vm.ShowBar.Execute(null);
            Assert.IsFalse(_vm.IsShowBarButtonVisible);
            Assert.IsTrue(_vm.IsBarVisible);
            _visibleModels[0].IsVisible = false;
            Assert.IsFalse(_vm.IsShowBarButtonVisible);
            Assert.IsFalse(_vm.IsBarVisible);
        }

        [TestMethod]
        public void NoVisibleModels_AllUIHidden()
        {
            _visibleModels[0].IsVisible = false;
            _vm.BarItems = _visibleModels;
            Assert.IsFalse(_vm.IsShowBarButtonVisible);
            Assert.IsFalse(_vm.IsBarVisible);
        }

        [TestMethod]
        public void ButtonVisible_ExecuteShowBar_BarShowsButtonHides()
        {
            _vm.BarItems = _visibleModels;
            _vm.ShowBar.Execute(null);
            Assert.IsTrue(_vm.IsBarVisible);
            Assert.IsFalse(_vm.IsShowBarButtonVisible);
        }

        [TestMethod]
        public void ShowAppBar_ClearBarItems_AllUIHidden()
        {
            _vm.BarItems = new BarItemModel[] { new TestBarItemModel(), new TestBarItemModel() };
            Assert.IsTrue(_vm.IsShowBarButtonVisible);
        }

        [TestMethod]
        public void NewViewModel_LandscapeOrientation()
        {
            Assert.AreEqual(_vm.ApplicationBarLayout, ViewOrientation.Landscape);
        }

        [TestMethod]
        public void NewViewModel_ChangeOrientationToLandscape_ChangeNotReported()
        {
            IList<string> propertyReported = new List<string>();
            ILayoutAwareViewModel lavm = _vm;

            _vm.PropertyChanged += (s, e) => propertyReported.Add(e.PropertyName);
            lavm.OrientationChanged(ViewOrientation.Landscape);
            Assert.AreEqual(_vm.ApplicationBarLayout, ViewOrientation.Landscape);
            Assert.AreEqual(0, propertyReported.Count);
        }

        [TestMethod]
        public void NewViewModel_ChangeOrientationToPortrait_ChangeReported()
        {
            IList<string> propertyReported = new List<string>();
            ILayoutAwareViewModel lavm = _vm;

            _vm.PropertyChanged += (s, e) => propertyReported.Add(e.PropertyName);
            lavm.OrientationChanged(ViewOrientation.Portrait);
            Assert.AreEqual(_vm.ApplicationBarLayout, ViewOrientation.Portrait);
            Assert.AreEqual(1, propertyReported.Count);
            Assert.AreEqual("ApplicationBarLayout", propertyReported[0]);
        }
    }
}
