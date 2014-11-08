using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Navigation;
using System.Collections.Generic;

namespace RdClient.Shared.Test
{
    [TestClass]
    public class NavigationServiceTests
    {
        [TestMethod]
        public void NavigateToViewNoViewModel_ShouldSucceed()
        {
            using(Mock.PresentableView view1 = new Mock.PresentableView())
            using(Mock.PresentableView view2 = new Mock.PresentableView())
            using(Mock.ViewFactory factory = new Mock.ViewFactory())
            using(Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            {
                NavigationService navigationService = new NavigationService(presenter, factory);
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
                NavigationService navigationService = new NavigationService(presenter, factory);
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
                NavigationService navigationService = new NavigationService(presenter, factory);
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
                NavigationService navigationService = new NavigationService(presenter, factory);
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
                NavigationService navigationService = new NavigationService(presenter, factory);
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
                NavigationService navigationService = new NavigationService(presenter, factory);
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

                NavigationService navigationService = new NavigationService(presenter, factory);
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
                NavigationService navigationService = new NavigationService(presenter, factory);
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
                NavigationService navigationService = new NavigationService(presenter, factory);
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
                NavigationService navigationService = new NavigationService(presenter, factory);
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
                NavigationService navigationService = new NavigationService(presenter, factory);
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
                NavigationService navigationService = new NavigationService(presenter, factory);
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
                NavigationService navigationService = new NavigationService(presenter, factory);
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
                NavigationService navigationService = new NavigationService(presenter, factory);
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
    }
}
