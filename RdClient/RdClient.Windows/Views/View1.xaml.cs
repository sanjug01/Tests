using RdClient.Navigation;
using RdClient.Shared.Models;
using System.Diagnostics.Contracts;
using Windows.UI.Xaml.Controls;

namespace RdClient.Views
{
    public sealed partial class View1 : Page, IPresentableView
    {
        private INavigationService _navigationService;

        public View1()
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

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ConnectionInformation connectionInformation = new ConnectionInformation()
            {
                Desktop = new Desktop() { hostName = "a3-w81" },
                Credentials = new Credentials() { username = "tslabadmin", domain = "", password = "1234AbCd", haveBeenPersisted = false }
            };

            _navigationService.NavigateToView("SessionView", connectionInformation);
        }

        private void TestsButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ConnectionInformation connectionInformation = new ConnectionInformation()
            {
                Desktop = new Desktop() { hostName = "a3-w81" },
                Credentials = new Credentials() { username = "tslabadmin", domain = "", password = "1234AbCd", haveBeenPersisted = false }
            };

            _navigationService.NavigateToView("TestsView", connectionInformation);
        }

        private void AddDesktopButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // new dektop
            _navigationService.PushModalView("AddOrEditDesktopView", null);
        }

        private void EditDesktopButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // edit existing desktop
            Desktop d = new Desktop() { hostName = "a3-w81" };
            _navigationService.PushModalView("AddOrEditDesktopView", d);
        }
    }
}
