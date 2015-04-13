// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RdClient.Controls
{
    using RdClient.Shared.Navigation;
    using System;
    using System.Diagnostics.Contracts;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class AccessoryViewPresenter : UserControl, IStackedViewPresenter
    {
        public readonly DependencyProperty AccessoryWidthProperty = DependencyProperty.Register("AccessoryWidth",
            typeof(double), typeof(AccessoryViewPresenter),
            new PropertyMetadata(0.0, OnAccessoryWidthChanged));

        public readonly DependencyProperty FullScreenWidthProperty = DependencyProperty.Register("FullScreenWidth",
            typeof(double), typeof(AccessoryViewPresenter),
            new PropertyMetadata(0.0));

        private enum VisualStateName
        {
            SidePanel,
            FullScreen
        }

        public AccessoryViewPresenter()
        {
            this.InitializeComponent();
            this.SizeChanged += this.OnSizeChanged;
        }

        public double AccessoryWidth
        {
            get { return (double)GetValue(AccessoryWidthProperty); }
            set { SetValue(AccessoryWidthProperty, value); }
        }

        public double FullScreenWidth
        {
            get { return (double)GetValue(FullScreenWidthProperty); }
            set { SetValue(FullScreenWidthProperty, value); }
        }

        void IStackedViewPresenter.PushView(IPresentableView view, bool animated)
        {
            Contract.Assert(view is UIElement);
            this.AccessoriesContainer.Push((UIElement)view, animated);
        }

        void IStackedViewPresenter.DismissView(IPresentableView view, bool animated)
        {
            this.AccessoriesContainer.Pop((UIElement)view, animated);
        }

        private static void OnAccessoryWidthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            AccessoryViewPresenter avp = sender as AccessoryViewPresenter;

            if(null != avp)
            {
                avp.InternalOnAccessoryWidthChanged(e);
            }
        }

        private void InternalOnAccessoryWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            this.AccessoryColumn.Width = new GridLength((double)e.NewValue);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            VisualStateName visualState = VisualStateName.SidePanel;
            double fullScreenWidth = this.FullScreenWidth;

            if (0.0 == fullScreenWidth || e.NewSize.Width <= fullScreenWidth)
                visualState = VisualStateName.FullScreen;

            VisualStateManager.GoToState(this, visualState.ToString(), true);
        }
    }
}
