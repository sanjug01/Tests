using RdClient.Shared.Navigation;
using RdClient.Shared.Models;
using System;
using System.Diagnostics.Contracts;
using System.Windows.Input;
using RdClient.Shared.Helpers;
using RdClient.Shared.CxWrappers;

namespace RdClient.Shared.ViewModels
{


    public class SessionViewModel : ViewModelBase, IViewModelDisconnectString
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
                args.RdpConnection.Events.ClientDisconnected += HandleUnexpectedDisconnect;
            };

            SessionModel.Connect(_connectionInformation);
        }        

        private void HandleUnexpectedDisconnect(object sender, ClientDisconnectedArgs args)
        {
            IRdpConnection rdpConnection = sender as IRdpConnection;
            rdpConnection.Events.ClientDisconnected -= HandleUnexpectedDisconnect;

            RdpDisconnectReason reason = args.DisconnectReason;
            string errorString = DisconnectString.GetDisconnectString(reason);
            DialogMessageArgs dialogArgs = new DialogMessageArgs(errorString, () => {
            }, null);
            this.NavigationService.PushModalView("DialogMessage", dialogArgs);
        }

        private void Disconnect(object o)
        {
            SessionModel.Disconnect();

            NavigationService.NavigateToView("ConnectionCenterView", null);            
        }
    }
}
