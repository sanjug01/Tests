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
using System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RdClient.Views
{
    public sealed partial class SessionView : Page, IPresentableView
    {
        private SessionViewModel _sessionViewModel;
        private INavigationService _navigationService;
        private IRdpConnection _connection;
        private IPhysicalScreenSize _screenSize;
        private object _activationParameter;

        private object _layoutSync = new object();

        public SessionView()
        {
            this.InitializeComponent();

            _sessionViewModel = new SessionViewModel();
            _screenSize = new PhysicalScreenSize();

            this.SizeChanged += SessionView_SizeChanged;
        }

        void SessionView_SizeChanged(object sender, SizeChangedEventArgs e)
        {            
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

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _connection.Disconnect();

            _navigationService.NavigateToView("view1", null);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Contract.Assert(_activationParameter != null);

            Tuple<Desktop, Credentials> connectionInformation = _activationParameter as Tuple<Desktop, Credentials>;
            Desktop desktop = connectionInformation.Item1;
            Credentials credentials = connectionInformation.Item2;

            _connection = RdpConnectionFactory.CreateInstance(CoreWindow.GetForCurrentThread(), this.SwapChainPanel, _sessionViewModel);
            RdpPropertyApplier.ApplyDesktop(_connection as IRdpProperties, desktop);
            RdpPropertyApplier.ApplyScreenSize(_connection as IRdpProperties, _screenSize);
            _connection.Connect(credentials, false);
        }
    }
}
