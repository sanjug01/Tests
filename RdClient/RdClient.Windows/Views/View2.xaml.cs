using RdClient.CxWrappers.Utils;
using RdClient.Navigation;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Utils;
using RdClient.Shared.Models;
using System;
using System.Diagnostics.Contracts;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace RdClient.Views
{
    public sealed partial class View2 : Page, IPresentableView
    {
        private INavigationService _navigationService;
        private IRdpConnection _connection;

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
            Contract.Requires(activationParameter != null);

            _navigationService = navigationService;

            Tuple<Desktop, Credentials> connectionInformation = activationParameter as Tuple<Desktop, Credentials>;
            Desktop desktop = connectionInformation.Item1;
            Credentials credentials = connectionInformation.Item2;

            _connection = RdpConnectionFactory.CreateInstance(CoreWindow.GetForCurrentThread(), this.SwapChainPanel);
            RdpPropertyApplier.ApplyRdpProperties(_connection as IRdpProperties, desktop);
            _connection.Connect(credentials, false);
        }

        public void Dismissing()
        {
            _connection.Disconnect();
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _navigationService.NavigateToView("view1", null);
        }
    }
}
