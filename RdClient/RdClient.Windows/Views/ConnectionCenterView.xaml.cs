namespace RdClient.Views
{
    using System;
    using RdClient.Shared.Navigation;
    using Windows.Foundation;
    using Windows.Graphics.Display;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

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

        private readonly IList<IPresentableView> _accessoryViews;
        private IStackedViewPresenter _accessoryPresenter;
        private INavigationService _nav;

        public ConnectionCenterView()
        {
            this.InitializeComponent();
            _accessoryViews = new List<IPresentableView>();
            _accessoryPresenter = null;
            this.SizeChanged += this.OnSizeChanged;
            this.VisualStates.CurrentStateChanging += this.OnVisualStateChanging;
            this.VisualStates.CurrentStateChanged += this.OnVisualStateChanged;
        }

        IViewModel IPresentableView.ViewModel { get { return this.DataContext as IViewModel; } }
        void IPresentableView.Activating(object activationParameter) { }
        void IPresentableView.Presenting(INavigationService navigationService, object activationParameter) { _nav = navigationService; }
        void IPresentableView.Dismissing() { }

        void IStackedViewPresenter.PushView(IPresentableView view, bool animated)
        {
            Contract.Assert(null != view);
            Contract.Assert(null != _accessoryPresenter);

            _accessoryPresenter.PushView(view, animated);
            _accessoryViews.Add(view);

            if (1 == _accessoryViews.Count)
            {
                Contract.Assert(_accessoryPresenter is UIElement);
                ((UIElement)_accessoryPresenter).Visibility = Visibility.Visible;
            }
        }

        void IStackedViewPresenter.DismissView(IPresentableView view, bool animated)
        {
            _accessoryPresenter.DismissView(view, animated);
            _accessoryViews.Remove(view);

            if (0 == _accessoryViews.Count)
            {
                Contract.Assert(_accessoryPresenter is UIElement);
                ((UIElement)_accessoryPresenter).Visibility = Visibility.Collapsed;
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Layout layout = GetNewLayout(e.NewSize, DisplayInformation.GetForCurrentView());

            VisualStateManager.GoToState(this, layout.ToString(), true);
        }

        private void OnVisualStateChanging(object sender, VisualStateChangedEventArgs e)
        {
        }

        private void OnVisualStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            IStackedViewPresenter newPresenter = FindAccessoryPresenter(this);
            Contract.Assert(null != newPresenter);

            if(!object.ReferenceEquals(_accessoryPresenter, newPresenter))
            {
                //
                // Remove all accessory views from the old presenter and push them onto the new one.
                //
                foreach (IPresentableView view in _accessoryViews)
                {
                    _accessoryPresenter.DismissView(view, false);
                    newPresenter.PushView(view, false);
                }

                if (_accessoryViews.Count != 0)
                    ((UIElement)newPresenter).Visibility = Visibility.Visible;
                if (null != _accessoryPresenter)
                    ((UIElement)_accessoryPresenter).Visibility = Visibility.Collapsed;
                _accessoryPresenter = newPresenter;
            }
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

        private static IStackedViewPresenter FindAccessoryPresenter(DependencyObject root)
        {
            IStackedViewPresenter presenter = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(root);

            for(int i = 0; null == presenter && i < childrenCount; ++i)
            {
                DependencyObject child = VisualTreeHelper.GetChild(root, i);

                presenter = child as IStackedViewPresenter;

                if (null == presenter)
                    presenter = FindAccessoryPresenter(child);
            }

            return presenter;
        }
    }
}
