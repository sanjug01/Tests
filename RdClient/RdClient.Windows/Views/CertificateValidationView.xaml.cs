namespace RdClient.Views
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Navigation;
    using Windows.System;
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

        private void OnAcceleratorKeyActivated(Windows.UI.Core.CoreDispatcher sender, Windows.UI.Core.AcceleratorKeyEventArgs e)
        {
            switch (e.VirtualKey)
            {
                case VirtualKey.Escape:
                    this.CancelButton.Invoke(e);
                    break;

                case VirtualKey.Enter:
                    this.AcceptButton.Invoke(e);
                    break;
            }
        }
    }
}
