﻿namespace RdClient.Views
{
    using RdClient.Shared.Navigation;
    using System.Diagnostics.Contracts;
    using Windows.Foundation;
    using Windows.Graphics.Display;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class ConnectionCenterView : Page, IPresentableView, IStackedViewPresenter
    {
        //
        // The enum prevents typos in names of visual states. When a new visual state
        // needs to be applied, the name of the state comes from a member of the enum.
        //
        private enum Layout
        {
            DefaultLayout,
            NarrowLayout,
            PhoneLayout
        }

        public ConnectionCenterView()
        {
            this.InitializeComponent();
            this.SizeChanged += this.OnSizeChanged;
            this.VisualStates.CurrentStateChanging += this.OnVisualStateChanging;
        }

        IViewModel IPresentableView.ViewModel { get { return this.DataContext as IViewModel; } }
        void IPresentableView.Activating(object activationParameter) { }
        void IPresentableView.Presenting(INavigationService navigationService, object activationParameter) { }
        void IPresentableView.Dismissing() { }

        void IStackedViewPresenter.PushView(IPresentableView view, bool animated)
        {
            Contract.Assert(null != view);
            this.AccessoryViewPresenter.PushView(view, animated);
        }

        void IStackedViewPresenter.DismissView(IPresentableView view, bool animated)
        {
            this.AccessoryViewPresenter.DismissView(view, animated);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Layout layout = GetNewLayout(e.NewSize, DisplayInformation.GetForCurrentView());

            VisualStateManager.GoToState(this, layout.ToString(), true);
        }

        private Layout GetNewLayout(Size viewSize, DisplayInformation displayInformation)
        {
            Layout layout = Layout.DefaultLayout;

            if (viewSize.Width < 640 * displayInformation.RawPixelsPerViewPixel)
            {
                //
                // The view is too small, switch to the phone layout.
                //
                layout = Layout.PhoneLayout;
            }
            else if (viewSize.Width < viewSize.Height)
            {
                if (viewSize.Height / viewSize.Width >= 15.0 / 9.0)
                {
                    //
                    // The view is much taller than it is wide; switch to the phone layout, unless
                    // the view is wider than 1024 virtual pixels.
                    //
                    if (viewSize.Width < 1024 * displayInformation.RawPixelsPerViewPixel)
                        layout = Layout.PhoneLayout;
                    else
                        layout = Layout.NarrowLayout;
                }
                else if (viewSize.Width < 1080 * displayInformation.RawPixelsPerViewPixel)
                {
                    //
                    // If the view is narrower than 1080 virtual pixels, switch to the narrow layout.
                    //
                    layout = Layout.NarrowLayout;
                }
            }

            return layout;
        }

        private void OnVisualStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            VisualStateManager.GoToState(this.AccessoryViewPresenter, e.NewState.Name, true);
        }
    }
}
