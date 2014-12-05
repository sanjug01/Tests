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

        public IViewModel ViewModel
        {
            get { return this.DataContext as IViewModel; }
        }

        public void Activating(object activationParameter) { }

        public void Presenting(INavigationService navigationService, object activationParameter) { }

        public void Dismissing() { }

    }
}
