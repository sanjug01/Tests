namespace RdClient.Shared.Test
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using RdClient.Shared.ViewModels;
    using System.Collections.Generic;

    [TestClass]
    public class NavigationServiceTests
    {
        private IApplicationBarViewModel _appBarViewModel;

        /// <summary>
        /// Imitation view model that dismisses itself and reports a result object.
        /// Used in the functional test of returning results of modal dialog presenting.
        /// </summary>
        private sealed class TestModalViewModel : ViewModelBase
        {
            public void ReportResult(object result)
            {
                this.DismissModal(result);
            }
        }

        [TestInitialize]
        public void SetUpTest()
        {
            _appBarViewModel = new MainPageViewModel();
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _appBarViewModel = null;
        }

        [TestMethod]
        public void NavigateToViewNoViewModel_ShouldSucceed()
        {
            using(Mock.PresentableView view1 = new Mock.PresentableView())
            using(Mock.PresentableView view2 = new Mock.PresentableView())
            using(Mock.ViewFactory factory = new Mock.ViewFactory())
            using(Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                NavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                object activationParameter = new object();

                factory.Expect("CreateView", new List<object>() { "foo", activationParameter }, view1);
                view1.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PresentView", new List<object>() { view1 }, 0);

                navigationService.NavigateToView("foo", activationParameter);

                view1.Expect("Dismissing", new List<object>() { }, 0);
                factory.Expect("CreateView", new List<object>() { "bar", activationParameter }, view2);
                view2.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PresentView", new List<object>() { view2 }, 0);

                navigationService.NavigateToView("bar", activationParameter);

            }
        }

        [TestMethod]
        public void NavigateToView_ShouldSucceed()
        {
            using (Mock.PresentableView view1 = new Mock.PresentableView())
            using (Mock.PresentableView view2 = new Mock.PresentableView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            using (Mock.ViewModel viewModel = new Mock.ViewModel())
            {
                view1.ViewModel = viewModel;
                NavigationService navigationService = new NavigationService()
                {
                    Presenter = presenter,
                    ViewFactory = factory,
                    Extensions = new NavigationExtensionList()
                };
                object activationParameter = new object();

                factory.Expect("CreateView", new List<object>() { "foo", activationParameter }, view1);
                viewModel.Expect("Presenting", new List<object>() { navigationService, activationParameter, null }, 0);
                view1.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PresentView", new List<object>() { view1 }, 0);

                navigationService.NavigateToView("foo", activationParameter);

                viewModel.Expect("Dismissing", new List<object>() { }, 0);
                view1.Expect("Dismissing", new List<object>() { }, 0);
                factory.Expect("CreateView", new List<object>() { "bar", activationParameter }, view2);
                view2.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PresentView", new List<object>() { view2 }, 0);

                navigationService.NavigateToView("bar", activationParameter);

        }
        }

        [TestMethod]
        public void NavigateToInvalid_ShouldThrowException()
        {
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                bool exceptionThrown = false;
                NavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                object activationParameter = new object();

                factory.Expect("CreateView", new List<object>() { "foo", activationParameter }, null);

                try {
                    navigationService.NavigateToView("foo", activationParameter);                
                }
                catch(NavigationServiceException /* e */)
                {
                    exceptionThrown = true;
                }

                Assert.IsTrue(exceptionThrown);
            }

        }

        [TestMethod]
        public void RePresentView_ShouldRePresentView()
        {
            using (Mock.PresentableView view1 = new Mock.PresentableView())
            using (Mock.PresentableView view2 = new Mock.PresentableView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                NavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                object activationParameter = new object();

                factory.Expect("CreateView", new List<object>() { "foo", activationParameter }, view1);
                view1.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PresentView", new List<object>() { view1 }, 0);

                navigationService.NavigateToView("foo", activationParameter);

                view1.Expect("Dismissing", new List<object>() { }, 0);
                factory.Expect("CreateView", new List<object>() { "bar", activationParameter }, view2);
                view2.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PresentView", new List<object>() { view2 }, 0);

                navigationService.NavigateToView("bar", activationParameter);

                view2.Expect("Dismissing", new List<object>() { }, 0);
                factory.Expect("CreateView", new List<object>() { "foo", activationParameter }, view1);
                view1.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PresentView", new List<object>() { view1 }, 0);

                navigationService.NavigateToView("foo", activationParameter);
            }

        }

        [TestMethod]
        public void PresentViewWithNewParameter_ShouldRePresentView()
        {
            using (Mock.PresentableView view1 = new Mock.PresentableView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                NavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                object activationParameter = new object();

                factory.Expect("CreateView", new List<object>() { "foo", activationParameter }, view1);
                view1.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PresentView", new List<object>() { view1 }, 0);

                navigationService.NavigateToView("foo", activationParameter);

                factory.Expect("CreateView", new List<object>() { "foo", activationParameter }, view1);
                view1.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);

                navigationService.NavigateToView("foo", activationParameter);
            }
        }

        [TestMethod]
        public void PushModalView_ShouldPresentView()
        {
            using (Mock.PresentableView view1 = new Mock.PresentableView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                INavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                bool callbackCalled = false;
                object activationParameter = new object();

                navigationService.PushingFirstModalView += (sender, args) => { callbackCalled = true; };

                factory.Expect("CreateView", new List<object>() { "foo", activationParameter }, view1);
                view1.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PushModalView", new List<object>() { view1 }, 0);

                navigationService.PushModalView("foo", activationParameter);

                Assert.IsTrue(callbackCalled);
            }
        }

        [TestMethod]
        public void Push2ModalViews_PresentsBoth()
        {
            using (Mock.PresentableView view1 = new Mock.PresentableView())
            using (Mock.PresentableView view2 = new Mock.PresentableView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                INavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                bool callbackCalled = false;
                object activationParameter = new object();

                navigationService.PushingFirstModalView += (sender, args) => { callbackCalled = true; };

                factory.Expect("CreateView", new List<object>() { "foo", activationParameter }, view1);
                view1.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PushModalView", new List<object>() { view1 }, 0);

                navigationService.PushModalView("foo", activationParameter);

                factory.Expect("CreateView", new List<object>() { "bar", activationParameter }, view2);
                view2.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PushModalView", new List<object>() { view2 }, 0);

                navigationService.PushModalView("bar", activationParameter);

                Assert.IsTrue(callbackCalled);
            }
        }

        [TestMethod]
        public void Push2ModalViewsAndDismissTop_Dismissed()
        {
            using (Mock.PresentableView view1 = new Mock.PresentableView())
            using (Mock.PresentableView view2 = new Mock.PresentableView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                INavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                bool callbackCalled = false;
                object activationParameter = new object();

                navigationService.PushingFirstModalView += (sender, args) => { callbackCalled = true; };

                factory.Expect("CreateView", new List<object>() { "foo", activationParameter }, view1);
                view1.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PushModalView", new List<object>() { view1 }, 0);

                navigationService.PushModalView("foo", activationParameter);

                factory.Expect("CreateView", new List<object>() { "bar", activationParameter }, view2);
                view2.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PushModalView", new List<object>() { view2 }, 0);

                navigationService.PushModalView("bar", activationParameter);

                view2.Expect("Dismissing", new List<object>() { }, 0);
                presenter.Expect("DismissModalView", new List<object>() { view2 }, 0);
                navigationService.DismissModalView(view2);

                Assert.IsTrue(callbackCalled);
            }
        }

        [TestMethod]
        public void PushDismissModalView_ShouldPresentDismissView()
        {
            using (Mock.PresentableView view1 = new Mock.PresentableView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            using (Mock.ViewModel viewModel = new Mock.ViewModel())
            {
                view1.ViewModel = viewModel;

                NavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                navigationService.Extensions = new NavigationExtensionList();
                bool callbackCalled = false;
                object activationParameter = new object();

                navigationService.DismissingLastModalView += (sender, args) => { callbackCalled = true; };

                factory.Expect("CreateView", new List<object>() { "foo", activationParameter }, view1);
                viewModel.Expect("Presenting", new List<object>() { navigationService, activationParameter, null }, 0);
                view1.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PushModalView", new List<object>() { view1 }, 0);

                navigationService.PushModalView("foo", activationParameter, null);

                viewModel.Expect("Dismissing", new List<object>() { }, 0);
                view1.Expect("Dismissing", new List<object>() {  }, 0);
                presenter.Expect("DismissModalView", new List<object>() { view1 }, 0);

                navigationService.DismissModalView(view1);

                Assert.IsTrue(callbackCalled);
            }
        }

        [TestMethod]
        public void PushDismissModalViewNoViewModel_ShouldNotThrow()
        {
            using (Mock.PresentableView view1 = new Mock.PresentableView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                INavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                bool callbackCalled = false;
                object activationParameter = new object();

                navigationService.DismissingLastModalView += (sender, args) => { callbackCalled = true; };

                factory.Expect("CreateView", new List<object>() { "foo", activationParameter }, view1);
                view1.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PushModalView", new List<object>() { view1 }, 0);

                navigationService.PushModalView("foo", activationParameter);

                view1.Expect("Dismissing", new List<object>() { }, 0);
                presenter.Expect("DismissModalView", new List<object>() { view1 }, 0);

                navigationService.DismissModalView(view1);

                Assert.IsTrue(callbackCalled);
            }
        }

        [TestMethod]
        public void PushDismissModalViewNoCallback_ShouldNotThrow()
        {
            using (Mock.PresentableView view1 = new Mock.PresentableView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                INavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                object activationParameter = new object();

                factory.Expect("CreateView", new List<object>() { "foo", activationParameter }, view1);
                view1.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PushModalView", new List<object>() { view1 }, 0);

                navigationService.PushModalView("foo", activationParameter);

                view1.Expect("Dismissing", new List<object>() { }, 0);
                presenter.Expect("DismissModalView", new List<object>() { view1 }, 0);

                navigationService.DismissModalView(view1);
            }
        }

        [TestMethod]
        public void PushDismissModalViewStack_ShouldDismissAll3Views()
        {
            using (Mock.PresentableView view1 = new Mock.PresentableView())
            using (Mock.PresentableView view2 = new Mock.PresentableView())
            using (Mock.PresentableView view3 = new Mock.PresentableView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                INavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                bool callbackCalled = false;
                object activationParameter = new object();

                navigationService.DismissingLastModalView += (sender, args) => { callbackCalled = true; };

                factory.Expect("CreateView", new List<object>() { "foo", activationParameter }, view1);
                view1.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PushModalView", new List<object>() { view1 }, 0);

                factory.Expect("CreateView", new List<object>() { "bar", activationParameter }, view2);
                view2.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PushModalView", new List<object>() { view2 }, 0);

                factory.Expect("CreateView", new List<object>() { "narf", activationParameter }, view3);
                view3.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PushModalView", new List<object>() { view3 }, 0);

                navigationService.PushModalView("foo", activationParameter);
                navigationService.PushModalView("bar", activationParameter);
                navigationService.PushModalView("narf", activationParameter);

                view3.Expect("Dismissing", new List<object>() { }, 0);
                presenter.Expect("DismissModalView", new List<object>() { view3 }, 0);
                
                view2.Expect("Dismissing", new List<object>() { }, 0);
                presenter.Expect("DismissModalView", new List<object>() { view2 }, 0);

                view1.Expect("Dismissing", new List<object>() { }, 0);
                presenter.Expect("DismissModalView", new List<object>() { view1 }, 0);

                navigationService.DismissModalView(view1);

                Assert.IsTrue(callbackCalled);
            }
        }

        [TestMethod]
        public void PushModalViewStackDoubleDismiss_ShouldThrow()
        {
            using (Mock.PresentableView view1 = new Mock.PresentableView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                INavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                bool exceptionThrown = false;
                object activationParameter = new object();

                factory.Expect("CreateView", new List<object>() { "foo", activationParameter }, view1);
                view1.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PushModalView", new List<object>() { view1 }, 0);

                navigationService.PushModalView("foo", activationParameter);

                view1.Expect("Dismissing", new List<object>() { }, 0);
                presenter.Expect("DismissModalView", new List<object>() { view1 }, 0);

                navigationService.DismissModalView(view1);

                try
                {
                    navigationService.DismissModalView(view1);
                }
                catch(NavigationServiceException /* e */)
                {
                    exceptionThrown = true;
                }

                Assert.IsTrue(exceptionThrown);
            }
        }

        [TestMethod]
        public void PushModalViewStackDismissInvalid_ShouldThrow()
        {
            using (Mock.PresentableView view1 = new Mock.PresentableView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                NavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                bool exceptionThrown = false;
                object activationParameter = new object();

                try
                {
                    navigationService.DismissModalView(view1);
                }
                catch (NavigationServiceException /* e */)
                {
                    exceptionThrown = true;
                }

                Assert.IsTrue(exceptionThrown);
            }
        }

        [TestMethod]
        public void PushModalViewStackDoublePush_ShouldThrow()
        {
            using (Mock.PresentableView view1 = new Mock.PresentableView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                INavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                bool exceptionThrown = false;
                object activationParameter = new object();

                factory.Expect("CreateView", new List<object>() { "foo", activationParameter }, view1);
                view1.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PushModalView", new List<object>() { view1 }, 0);

                navigationService.PushModalView("foo", activationParameter);

                factory.Expect("CreateView", new List<object>() { "foo", activationParameter }, view1);

                try
                {
                    navigationService.PushModalView("foo", activationParameter);
                }
                catch (NavigationServiceException /* e */)
                {
                    exceptionThrown = true;
                }

                Assert.IsTrue(exceptionThrown);
            }
        }

        [TestMethod]
        public void PushModalViewStackPushPresented_ShouldThrow()
        {
            using (Mock.PresentableView view1 = new Mock.PresentableView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                INavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                bool exceptionThrown = false;
                object activationParameter = new object();

                factory.Expect("CreateView", new List<object>() { "foo", activationParameter }, view1);
                view1.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PresentView", new List<object>() { view1 }, 0);

                navigationService.NavigateToView("foo", activationParameter);

                factory.Expect("CreateView", new List<object>() { "foo", activationParameter }, view1);

                try
                {
                    navigationService.PushModalView("foo", activationParameter);
                }
                catch (NavigationServiceException /* e */)
                {
                    exceptionThrown = true;
                }

                Assert.IsTrue(exceptionThrown);
            }
        }

        [TestMethod]
        public void PresentViewModelWithItems_AppBarViewModelUpdated()
        {
            using (Mock.PresentableView view1 = new Mock.PresentableView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                NavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                navigationService.Extensions = new NavigationExtensionList()
                    {
                        new ApplicationBarExtension() { ViewModel = _appBarViewModel }
                    };

                Mock.BarItemsViewModel vm = new Mock.BarItemsViewModel();

                vm.Models = new BarItemModel[] { new Mock.TestBarItemModel(BarItemModel.ItemAlignment.Left, true) };
                view1.ViewModel = vm;

                factory.Expect("CreateView", new List<object>() { "foo", null }, view1);
                view1.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                vm.Expect("Presenting", new List<object>() { navigationService, null, null }, 0);
                presenter.Expect("PresentView", new List<object>() { view1 }, 0);

                navigationService.NavigateToView("foo", null);

                Assert.IsNotNull(_appBarViewModel.ShowBar);
                Assert.IsTrue(_appBarViewModel.IsShowBarButtonVisible);
                Assert.IsFalse(_appBarViewModel.IsBarVisible);
                Assert.IsFalse(_appBarViewModel.IsBarSticky);
                Assert.IsTrue(_appBarViewModel.IsBarAvailable);
                Assert.IsNotNull(_appBarViewModel.BarItems);
                Assert.IsInstanceOfType(_appBarViewModel.BarItems, typeof(BarItemModel[]));
                Assert.AreEqual(((BarItemModel[])_appBarViewModel.BarItems).Length, 1);
                Assert.IsNotNull(vm.ApplicationBarSite.ShowBar);
                Assert.IsNotNull(vm.ApplicationBarSite.HideBar);
            }
        }

        [TestMethod]
        public void PresentAnotherViewModelWithoutItems_AppBarViewModelUpdated()
        {
            using (Mock.PresentableView view1 = new Mock.PresentableView())
            using (Mock.PresentableView view2 = new Mock.PresentableView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                NavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                navigationService.Extensions = new NavigationExtensionList();

                Mock.BarItemsViewModel vm = new Mock.BarItemsViewModel();

                vm.Models = new BarItemModel[] { new Mock.TestBarItemModel(BarItemModel.ItemAlignment.Left, true) };
                view1.ViewModel = vm;

                factory.Expect("CreateView", new List<object>() { "foo", null }, view1);
                view1.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                vm.Expect("Presenting", new List<object>() { navigationService, null, null }, 0);
                presenter.Expect("PresentView", new List<object>() { view1 }, 0);

                navigationService.NavigateToView("foo", null);

                view1.Expect("Dismissing", new List<object>() { }, 0);
                vm.Expect("Dismissing", new List<object>() { }, 0);
                factory.Expect("CreateView", new List<object>() { "bar", null }, view2);
                view2.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                presenter.Expect("PresentView", new List<object>() { view2 }, 0);

                navigationService.NavigateToView("bar", null);

                Assert.IsFalse(_appBarViewModel.IsShowBarButtonVisible);
                Assert.IsFalse(_appBarViewModel.IsBarVisible);
                Assert.IsTrue(_appBarViewModel.IsBarSticky);
                Assert.IsNull(_appBarViewModel.BarItems);
                Assert.IsFalse(_appBarViewModel.IsBarAvailable);
            }
        }

        [TestMethod]
        public void PresentAnotherViewModelWithItems_AppBarViewModelUpdated()
        {
            using (Mock.PresentableView view1 = new Mock.PresentableView())
            using (Mock.PresentableView view2 = new Mock.PresentableView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                NavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                navigationService.Extensions = new NavigationExtensionList()
                    {
                        new ApplicationBarExtension() { ViewModel = _appBarViewModel }
                    };

                Mock.BarItemsViewModel
                    vm1 = new Mock.BarItemsViewModel(),
                    vm2 = new Mock.BarItemsViewModel();

                vm1.Models = new BarItemModel[] { new Mock.TestBarItemModel(BarItemModel.ItemAlignment.Left, true) };
                vm2.Models = new BarItemModel[] { new Mock.TestBarItemModel(BarItemModel.ItemAlignment.Left, true), new Mock.TestBarItemModel(BarItemModel.ItemAlignment.Left, true) };
                view1.ViewModel = vm1;
                view2.ViewModel = vm2;

                factory.Expect("CreateView", new List<object>() { "foo", null }, view1);
                view1.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                vm1.Expect("Presenting", new List<object>() { navigationService, null, null }, 0);
                presenter.Expect("PresentView", new List<object>() { view1 }, 0);

                navigationService.NavigateToView("foo", null);

                view1.Expect("Dismissing", new List<object>() { }, 0);
                vm1.Expect("Dismissing", new List<object>() { }, 0);
                vm2.Expect("Presenting", new List<object>() { navigationService, null, null }, 0);
                factory.Expect("CreateView", new List<object>() { "bar", null }, view2);
                view2.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                presenter.Expect("PresentView", new List<object>() { view2 }, 0);

                navigationService.NavigateToView("bar", null);

                Assert.IsTrue(_appBarViewModel.IsShowBarButtonVisible);
                Assert.IsFalse(_appBarViewModel.IsBarVisible);
                Assert.IsFalse(_appBarViewModel.IsBarSticky);
                Assert.IsNotNull(_appBarViewModel.BarItems);
                Assert.IsInstanceOfType(_appBarViewModel.BarItems, typeof(BarItemModel[]));
                Assert.AreEqual(((BarItemModel[])_appBarViewModel.BarItems).Length, 2);
            }
        }

        [TestMethod]
        public void PresentViewModelWithItems_ShowAppBar_AppBarShown()
        {
            using (Mock.PresentableView view1 = new Mock.PresentableView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                NavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                navigationService.Extensions = new NavigationExtensionList()
                    {
                        new ApplicationBarExtension() { ViewModel = _appBarViewModel }
                    };

                Mock.BarItemsViewModel vm = new Mock.BarItemsViewModel();

                vm.Models = new BarItemModel[] { new Mock.TestBarItemModel(BarItemModel.ItemAlignment.Left, true) };
                view1.ViewModel = vm;

                factory.Expect("CreateView", new List<object>() { "foo", null }, view1);
                view1.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                vm.Expect("Presenting", new List<object>() { navigationService, null, null }, 0);
                presenter.Expect("PresentView", new List<object>() { view1 }, 0);

                navigationService.NavigateToView("foo", null);

                Assert.IsNotNull(vm.ApplicationBarSite);
                Assert.IsTrue(vm.ApplicationBarSite.ShowBar.CanExecute(null));
                vm.ApplicationBarSite.ShowBar.Execute(null);
                Assert.IsTrue(_appBarViewModel.IsBarVisible);
            }
        }

        [TestMethod]
        public void PresentWithBarItems_PushModal_AllBarUIHidden()
        {
            using (Mock.PresentableView view1 = new Mock.PresentableView())
            using (Mock.PresentableView view2 = new Mock.PresentableView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            using (Mock.ViewModel viewModel = new Mock.ViewModel())
            using (Mock.BarItemsViewModel vmItems = new Mock.BarItemsViewModel())
            {
                vmItems.Models = new BarItemModel[] { new Mock.TestBarItemModel(BarItemModel.ItemAlignment.Left, true) };
                view1.ViewModel = vmItems;
                view2.ViewModel = viewModel;

                INavigationService navigationService = new NavigationService()
                {
                    Presenter = presenter,
                    ViewFactory = factory,
                    Extensions = new NavigationExtensionList()
                };

                factory.Expect("CreateView", new List<object>() { "foo", null }, view1);
                vmItems.Expect("Presenting", new List<object>() { navigationService, null, null }, 0);
                view1.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                presenter.Expect("PresentView", new List<object>() { view1 }, 0);

                navigationService.NavigateToView("foo", null);

                factory.Expect("CreateView", new List<object>() { "bar", null }, view2);
                view2.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                viewModel.Expect("Presenting", new List<object>() { navigationService, null, null }, 0);
                presenter.Expect("PushModalView", new List<object>() { view2 }, 0);
                navigationService.PushModalView("bar", null);

                Assert.IsFalse(_appBarViewModel.IsBarVisible);
                Assert.IsFalse(_appBarViewModel.IsShowBarButtonVisible);
                Assert.IsFalse(_appBarViewModel.ShowBar.CanExecute(null));
            }
        }

        [TestMethod]
        public void PresentWithBarItems_PushDismissModal_ShowBarVisible()
        {
            using (Mock.PresentableView view1 = new Mock.PresentableView())
            using (Mock.PresentableView view2 = new Mock.PresentableView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            using (Mock.ViewModel viewModel = new Mock.ViewModel())
            using (Mock.BarItemsViewModel vmItems = new Mock.BarItemsViewModel())
            {
                vmItems.Models = new BarItemModel[] { new Mock.TestBarItemModel(BarItemModel.ItemAlignment.Left, true) };
                view1.ViewModel = vmItems;
                view2.ViewModel = viewModel;

                INavigationService navigationService = new NavigationService()
                {
                    Presenter = presenter,
                    ViewFactory = factory,
                    Extensions = new NavigationExtensionList() { new ApplicationBarExtension() { ViewModel = _appBarViewModel } }
                };

                factory.Expect("CreateView", new List<object>() { "foo", null }, view1);
                vmItems.Expect("Presenting", new List<object>() { navigationService, null, null }, 0);
                view1.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                presenter.Expect("PresentView", new List<object>() { view1 }, 0);

                navigationService.NavigateToView("foo", null);

                factory.Expect("CreateView", new List<object>() { "bar", null }, view2);
                view2.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                viewModel.Expect("Presenting", new List<object>() { navigationService, null, null }, 0);
                presenter.Expect("PushModalView", new List<object>() { view2 }, 0);
                navigationService.PushModalView("bar", null);

                view2.Expect("Dismissing", new List<object>() { }, 0);
                viewModel.Expect("Dismissing", new List<object>() { }, 0);
                presenter.Expect("DismissModalView", new List<object>() { view2 }, 0);
                navigationService.DismissModalView(view2);

                Assert.IsFalse(_appBarViewModel.IsBarVisible);
                Assert.IsTrue(_appBarViewModel.IsShowBarButtonVisible);
                Assert.IsTrue(_appBarViewModel.ShowBar.CanExecute(null));
            }
        }

        [TestMethod]
        public void PresentWithBarItems_ShowBarPushModal_AllBarUIHidden()
        {
            using (Mock.PresentableView view1 = new Mock.PresentableView())
            using (Mock.PresentableView view2 = new Mock.PresentableView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            using (Mock.ViewModel viewModel = new Mock.ViewModel())
            using (Mock.BarItemsViewModel vmItems = new Mock.BarItemsViewModel())
            {
                vmItems.Models = new BarItemModel[] { new Mock.TestBarItemModel(BarItemModel.ItemAlignment.Left, true) };
                view1.ViewModel = vmItems;
                view2.ViewModel = viewModel;

                INavigationService navigationService = new NavigationService()
                {
                    Presenter = presenter,
                    ViewFactory = factory,
                    Extensions = new NavigationExtensionList() { new ApplicationBarExtension() { ViewModel = _appBarViewModel } }
                };

                factory.Expect("CreateView", new List<object>() { "foo", null }, view1);
                vmItems.Expect("Presenting", new List<object>() { navigationService, null, null }, 0);
                view1.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                presenter.Expect("PresentView", new List<object>() { view1 }, 0);

                navigationService.NavigateToView("foo", null);
                Assert.IsNotNull(vmItems.ApplicationBarSite);
                vmItems.ApplicationBarSite.ShowBar.Execute(null);
                Assert.IsTrue(_appBarViewModel.IsBarVisible);

                factory.Expect("CreateView", new List<object>() { "bar", null }, view2);
                view2.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                viewModel.Expect("Presenting", new List<object>() { navigationService, null, null }, 0);
                presenter.Expect("PushModalView", new List<object>() { view2 }, 0);
                navigationService.PushModalView("bar", null);

                Assert.IsFalse(_appBarViewModel.IsBarVisible);
                Assert.IsFalse(_appBarViewModel.IsShowBarButtonVisible);
                Assert.IsFalse(_appBarViewModel.ShowBar.CanExecute(null));
            }
        }

        [TestMethod]
        public void PresentModalWithCompletion_SetResult_ResultPassed()
        {
            using (Mock.PresentableView baseView = new Mock.PresentableView())
            using (Mock.PresentableView modalView = new Mock.PresentableView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                IList<PresentationCompletionEventArgs> completions = new List<PresentationCompletionEventArgs>();
                object modalResult = new object();
                ModalPresentationCompletion completion = new ModalPresentationCompletion();
                TestModalViewModel modalViewModel = new TestModalViewModel();

                modalView.ViewModel = modalViewModel;
                completion.Completed += (s, e) => completions.Add(e);

                INavigationService navigationService = new NavigationService()
                {
                    Presenter = presenter,
                    ViewFactory = factory
                };

                factory.Expect("CreateView", new List<object>() { "foo", null }, baseView);
                baseView.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                presenter.Expect("PresentView", new List<object>() { baseView }, 0);
                navigationService.NavigateToView("foo", null);

                factory.Expect("CreateView", new List<object>() { "bar", null }, modalView);
                modalView.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                presenter.Expect("PushModalView", new List<object>() { modalView }, 0);
                navigationService.PushModalView("bar", null, completion);


                modalView.Expect("Dismissing", new List<object>() { }, 0);
                presenter.Expect("DismissModalView", new List<object>() { modalView }, 0);
                modalViewModel.ReportResult(modalResult);

                Assert.AreEqual(1, completions.Count);
                Assert.AreSame(completions[0].Result, modalResult);
                Assert.AreSame(modalView, completions[0].View);
            }
        }

        [TestMethod]
        public void BackCommandCalledWithAlreadyHandledDoesNotCallViewModel()
        {
            using (Mock.PresentableView view = new Mock.PresentableView())
            using (Mock.ViewModel vm = new Mock.ViewModel())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                view.ViewModel = vm;
                INavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                IBackCommandArgs backArgs = new BackCommandArgs();
                backArgs.Handled = true;

                //setup nav so that view is presented
                factory.Expect("CreateView", o => { return view; });
                presenter.Expect("PresentView", null);
                view.Expect("Presenting", null);
                vm.Expect("Presenting", null);
                navigationService.NavigateToView("view", null);

                //calling back command when Handled is already true should do nothing
                navigationService.BackCommand.Execute(backArgs);
                Assert.IsTrue(backArgs.Handled);
            }
        }

        [TestMethod]
        public void BackCommandCalledWithAlreadyHandledDoesNotCallViewModelOrDismissView()
        {
            using (Mock.PresentableView view = new Mock.PresentableView())
            using (Mock.ViewModel vm = new Mock.ViewModel())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                view.ViewModel = vm;
                INavigationService nav = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                IBackCommandArgs backArgs = new BackCommandArgs();
                backArgs.Handled = true;

                //setup nav so that view is top modal view
                factory.Expect("CreateView", o => { return view; });
                presenter.Expect("PushModalView", null);
                view.Expect("Presenting", null);
                vm.Expect("Presenting", null);
                nav.PushModalView("view", null);

                //calling back command when Handled is already true should do nothing
                nav.BackCommand.Execute(backArgs);
                Assert.IsTrue(backArgs.Handled);
            }
        }        

        [TestMethod]
        public void BackCommandCalledWithNullArgsPassesValidArgsToViewModel()
        {
            using (Mock.PresentableView view = new Mock.PresentableView())
            using (Mock.ViewModel vm = new Mock.ViewModel())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                view.ViewModel = vm;
                INavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                IBackCommandArgs backArgs = null;

                factory.Expect("CreateView", o => { return view; });
                presenter.Expect("PresentView", null);
                view.Expect("Presenting", null);
                vm.Expect("Presenting", null);
                navigationService.NavigateToView("view", null);

                vm.Expect("NavigatingBack",
                    p =>
                    {
                        backArgs = p[0] as IBackCommandArgs;
                        Assert.IsNotNull(backArgs);
                        backArgs.Handled = true;
                        return null;
                    });
                Assert.IsNull(backArgs, "precondition");
                navigationService.BackCommand.Execute(null);
                Assert.IsTrue(backArgs.Handled);
            }
        }

        [TestMethod]
        public void BackCommandCallsViewModel()
        {
            using (Mock.PresentableView view = new Mock.PresentableView())
            using (Mock.ViewModel vm = new Mock.ViewModel())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                view.ViewModel = vm;
                INavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                IBackCommandArgs backArgs = new BackCommandArgs();
                bool vmCalled = false;

                factory.Expect("CreateView", o => { return view; });
                presenter.Expect("PresentView", null);
                view.Expect("Presenting", null);
                vm.Expect("Presenting", null);
                navigationService.NavigateToView("view", null);

                vm.Expect("NavigatingBack",
                    p =>
                    {
                        vmCalled = true;
                        IBackCommandArgs args = p[0] as IBackCommandArgs;
                        Assert.AreEqual(backArgs, args);
                        args.Handled = true;
                        return null;
                    });
                Assert.IsFalse(backArgs.Handled);
                Assert.IsFalse(vmCalled);
                navigationService.BackCommand.Execute(backArgs);
                Assert.IsTrue(vmCalled);
                Assert.IsTrue(backArgs.Handled);
            }
        }

        [TestMethod]
        public void BackCommandSetsHandledToFalseIfNonModalViewModelDoesNotHandleBackNavigation()
        {
            using (Mock.PresentableView view = new Mock.PresentableView())
            using (Mock.ViewModel vm = new Mock.ViewModel())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                view.ViewModel = vm;
                INavigationService nav = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                IBackCommandArgs backArgs = new BackCommandArgs();

                //setup nav so that view is currently presented view
                factory.Expect("CreateView", o => { return view; });
                presenter.Expect("PresentView", null);
                view.Expect("Presenting", null);
                vm.Expect("Presenting", null);
                nav.NavigateToView("view", null);

                //navigate back and verify it isn't handled
                vm.Expect("NavigatingBack", new List<object>() { backArgs }, 0);
                nav.BackCommand.Execute(backArgs);
                Assert.IsFalse(backArgs.Handled);
            }
        }

        [TestMethod]
        public void BackCommandDismissesModalViewIfItDoesNotHandleBackNavigation()
        {
            using (Mock.PresentableView view = new Mock.PresentableView())
            using (Mock.ViewModel vm = new Mock.ViewModel())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                view.ViewModel = vm;
                INavigationService nav = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                IBackCommandArgs backArgs = new BackCommandArgs();

                //setup nav so that view is top modal view
                factory.Expect("CreateView", o => { return view; });
                presenter.Expect("PushModalView", null);                
                view.Expect("Presenting", null);
                vm.Expect("Presenting", null);
                nav.PushModalView("view", null);

                //navigate back without vm handling it and verify view is dismissed
                vm.Expect("NavigatingBack", new List<object>() { backArgs }, 0);                
                presenter.Expect("DismissModalView", new List<object>() { view }, 0);
                view.Expect("Dismissing", null);
                vm.Expect("Dismissing", null);
                nav.BackCommand.Execute(backArgs);
                Assert.IsTrue(backArgs.Handled);
            }
        }

        [TestMethod]
        public void BackCommandDoesNotDismissModalViewIfItDoesHandleBackNavigation()
        {
            using (Mock.PresentableView view = new Mock.PresentableView())
            using (Mock.ViewModel vm = new Mock.ViewModel())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                view.ViewModel = vm;
                INavigationService nav = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                IBackCommandArgs backArgs = new BackCommandArgs();
                bool vmCalled = false;

                //setup nav so that view is top modal view
                factory.Expect("CreateView", o => { return view; });
                presenter.Expect("PushModalView", null);
                view.Expect("Presenting", null);
                vm.Expect("Presenting", null);
                nav.PushModalView("view", null);

                //navigate back have vm handle it and verify view is not dismissed
                vm.Expect("NavigatingBack", 
                    p =>
                    {
                        vmCalled = true;
                        IBackCommandArgs args = p[0] as IBackCommandArgs;
                        Assert.AreEqual(backArgs, args);
                        args.Handled = true;
                        return null;
                    });
                Assert.IsFalse(vmCalled);
                Assert.IsFalse(backArgs.Handled);
                nav.BackCommand.Execute(backArgs);
                Assert.IsTrue(vmCalled);
                Assert.IsTrue(backArgs.Handled);
            }
        }
    }
}
