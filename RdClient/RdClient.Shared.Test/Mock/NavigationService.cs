using RdClient.Shared.Helpers;
using RdClient.Shared.Navigation;
using RdMock;
using System;
using System.Windows.Input;

namespace RdClient.Shared.Test.Mock
{
    public class NavigationService : MockBase, INavigationService
    {
#pragma warning disable 67 // warning CS0067: the event <...> is never used
        public event EventHandler PushingFirstModalView;
        public event EventHandler DismissingLastModalView;

        public void NavigateToView(string viewName, object activationParameter)
        {
            Invoke(new object[] { viewName, activationParameter });
        }

        public void PushModalView(string viewName, object activationParameter, IPresentationCompletion presentationCompletion)
        {
            Invoke(new object[] { viewName, activationParameter, presentationCompletion });
        }

        public void DismissModalView(IPresentableView modalView)
        {
            Invoke(new object[] { modalView });
        }

        void INavigationService.PushAccessoryView(string viewName, object activationParameter, IPresentationCompletion presentationCompletion)
        {
            Invoke(new object[] { viewName, activationParameter, presentationCompletion });
        }

        public void DismissAccessoryView(IPresentableView accessoryView)
        {
            Invoke(new object[] { accessoryView });
        }

        public IViewPresenter Presenter
        {
            set {  }
        }

        public IPresentableViewFactory ViewFactory
        {
            set {  }
        }

        public Shared.ViewModels.IApplicationBarViewModel AppBarViewModel
        {
            set {  }
        }

        public NavigationExtensionList Extensions { get; set; }


        public ICommand BackCommand { get; set; }

        public ICommand DismissAccessoryViewsCommand { get; set; }
    }
}
