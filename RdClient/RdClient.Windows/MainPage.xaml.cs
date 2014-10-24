using RdClient.Navigation;
using System;
using System.Diagnostics.Contracts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RdClient
{
    public sealed partial class MainPage : Page, IViewPresenter
    {
        private PresentableViewFactory _viewFactory;
        private INavigationService _navigationService;

        public MainPage()
        {
            this.InitializeComponent();

            _viewFactory = new PresentableViewFactory();
            _navigationService = new NavigationService(this, _viewFactory);

            _viewFactory.AddViewClass("view1", typeof(Views.View1));
            _viewFactory.AddViewClass("SessionView", typeof(Views.SessionView));

            _navigationService.NavigateToView("view1", null);
        }

        public void PresentView(IPresentableView view)
        {
            Contract.Requires(view != null);

            this.TransitionAnimationContainer.ShowContent(view as UIElement);
        }

        public void PushModalView(IPresentableView view)
        {
            throw new NotImplementedException();
        }

        public void DismissModalView(IPresentableView view)
        {
            throw new NotImplementedException();
        }
    }
}
