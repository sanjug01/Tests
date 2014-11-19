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

            this.NavigationService.PushingFirstModalView += OnAddingFirstModalView;
            this.NavigationService.DismissingLastModalView += OnRemovedLastModalView;
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.ViewFactory.AddViewClass("ConnectionCenterView", typeof(Views.View1));
            this.ViewFactory.AddViewClass("SessionView", typeof(Views.SessionView));
            this.ViewFactory.AddViewClass("TestsView", typeof(Views.TestsView));
            this.ViewFactory.AddViewClass("AddOrEditDesktopView", typeof(Views.AddOrEditDesktopView));
            this.ViewFactory.AddViewClass("DialogMessage", typeof(Views.DialogMessage));

            this.NavigationService.Presenter = this;
            this.NavigationService.AppBarViewModel = this.DataContext as IApplicationBarViewModel;

            this.NavigationService.NavigateToView("view1", null);
            this.NavigationService.PushModalView("DialogMessage", new DialogMessageArgs("this is a message", () => { }, () => { }));
        }
    }
}
