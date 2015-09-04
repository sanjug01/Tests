// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RdClient.Views
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ViewModels;
    using System;
    using Windows.UI.Core;
    using Windows.UI.ViewManagement;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;

    public sealed partial class RemoteSessionView : UserControl, IPresentableView, IInputFocusController
    {
        private readonly InputPane _inputPane;
        private readonly CoreWindow _coreWindow;

        public RemoteSessionView()
        {
            this.InitializeComponent();

            _inputPane = InputPane.GetForCurrentView();
            _coreWindow = CoreWindow.GetForCurrentThread();

            _inputPane.Showing += this.OnInputPaneShowing;
            _inputPane.Hiding += this.OnInputPaneHiding;
            _coreWindow.Activated += this.OnCoreWindowActivated;

            this.SizeChanged += OnSizeChanged;
        }
        /// <summary>
        /// Property for binding with x:Bind in XAML.
        /// </summary>
        public IInputFocusController InputFocusController
        {
            get { return this; }
        }

        //
        // IPresentableView interface
        //
        IViewModel IPresentableView.ViewModel
        {
            get { return (IViewModel)this.DataContext; }
        }

        void IPresentableView.Activating(object activationParameter)
        {
        }

        void IPresentableView.Presenting(INavigationService navigationService, object activationParameter)
        {
        }

        void IPresentableView.Dismissing()
        {
        }

        void IInputFocusController.SetDefault()
        {
            this.RemoteSessionPanel.Focus(FocusState.Programmatic);
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Tab:
                case Windows.System.VirtualKey.Enter:
                    e.Handled = true;
                    break;

                default:
                    base.OnKeyDown(e);
                    break;
            }
        }

        protected override void OnKeyUp(KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Tab:
                case Windows.System.VirtualKey.Enter:
                    e.Handled = true;
                    break;

                default:
                    base.OnKeyUp(e);
                    break;
            }
        }

        private void OnSizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            IScrollBarModel sbModel = ((RemoteSessionViewModel)DataContext).ScrollBarModel;

            sbModel.HorizontalScrollBarWidth = this.HorizontalScrolbar.ActualHeight;
            sbModel.VerticalScrollBarWidth = this.VerticalScrolbar.ActualWidth;

            this.SessionPanel.Width = this.SessionPanelContainer.ActualWidth;
            this.SessionPanel.Height = this.SessionPanelContainer.ActualHeight - _inputPane.OccludedRect.Height;
        }

        private void OnInputPaneShowing(InputPane sender, InputPaneVisibilityEventArgs e)
        {
            this.SessionPanel.Height = Math.Min(this.SessionPanel.Height - e.OccludedRect.Height, 0);
        }

        private void OnInputPaneHiding(InputPane sender, InputPaneVisibilityEventArgs e)
        {
            this.SessionPanel.Height = this.SessionPanelContainer.ActualHeight;
        }

        private void OnCoreWindowActivated(CoreWindow sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            switch(e.WindowActivationState)
            {
                case CoreWindowActivationState.Deactivated:
                    this.SessionPanel.Height = this.SessionPanelContainer.ActualHeight;
                    break;

                default:
                    this.SessionPanel.Height = this.SessionPanelContainer.ActualHeight - _inputPane.OccludedRect.Height;
                    break;
            }
        }
    }
}
