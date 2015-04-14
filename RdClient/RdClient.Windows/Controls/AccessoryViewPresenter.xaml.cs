// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RdClient.Controls
{
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ViewModels;
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;

    public sealed partial class AccessoryViewPresenter : UserControl, IStackedViewPresenter
    {
        public readonly DependencyProperty AccessoryWidthProperty = DependencyProperty.Register("AccessoryWidth",
            typeof(double), typeof(AccessoryViewPresenter),
            new PropertyMetadata(0.0, OnAccessoryWidthChanged));

        public readonly DependencyProperty FullScreenWidthProperty = DependencyProperty.Register("FullScreenWidth",
            typeof(double), typeof(AccessoryViewPresenter),
            new PropertyMetadata(0.0));

        public readonly DependencyProperty CancellationRequestedProperty = DependencyProperty.Register("CancellationRequested",
            typeof(ICommand), typeof(AccessoryViewPresenter),
            new PropertyMetadata(null));

        private sealed class CommandParameter : IHandleable
        {
            private readonly PointerRoutedEventArgs _e;

            public CommandParameter(PointerRoutedEventArgs e)
            {
                Contract.Assert(null != e);
                _e = e;
            }

            bool IHandleable.Handled
            {
                get { return _e.Handled; }
                set { _e.Handled = value; }
            }
        }

        private enum VisualStateName
        {
            SidePanel,
            FullScreen
        }

        public AccessoryViewPresenter()
        {
            this.InitializeComponent();
            this.SizeChanged += this.OnSizeChanged;
            this.OverlayPanel.PointerPressed += this.OnPointerPressed;
        }

        public double AccessoryWidth
        {
            get { return (double)GetValue(AccessoryWidthProperty); }
            set { SetValue(AccessoryWidthProperty, value); }
        }

        public ICommand CancellationRequested
        {
            get { return (ICommand)GetValue(CancellationRequestedProperty); }
            set { SetValue(CancellationRequestedProperty, value); }
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

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ICommand command = this.CancellationRequested;

            if(null != command)
            {
                IHandleable param = new CommandParameter(e);

                if(command.CanExecute(param))
                {
                    command.Execute(param);
                }
            }
        }
    }
}
