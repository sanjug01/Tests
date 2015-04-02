using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.ViewModels;
using System;
using System.Windows.Input;

namespace RdClient.DesignTime
{
    public sealed class FakeGatewayViewModel : IGatewayViewModel
    {
        private GatewayModel _gateway = new GatewayModel() { HostName = "MyPC_1234", CredentialsId = Guid.Empty };

        public GatewayModel Gateway
        {
            get { return _gateway; }
        }

        public ICommand DeleteCommand {get; set; }

        public ICommand EditCommand {get; set; }

        public void Presented(INavigationService navService, ApplicationDataModel dataModel)
        {
            throw new NotImplementedException();
        }

        void IGatewayViewModel.Dismissed()
        {
        }
    }
}
