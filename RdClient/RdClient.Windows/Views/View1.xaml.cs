using RdClient.Navigation;
using RdClient.Shared.Models;
using System;
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
            Tuple<Desktop, Credentials> connectionInformation = new Tuple<Desktop, Credentials>(
                new Desktop() { hostName = "a3-w81" },
                new Credentials() { username = "tslabadmin", domain = "", password = "1234AbCd" });

            _navigationService.NavigateToView("SessionView", connectionInformation);
        }

        private void TestsButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // test connection settings passed from main view.
            Tuple<Desktop, Credentials> testConnectionInformation = new Tuple<Desktop, Credentials>(
                new Desktop() { hostName = "a3-w81" },
                new Credentials() { username = "tslabadmin", domain = "", password = "1234AbCd" });

            _navigationService.NavigateToView("TestsView", testConnectionInformation);
        }
    }
}
