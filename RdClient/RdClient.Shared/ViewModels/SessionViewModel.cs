using RdClient.Navigation;
using RdClient.Shared.Models;
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
            SessionModel.Connect(_connectionInformation);
        }

        private void Disconnect(object o)
        {
            SessionModel.Disconnect();

            NavigationService.NavigateToView("ConnectionCenterView", null);            
        }
    }
}
