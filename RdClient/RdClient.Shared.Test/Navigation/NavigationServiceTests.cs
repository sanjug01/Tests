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
        }

        [TestCleanup]
        public void TearDownTest()
        {
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
                presenter.Expect("PushView", new List<object>() { view1, true }, 0);

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
                presenter.Expect("PushView", new List<object>() { view1, true }, 0);

                navigationService.PushModalView("foo", activationParameter);

                factory.Expect("CreateView", new List<object>() { "bar", activationParameter }, view2);
                view2.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PushView", new List<object>() { view2, true }, 0);

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
                presenter.Expect("PushView", new List<object>() { view1, true }, 0);

                navigationService.PushModalView("foo", activationParameter);

                factory.Expect("CreateView", new List<object>() { "bar", activationParameter }, view2);
                view2.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PushView", new List<object>() { view2, true }, 0);

                navigationService.PushModalView("bar", activationParameter);

                view2.Expect("Dismissing", new List<object>() { }, 0);
                presenter.Expect("DismissView", new List<object>() { view2, true }, 0);
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
                presenter.Expect("PushView", new List<object>() { view1, true }, 0);

                navigationService.PushModalView("foo", activationParameter, null);

                viewModel.Expect("Dismissing", new List<object>() { }, 0);
                view1.Expect("Dismissing", new List<object>() {  }, 0);
                presenter.Expect("DismissView", new List<object>() { view1, true }, 0);

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
                presenter.Expect("PushView", new List<object>() { view1, true }, 0);

                navigationService.PushModalView("foo", activationParameter);

                view1.Expect("Dismissing", new List<object>() { }, 0);
                presenter.Expect("DismissView", new List<object>() { view1, true }, 0);

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
                presenter.Expect("PushView", new List<object>() { view1, true }, 0);

                navigationService.PushModalView("foo", activationParameter);

                view1.Expect("Dismissing", new List<object>() { }, 0);
                presenter.Expect("DismissView", new List<object>() { view1, true }, 0);

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
                presenter.Expect("PushView", new List<object>() { view1, true }, 0);

                factory.Expect("CreateView", new List<object>() { "bar", activationParameter }, view2);
                view2.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PushView", new List<object>() { view2, true }, 0);

                factory.Expect("CreateView", new List<object>() { "narf", activationParameter }, view3);
                view3.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PushView", new List<object>() { view3, true }, 0);

                navigationService.PushModalView("foo", activationParameter);
                navigationService.PushModalView("bar", activationParameter);
                navigationService.PushModalView("narf", activationParameter);

                view3.Expect("Dismissing", new List<object>() { }, 0);
                presenter.Expect("DismissView", new List<object>() { view3, true }, 0);
                
                view2.Expect("Dismissing", new List<object>() { }, 0);
                presenter.Expect("DismissView", new List<object>() { view2, true }, 0);

                view1.Expect("Dismissing", new List<object>() { }, 0);
                presenter.Expect("DismissView", new List<object>() { view1, true }, 0);

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
                presenter.Expect("PushView", new List<object>() { view1, true }, 0);

                navigationService.PushModalView("foo", activationParameter);

                view1.Expect("Dismissing", new List<object>() { }, 0);
                presenter.Expect("DismissView", new List<object>() { view1, true }, 0);

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
                presenter.Expect("PushView", new List<object>() { view1, true }, 0);

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
                presenter.Expect("PushView", new List<object>() { modalView, true }, 0);
                navigationService.PushModalView("bar", null, completion);


                modalView.Expect("Dismissing", new List<object>() { }, 0);
                presenter.Expect("DismissView", new List<object>() { modalView, true }, 0);
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
                presenter.Expect("PushView", null);
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
                presenter.Expect("PushView", null);                
                view.Expect("Presenting", null);
                vm.Expect("Presenting", null);
                nav.PushModalView("view", null);

                //navigate back without vm handling it and verify view is dismissed
                vm.Expect("NavigatingBack", new List<object>() { backArgs }, 0);                
                presenter.Expect("DismissView", new List<object>() { view, true }, 0 );
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
                presenter.Expect("PushView", null);
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

        [TestMethod]
        public void NoAccessoryPresenter_PushAccessoryView_PushedModalView()
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
                presenter.Expect("PushView", new List<object>() { view1, true }, 0);

                navigationService.PushAccessoryView("foo", activationParameter);

                Assert.IsTrue(callbackCalled);
            }
        }

        [TestMethod]
        public void NoAccessoryPresenter_DismissAccessoryView_DismissedModalView()
        {
            using (Mock.PresentableView view1 = new Mock.PresentableView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            using (Mock.ViewModel viewModel = new Mock.ViewModel())
            {
                view1.ViewModel = viewModel;

                INavigationService navigationService = new NavigationService()
                {
                    Presenter = presenter,
                    ViewFactory = factory,
                    Extensions = new NavigationExtensionList()
                };

                bool callbackCalled = false;
                object activationParameter = new object();

                navigationService.DismissingLastModalView += (sender, args) => { callbackCalled = true; };

                factory.Expect("CreateView", new List<object>() { "foo", activationParameter }, view1);
                viewModel.Expect("Presenting", new List<object>() { navigationService, activationParameter, null }, 0);
                view1.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PushView", new List<object>() { view1, true }, 0);

                navigationService.PushAccessoryView("foo", activationParameter);

                viewModel.Expect("Dismissing", new List<object>() { }, 0);
                view1.Expect("Dismissing", new List<object>() { }, 0);
                presenter.Expect("DismissView", new List<object>() { view1, true }, 0);

                navigationService.DismissAccessoryView(view1);

                Assert.IsTrue(callbackCalled);
            }
        }

        [TestMethod]
        public void DismissAccessoryViewsCommandDismissesAccessoryViews()
        {
            using (Mock.PresentableView view1 = new Mock.PresentableView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            using (Mock.ViewModel viewModel = new Mock.ViewModel())
            {
                view1.ViewModel = viewModel;

                INavigationService navigationService = new NavigationService()
                {
                    Presenter = presenter,
                    ViewFactory = factory,
                    Extensions = new NavigationExtensionList()                    
                };

                object activationParameter = new object();
                factory.Expect("CreateView", new List<object>() { "foo", activationParameter }, view1);
                viewModel.Expect("Presenting", new List<object>() { navigationService, activationParameter, null }, 0);
                view1.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PushView", new List<object>() { view1, true }, 0);
                navigationService.PushAccessoryView("foo", activationParameter);                

                viewModel.Expect("Dismissing", new List<object>() { }, 0);
                view1.Expect("Dismissing", new List<object>() { }, 0);
                presenter.Expect("DismissView", new List<object>() { view1, true }, 0);               
                navigationService.DismissAccessoryViewsCommand.Execute(null);
            }
        }

        [TestMethod]
        public void HaveAccessoryPresenter_Back_DismissedAccessory()
        {
            using (Mock.PresentableViewWithStackedPresenter view = new Mock.PresentableViewWithStackedPresenter())
            using (Mock.PresentableView accessory = new Mock.PresentableStackedView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            using (Mock.ViewModel viewModel = new Mock.ViewModel())
            using (Mock.ViewModel accessoryViewModel = new Mock.ViewModel())
            {
                view.ViewModel = viewModel;
                accessory.ViewModel = accessoryViewModel;

                INavigationService navigationService = new NavigationService()
                {
                    Presenter = presenter,
                    ViewFactory = factory,
                    Extensions = new NavigationExtensionList()
                };
                object activationParameter = new object();
                IBackCommandArgs backArgs = new BackCommandArgs();

                factory.Expect("CreateView", new List<object>() { "foo", activationParameter }, view);
                viewModel.Expect("Presenting", new List<object>() { navigationService, activationParameter, null }, 0);
                view.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                presenter.Expect("PresentView", new List<object>() { view }, 0);
                factory.Expect("CreateView", new List<object>() { "bar", activationParameter }, accessory);
                accessoryViewModel.Expect("Presenting", new List<object>() { navigationService, activationParameter, null }, 0);
                accessory.Expect("Presenting", new List<object>() { navigationService, activationParameter }, 0);
                view.Expect("PushView", new List<object>() { accessory, true }, 0);
                accessory.Expect("Activate", null);
                accessoryViewModel.Expect("NavigatingBack", new List<object>() { backArgs }, 0);
                accessory.Expect("Deactivate", null);
                accessory.Expect("Dismissing", null);
                view.Expect("DismissView", new List<object>() { accessory, true }, 0);
                accessoryViewModel.Expect("Dismissing", null);

                navigationService.NavigateToView("foo", activationParameter);
                navigationService.PushAccessoryView("bar", activationParameter);
                navigationService.BackCommand.Execute(backArgs);
            }
        }

        [TestMethod]
        public void DismissAccessoryViewsCommandDoesNotCrashWhenThereAreNoAccessoryViews()
        {
            using (Mock.PresentableView view1 = new Mock.PresentableView())
            using (Mock.ViewFactory factory = new Mock.ViewFactory())
            using (Mock.ViewPresenter presenter = new Mock.ViewPresenter())
            using (Mock.ViewModel viewModel = new Mock.ViewModel())
            {
                view1.ViewModel = viewModel;

                INavigationService navigationService = new NavigationService()
                {
                    Presenter = presenter,
                    ViewFactory = factory,
                    Extensions = new NavigationExtensionList()
                };

                navigationService.DismissAccessoryViewsCommand.Execute(null);
            }
        }
    }
}
