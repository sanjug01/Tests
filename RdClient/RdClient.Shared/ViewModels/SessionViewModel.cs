using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using System;
using System.Diagnostics.Contracts;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public class SessionViewModel : ViewModelBase
    {
        private ConnectionInformation _connectionInformation;

        private readonly ICommand _disconnectCommand;
        public ICommand DisconnectCommand { get { return _disconnectCommand; } }

        private readonly ICommand _connectCommand;
        public ICommand ConnectCommand { get { return _connectCommand; } }

        public ISessionModel SessionModel { get; set; }
        public DisconnectString DisconnectString { get; set; }

        public SessionViewModel()
        {
            _disconnectCommand = new RelayCommand(new Action<object>(Disconnect));
            _connectCommand = new RelayCommand(new Action<object>(Connect));
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Requires(null != activationParameter as ConnectionInformation);
            _connectionInformation = activationParameter as ConnectionInformation;
        }


        private void Connect(object o)
        {            
            Contract.Assert(null != _connectionInformation);
            Contract.Assert(null != SessionModel);

            SessionModel.ConnectionCreated += (sender, args) => {
                args.RdpConnection.Events.ClientDisconnected += HandleDisconnected;
            };

            SessionModel.Connect(_connectionInformation);
        }        

        private void HandleDisconnected(object sender, ClientDisconnectedArgs args)
        {
            IRdpConnection rdpConnection = sender as IRdpConnection;
            rdpConnection.Events.ClientDisconnected -= HandleDisconnected;

            RdpDisconnectReason reason = args.DisconnectReason;

            if(reason.Code != RdpDisconnectCode.UserInitiated)
            {
                ErrorMessageArgs dialogArgs = new ErrorMessageArgs(reason, () =>
                {
                }, null);
                this.NavigationService.PushModalView("DialogMessage", dialogArgs);
            }

            NavigationService.NavigateToView("ConnectionCenterView", null);            
        }

        private void Disconnect(object o)
        {
            SessionModel.Disconnect();
        }
    }
}
