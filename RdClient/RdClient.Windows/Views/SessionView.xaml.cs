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
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RdClient.Views
{
    public sealed partial class SessionView : Page, IPresentableView
    {
        private INavigationService _navigationService;
        private IPhysicalScreenSize _screenSize;
        private object _activationParameter;

        private object _layoutSync = new object();

        public SessionView()
        {
            this.InitializeComponent();

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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Contract.Assert(_activationParameter != null);
            SessionViewModel svm = (SessionViewModel)this.Resources["SessionViewModel"];

            Tuple<Desktop, Credentials> connectionInformation = _activationParameter as Tuple<Desktop, Credentials>;

            IRdpConnection connection = RdpConnectionFactory.CreateInstance(CoreWindow.GetForCurrentThread(), this.SwapChainPanel);
            RdpPropertyApplier.ApplyScreenSize(connection as IRdpProperties, _screenSize);

            svm.NavigationService = _navigationService;
            svm.RdpConnection = connection;
            svm.ConnectCommand.Execute(connectionInformation);
        }
    }
}
