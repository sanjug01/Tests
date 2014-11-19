using RdClient.Navigation;
using RdClient.Shared.Navigation;
using RdClient.Shared.ViewModels;
using System;
using System.Diagnostics.Contracts;
using Windows.Graphics.Display;
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
            //
            // Set initial orientation.
            // TODO: handle orientation based on the actual dimentions of the view.
            //
            DisplayInformation di = DisplayInformation.GetForCurrentView();
            OnOrientationChanged(di, null);
            di.OrientationChanged += OnOrientationChanged;

            _viewFactory = new PresentableViewFactory<PresentableViewConstructor>();
            _navigationService = new NavigationService(this, _viewFactory, this.DataContext as IApplicationBarViewModel);

            _navigationService.PushingFirstModalView += OnAddingFirstModalView;
            _navigationService.DismissingLastModalView += OnRemovedLastModalView;

            _viewFactory.AddViewClass("ConnectionCenterView", typeof(Views.ConnectionCenterView));
            _viewFactory.AddViewClass("view1", typeof(Views.View1));
            _viewFactory.AddViewClass("SessionView", typeof(Views.SessionView));
            _viewFactory.AddViewClass("TestsView", typeof(Views.TestsView));
            _viewFactory.AddViewClass("AddOrEditDesktopView", typeof(Views.AddOrEditDesktopView));
            _viewFactory.AddViewClass("DialogMessage", typeof(Views.DialogMessage));

            _navigationService.NavigateToView("ConnectionCenterView", null);
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

        private void OnOrientationChanged(DisplayInformation sender, object e)
        {
            //
            // If the view model implements ILayoutAwareViewModel, tell it to update the layout for the new orientation.
            // Ignore flipped orientations, care only about portrait and landscape layouts.
            //
            ILayoutAwareViewModel lavm = this.DataContext as ILayoutAwareViewModel;

            if (null != lavm)
            {
                switch (sender.CurrentOrientation)
                {
                    case DisplayOrientations.Portrait:
                    case DisplayOrientations.PortraitFlipped:
                        lavm.OrientationChanged(ViewOrientation.Portrait);
                        break;

                    default:
                        lavm.OrientationChanged(ViewOrientation.Landscape);
                        break;
                }
            }
        }
    }
}
