using RdClient.Navigation;
using RdClient.Shared.ViewModels;
using System.Diagnostics.Contracts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RdClient.Views
{
    public sealed partial class ConnectionCenter : Page, IPresentableView
    {
        private INavigationService _navigationService;

        public ConnectionCenter()
        {
            this.InitializeComponent();
        }
        
        public void Activating(object activationParameter)
        {
            
        }

        public void Presenting(INavigationService navigationService, object activationParameter)
        {
            Contract.Requires(navigationService != null);

            _navigationService = navigationService;
        }

        public void Dismissing()
        {

        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateToView("SessionView", null);
        }
    }
}
