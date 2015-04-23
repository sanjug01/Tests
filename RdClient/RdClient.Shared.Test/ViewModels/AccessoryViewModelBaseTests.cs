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

        [TestMethod]
        public void AccessoryViewModel_PresentComplete_DismissesSelf()
        {
        }
    }
}
