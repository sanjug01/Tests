namespace RdClient.Shared.Test.ViewModels
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ViewModels;
    using System;
    using System.Windows.Input;

    [TestClass]
    public sealed class AccessoryViewModelBaseTests
    {
        private sealed class TestViewModel : AccessoryViewModelBase
        {
            public sealed class Completion : CompletionBase
            {
                public EventHandler<object> OnCompletedCalled;

                protected override void OnCompleted(object result)
                {
                    if (null != this.OnCompletedCalled)
                        this.OnCompletedCalled(this, result);
                }
            }
        }


        private sealed class TestNavigationService : INavigationService
        {
            ICommand INavigationService.BackCommand
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            NavigationExtensionList INavigationService.Extensions
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            IViewPresenter INavigationService.Presenter
            {
                set
                {
                    throw new NotImplementedException();
                }
            }

            IPresentableViewFactory INavigationService.ViewFactory
            {
                set
                {
                    throw new NotImplementedException();
                }
            }

            event EventHandler INavigationService.DismissingLastModalView
            {
                add
                {
                    throw new NotImplementedException();
                }

                remove
                {
                    throw new NotImplementedException();
                }
            }

            event EventHandler INavigationService.PushingFirstModalView
            {
                add
                {
                    throw new NotImplementedException();
                }

                remove
                {
                    throw new NotImplementedException();
                }
            }

            void INavigationService.DismissAccessoryView(IPresentableView accessoryView)
            {
                throw new NotImplementedException();
            }

            void INavigationService.DismissModalView(IPresentableView modalView)
            {
                throw new NotImplementedException();
            }

            void INavigationService.NavigateToView(string viewName, object activationParameter)
            {
                throw new NotImplementedException();
            }

            void INavigationService.PushAccessoryView(string viewName, object activationParameter, IPresentationCompletion presentationCompletion)
            {
                throw new NotImplementedException();
            }

            void INavigationService.PushModalView(string viewName, object activationParameter, IPresentationCompletion presentationCompletion)
            {
                throw new NotImplementedException();
            }
        }

        private sealed class TestPresentationContext : IStackedPresentationContext
        {
            public event EventHandler<object> DismissCalled;

            void IStackedPresentationContext.Dismiss(object result)
            {
                if (null != this.DismissCalled)
                    this.DismissCalled(this, result);
            }
        }

        [TestMethod]
        public void AccessoryViewModel_PresentComplete_DismissesSelf()
        {
            TestViewModel vm = new TestViewModel();
            IViewModel ivm = vm;
            SynchronousCompletion completion = new SynchronousCompletion();
            INavigationService nav = new TestNavigationService();
            TestPresentationContext context = new TestPresentationContext();

            int dismissedCalls = 0;

            context.DismissCalled += (sender, e) =>
            {
                ++dismissedCalls;
                Assert.IsNull(e);
            };

            ivm.Presenting(nav, completion, context);
            completion.Complete();

            Assert.AreEqual(1, dismissedCalls);
        }

        [TestMethod]
        public void AccessoryViewModelCompletion_CompletedNoParameter_Cancelled()
        {
            TestViewModel.Completion completion = new TestViewModel.Completion();
            IPresentationCompletion icompletion = completion;

            int cancelledCalls = 0, completedCalls = 0;

            completion.Cancelled += (sender, e) =>
            {
                Assert.AreEqual(1, completedCalls);
                ++cancelledCalls;
            };

            completion.Completed += (sender, e) =>
            {
                Assert.AreEqual(0, cancelledCalls);
                ++completedCalls;
            };

            completion.OnCompletedCalled += (sender, e) =>
            {
                Assert.Fail();
            };

            icompletion.Completed(null, null);

            Assert.AreEqual(1, completedCalls);
            Assert.AreEqual(1, cancelledCalls);
        }

        [TestMethod]
        public void AccessoryViewModelCompletion_CompletedParameter_Completed()
        {
            TestViewModel.Completion completion = new TestViewModel.Completion();
            IPresentationCompletion icompletion = completion;

            int completedCalls = 0, onCompletedCalls = 0;

            completion.Cancelled += (sender, e) =>
            {
                Assert.Fail();
            };

            completion.Completed += (sender, e) =>
            {
                Assert.AreEqual(0, onCompletedCalls);
                ++completedCalls;
            };

            completion.OnCompletedCalled += (sender, e) =>
            {
                ++onCompletedCalls;
                Assert.AreEqual(1, e);
            };

            icompletion.Completed(null, 1);

            Assert.AreEqual(1, completedCalls);
            Assert.AreEqual(1, onCompletedCalls);
        }
    }
}
