using RdClient.Shared.Navigation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RdClient.Views
{
    public sealed partial class CertificateValidationView : Page, IPresentableView
    {
        public CertificateValidationView()
        {
            this.InitializeComponent();
        }

        IViewModel IPresentableView.ViewModel
        {
            get { return this.DataContext as IViewModel; }
        }

        void IPresentableView.Activating(object activationParameter) { }

        void IPresentableView.Presenting(INavigationService navigationService, object activationParameter) { }

        void IPresentableView.Dismissing() { }

    }
}
