using RdClient.Navigation;
using System;
using System.Diagnostics.Contracts;
using Windows.UI.Xaml.Controls;

namespace RdClient.Views
{
    public sealed partial class View2 : Page, IPresentableView
    {
        private INavigationService _navigationService;

        public View2()
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
            _navigationService.NavigateToView("view1", null);
        }

        private void DisconnectButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // should disconnect and switch ConnectionCenter view
            _navigationService.NavigateToView("view1", null);
        }

    }
}
