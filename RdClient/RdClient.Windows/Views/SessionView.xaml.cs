using RdClient.CxWrappers.Utils;
using RdClient.Helpers;
using RdClient.Navigation;
using RdClient.Shared;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Utils;
using RdClient.Shared.Models;
using RdClient.Shared.ViewModels;
using System;
using System.Diagnostics.Contracts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RdClient.Views
{
    public sealed partial class SessionView : Page, IPresentableView
    {
        private INavigationService _navigationService;
        private object _activationParameter;

        private object _layoutSync = new object();

        public SessionView()
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
            _activationParameter = activationParameter;
        }

        public void Dismissing()
        {
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Contract.Assert(_activationParameter != null);
            SessionViewModel svm = (SessionViewModel)this.Resources["SessionViewModel"];

            ConnectionInformation connectionInformation = _activationParameter as ConnectionInformation;

            RdpConnectionFactory factory = new RdpConnectionFactory();
            factory.SwapChainPanel = this.SwapChainPanel;

            svm.NavigationService = _navigationService;
            svm.RdpConnectionFactory = factory;
            svm.ConnectCommand.Execute(connectionInformation);
        }
    }
}
