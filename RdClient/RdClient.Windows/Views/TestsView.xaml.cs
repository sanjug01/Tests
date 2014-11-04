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
    public sealed partial class TestsView : Page, IPresentableView
    {
        private INavigationService _navigationService;
        private IPhysicalScreenSize _screenSize;
        private object _activationParameter;

        private object _layoutSync = new object();

        public TestsView()
        {
            this.InitializeComponent();

            _screenSize = new PhysicalScreenSize();
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
            TestsViewModel tvm = (TestsViewModel)this.Resources["TestsViewModel"];

            ConnectionInformation connectionInformation = _activationParameter as ConnectionInformation;

            RdpConnectionFactory factory = new RdpConnectionFactory();
            factory.SwapChainPanel = this.TestSwapChainPanel;

            tvm.NavigationService = _navigationService;
            tvm.RdpConnectionFactory = factory;
            tvm.ConnectionInformation = connectionInformation;
        }
    }
}
