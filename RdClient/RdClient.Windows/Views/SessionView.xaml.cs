using RdClient.CxWrappers.Utils;
using RdClient.Navigation;
using RdClient.Shared.Models;
using RdClient.Shared.ViewModels;
using System.Diagnostics.Contracts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RdClient.Views
{
    public sealed partial class SessionView : Page, IPresentableView
    {
        private ConnectionInformation _connectionInformation;

        public IViewModel ViewModel { get { return this.SessionViewModel; } }

        public SessionView()
        {
            this.InitializeComponent();
        }

        public void Activating(object activationParameter)
        {            
        }

        public void Presenting(INavigationService navigationService, object activationParameter)
        {
            Contract.Requires(null != activationParameter as ConnectionInformation);
            _connectionInformation = activationParameter as ConnectionInformation;
        }

        public void Dismissing()
        {
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            RdpConnectionFactory factory = new RdpConnectionFactory();
            factory.SwapChainPanel = this.SwapChainPanel;

            this.SessionViewModel.RdpConnectionFactory = factory;
            this.SessionViewModel.ConnectCommand.Execute(_connectionInformation);
        }
    }
}
