using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.Navigation;
using RdClient.Shared.Navigation.Extensions;
using RdClient.Shared.ViewModels;
using System.Collections.Generic;

namespace RdClient.Shared.Test
{
    [TestClass]
    public class NavigationServiceTests
    {
        private IApplicationBarViewModel _appBarViewModel;

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
                NavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                navigationService.Extensions = new NavigationExtensionList();
                object activationParameter = new object();

                factory.Expect("CreateView", new List<object>() { "foo", activationParameter }, view1);
                viewModel.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
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
                NavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
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
                viewModel.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                view1.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PushModalView", new List<object>() { view1 }, 0);

                navigationService.PushModalView("foo", activationParameter);

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
                NavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
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
                NavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
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
                NavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
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
                NavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
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
                NavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
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
                NavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
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
                vm.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                presenter.Expect("PresentView", new List<object>() { view1 }, 0);

                navigationService.NavigateToView("foo", null);

                Assert.IsTrue(_appBarViewModel.IsShowBarButtonVisible);
                Assert.IsFalse(_appBarViewModel.IsBarVisible);
                Assert.IsFalse(_appBarViewModel.IsBarSticky);
                Assert.IsNotNull(_appBarViewModel.BarItems);
                Assert.IsInstanceOfType(_appBarViewModel.BarItems, typeof(BarItemModel[]));
                Assert.AreEqual(((BarItemModel[])_appBarViewModel.BarItems).Length, 1);
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
                vm.Expect("Presenting", new List<object>() { navigationService, null }, 0);
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
                Assert.IsFalse(_appBarViewModel.IsBarSticky);
                Assert.IsNull(_appBarViewModel.BarItems);
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
                vm1.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                presenter.Expect("PresentView", new List<object>() { view1 }, 0);

                navigationService.NavigateToView("foo", null);

                view1.Expect("Dismissing", new List<object>() { }, 0);
                vm1.Expect("Dismissing", new List<object>() { }, 0);
                vm2.Expect("Presenting", new List<object>() { navigationService, null }, 0);
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
                vm.Expect("Presenting", new List<object>() { navigationService, null }, 0);
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

                NavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                navigationService.Extensions = new NavigationExtensionList();

                factory.Expect("CreateView", new List<object>() { "foo", null }, view1);
                vmItems.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                view1.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                presenter.Expect("PresentView", new List<object>() { view1 }, 0);

                navigationService.NavigateToView("foo", null);

                factory.Expect("CreateView", new List<object>() { "bar", null }, view2);
                view2.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                viewModel.Expect("Presenting", new List<object>() { navigationService, null }, 0);
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

                NavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                navigationService.Extensions = new NavigationExtensionList()
                    {
                        new ApplicationBarExtension() { ViewModel = _appBarViewModel }
                    };

                factory.Expect("CreateView", new List<object>() { "foo", null }, view1);
                vmItems.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                view1.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                presenter.Expect("PresentView", new List<object>() { view1 }, 0);

                navigationService.NavigateToView("foo", null);

                factory.Expect("CreateView", new List<object>() { "bar", null }, view2);
                view2.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                viewModel.Expect("Presenting", new List<object>() { navigationService, null }, 0);
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

                NavigationService navigationService = new NavigationService() { Presenter = presenter, ViewFactory = factory };
                navigationService.Extensions = new NavigationExtensionList()
                    {
                        new ApplicationBarExtension() { ViewModel = _appBarViewModel }
                    };

                factory.Expect("CreateView", new List<object>() { "foo", null }, view1);
                vmItems.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                view1.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                presenter.Expect("PresentView", new List<object>() { view1 }, 0);

                navigationService.NavigateToView("foo", null);
                Assert.IsNotNull(vmItems.ApplicationBarSite);
                vmItems.ApplicationBarSite.ShowBar.Execute(null);
                Assert.IsTrue(_appBarViewModel.IsBarVisible);

                factory.Expect("CreateView", new List<object>() { "bar", null }, view2);
                view2.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                viewModel.Expect("Presenting", new List<object>() { navigationService, null }, 0);
                presenter.Expect("PushModalView", new List<object>() { view2 }, 0);
                navigationService.PushModalView("bar", null);

                Assert.IsFalse(_appBarViewModel.IsBarVisible);
                Assert.IsFalse(_appBarViewModel.IsShowBarButtonVisible);
                Assert.IsFalse(_appBarViewModel.ShowBar.CanExecute(null));
            }
        }
    }
}
