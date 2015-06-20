namespace RdClient.Views
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Navigation;
    using Windows.System;
    using Windows.UI.Core;
    using Windows.UI.Xaml.Controls;

    public sealed partial class CertificateValidationView : Page, IPresentableView
    {
        public CertificateValidationView()
        {
            this.InitializeComponent();
        }

        IViewModel IPresentableView.ViewModel
        {
            get { return (IViewModel)this.DataContext; }
        }

        void IPresentableView.Activating(object activationParameter) { }

        void IPresentableView.Presenting(INavigationService navigationService, object activationParameter)
        {
            Dispatcher.AcceleratorKeyActivated += OnAcceleratorKeyActivated;
        }

        void IPresentableView.Dismissing()
        {
            Dispatcher.AcceleratorKeyActivated -= this.OnAcceleratorKeyActivated;
        }

        private void OnAcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs e)
        {
            if (CoreAcceleratorKeyEventType.KeyDown == e.EventType)
            {
                switch (e.VirtualKey)
                {
                    // Esc invokes the Cancel action.
                    case VirtualKey.Escape:
                        this.CancelButton.Invoke(e);
                        break;
                    // Enter should be ignored, an explicit user action is required for validation
                    case VirtualKey.Enter:
                        break;
                }
            }
        }
    }
}
