using RdClient.Navigation;
using RdClient.Shared.Navigation;
using System;
using System.Diagnostics.Contracts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RdClient
{
    public sealed partial class MainPage : Page, IViewPresenter
    {
        private PresentableViewFactory<PresentableViewConstructor> _viewFactory;
        private INavigationService _navigationService;

        public MainPage()
        {
            this.InitializeComponent();

            _viewFactory = new PresentableViewFactory<PresentableViewConstructor>();
            _navigationService = new NavigationService(this, _viewFactory);

            _navigationService.PushingFirstModalView += OnAddingFirstModalView;
            _navigationService.DismissingLastModalView += OnRemovedLastModalView;

            _viewFactory.AddViewClass("view1", typeof(Views.View1));
            _viewFactory.AddViewClass("SessionView", typeof(Views.SessionView));
            _viewFactory.AddViewClass("TestsView", typeof(Views.TestsView));
            _viewFactory.AddViewClass("AddOrEditDesktopView", typeof(Views.AddOrEditDesktopView));

            _viewFactory.AddViewClass("Dialog1", typeof(Views.Dialog1));

            _navigationService.NavigateToView("view1", null);
        }

        public void PresentView(IPresentableView view)
        {
            Contract.Requires(view != null);

            this.TransitionAnimationContainer.ShowContent(view as UIElement);
        }

        public void PushModalView(IPresentableView view)
        {
            Contract.Requires(view != null);

            this.ModalStackContainer.Push(view as UIElement);
        }

        public void DismissModalView(IPresentableView view)
        {
            Contract.Requires(view != null);

            this.ModalStackContainer.Pop();
        }

        private void OnAddingFirstModalView(object sender, EventArgs e)
        {
            this.TransitionAnimationContainer.IsEnabled = false;
            this.ModalStackContainer.Visibility = Visibility.Visible;
        }

        private void OnRemovedLastModalView(object sender, EventArgs e)
        {
            this.ModalStackContainer.Visibility = Visibility.Collapsed;
            this.TransitionAnimationContainer.IsEnabled = true;
        }
    }
}
