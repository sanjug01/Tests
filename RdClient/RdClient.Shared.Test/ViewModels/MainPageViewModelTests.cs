namespace RdClient.Shared.Test.ViewModels
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
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
            Assert.IsTrue(2 == reportedProperties.Count);
            Assert.IsTrue(reportedProperties.Contains("BarItems"));
            Assert.IsTrue(reportedProperties.Contains("IsBarAvailable"));
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
            Assert.AreEqual(3, reportedProperties.Count);
            Assert.IsTrue(reportedProperties.Contains("BarItems"));
            Assert.IsTrue(reportedProperties.Contains("IsShowBarButtonVisible"));
            Assert.IsTrue(reportedProperties.Contains("IsBarAvailable"));
        }

        [TestMethod]
        public void NewBar_SetAndClearVisibleModels_AllUIHidden()
        {
            IList<string> reportedProperties = new List<string>();

            _vm.PropertyChanged += (s, e) => reportedProperties.Add(e.PropertyName);
            _vm.BarItems = _visibleModels;
            Assert.IsTrue(_vm.IsBarAvailable);
            Assert.IsTrue(_vm.IsShowBarButtonVisible);
            Assert.IsFalse(_vm.IsBarVisible);
            _vm.BarItems = null;
            Assert.IsFalse(_vm.IsBarAvailable);
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

        [TestMethod]
        public void PresentViewWithButtonModels_ShowAndHideAppBar_ShowsAndHides()
        {
            IPresentableViewFactory factory = new PresentableViewFactoryConcrete();
            MainPageViewModel mainVM = new MainPageViewModel();

            factory.AddViewClass("view", typeof(TestViewWithModelWithBarItems), false);

            NavigationService nav = new NavigationService()
            {
                ViewFactory = factory,
                Presenter = new TestViewPresenter(),
                Extensions = new NavigationExtensionList() { new ApplicationBarExtension() { ViewModel = mainVM } }
            };

            PresentationParameter param = new PresentationParameter();
            nav.NavigateToView("view", param);
            Assert.IsNotNull(param.ViewModel);
            Assert.IsTrue(param.ViewModel.BarSite.ShowBar.CanExecute(null));
            param.ViewModel.BarSite.ShowBar.Execute(null);
            Assert.IsTrue(param.ViewModel.BarSite.IsBarVisible);
            param.ViewModel.BarSite.HideBar.Execute(null);
            Assert.IsFalse(param.ViewModel.BarSite.IsBarVisible);
        }

        [TestMethod]
        public void PresentViewWithButtonModels_ShowBarHideAllModels_BarUIHides()
        {
            IPresentableViewFactory factory = new PresentableViewFactoryConcrete();
            MainPageViewModel mainVM = new MainPageViewModel();

            factory.AddViewClass("view", typeof(TestViewWithModelWithBarItems), false);

            NavigationService nav = new NavigationService()
            {
                ViewFactory = factory,
                Presenter = new TestViewPresenter(),
                Extensions = new NavigationExtensionList() { new ApplicationBarExtension() { ViewModel = mainVM } }
            };

            PresentationParameter param = new PresentationParameter();
            nav.NavigateToView("view", param);
            Assert.IsNotNull(param.ViewModel);
            Assert.IsTrue(param.ViewModel.BarSite.ShowBar.CanExecute(null));
            param.ViewModel.BarSite.ShowBar.Execute(null);
            Assert.IsTrue(param.ViewModel.BarSite.IsBarVisible);
            param.ViewModel.Models[0].IsVisible = false;
            Assert.IsFalse(mainVM.IsBarVisible);
            Assert.IsFalse(mainVM.IsShowBarButtonVisible);
        }

        [TestMethod]
        public void PresentViewWithButtonModels_ShowBarHideAllModelsShowModel_BarUIReappears()
        {
            IPresentableViewFactory factory = new PresentableViewFactoryConcrete();
            MainPageViewModel mainVM = new MainPageViewModel();

            factory.AddViewClass("view", typeof(TestViewWithModelWithBarItems), false);

            NavigationService nav = new NavigationService()
            {
                ViewFactory = factory,
                Presenter = new TestViewPresenter(),
                Extensions = new NavigationExtensionList() { new ApplicationBarExtension() { ViewModel = mainVM } }
            };

            PresentationParameter param = new PresentationParameter();
            nav.NavigateToView("view", param);
            Assert.IsNotNull(param.ViewModel);
            Assert.IsTrue(param.ViewModel.BarSite.ShowBar.CanExecute(null));
            param.ViewModel.BarSite.ShowBar.Execute(null);
            Assert.IsTrue(param.ViewModel.BarSite.IsBarVisible);
            param.ViewModel.Models[0].IsVisible = false;
            Assert.IsFalse(mainVM.IsBarVisible);
            Assert.IsFalse(mainVM.IsShowBarButtonVisible);
            Assert.IsFalse(mainVM.IsBarAvailable);
            param.ViewModel.Models[0].IsVisible = true;
            Assert.IsTrue(mainVM.IsBarAvailable);
            Assert.IsFalse(mainVM.IsBarVisible);
            Assert.IsTrue(mainVM.IsShowBarButtonVisible);
        }

        private sealed class PresentationParameter
        {
            public TestViewModel ViewModel;
        }

        private class TestViewWithModelWithBarItems : IPresentableView
        {
            private readonly IViewModel _viewModel;

            public TestViewWithModelWithBarItems()
            {
                _viewModel = new TestViewModel();
            }

            IViewModel IPresentableView.ViewModel { get { return _viewModel; } }

            void IPresentableView.Activating(object activationParameter) { }

            void IPresentableView.Presenting(INavigationService navigationService, object activationParameter) { }

            void IPresentableView.Dismissing() { }
        }

        private sealed class TestViewModel : ViewModelBase, IApplicationBarItemsSource
        {
            public readonly BarItemModel[] Models = new BarItemModel[] { new SeparatorBarItemModel() };
            public IApplicationBarSite BarSite;

            IEnumerable<BarItemModel> IApplicationBarItemsSource.GetItems(IApplicationBarSite applicationBarSite)
            {
                this.BarSite = applicationBarSite;
                return this.Models;
            }

            protected override void OnPresenting(object activationParameter)
            {
                base.OnPresenting(activationParameter);
                ((PresentationParameter)activationParameter).ViewModel = this;
            }
        }

        private sealed class TestViewPresenter : IViewPresenter
        {
            void IViewPresenter.PresentView(IPresentableView view) { }
            void IViewPresenter.PushModalView(IPresentableView view) { }
            void IViewPresenter.DismissModalView(IPresentableView view) { }
            void IViewPresenter.PresentingFirstModalView() { }
            void IViewPresenter.DismissedLastModalView() { }
        }
    }
}
