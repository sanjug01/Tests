using RdClient.Navigation;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Utils;
using RdClient.Shared.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public class SessionViewModel : ViewModelBase
    {
        public ICommand DisconnectCommand { get; private set; }
        public ICommand ConnectCommand { get; private set; }
        public IRdpConnection RdpConnection { private get; set; }
        public INavigationService NavigationService { private get; set; }

        public SessionViewModel()
        {
            DisconnectCommand = new RelayCommand(new Action<object>(Disconnect));
            ConnectCommand = new RelayCommand(new Action<object>(Connect));
        }

        private void Connect(object o)
        {
            Tuple<Desktop, Credentials> connectionInformation = o as Tuple<Desktop, Credentials>;
            Desktop desktop = connectionInformation.Item1;
            Credentials credentials = connectionInformation.Item2;

            RdpPropertyApplier.ApplyDesktop(RdpConnection as IRdpProperties, desktop);
            RdpConnection.Connect(credentials, credentials.haveBeenPersisted);
        }

        private void Disconnect(object o)
        {
            RdpConnection.Disconnect();
            NavigationService.NavigateToView("view1", null);
        }
    }
}
