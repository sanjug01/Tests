using RdClient.Navigation;
using RdClient.Shared.CxWrappers;
using System;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public interface ISessionViewModel
    {
        event EventHandler<ConnectionCreatedArgs> ConnectionCreated;
        ICommand ConnectCommand { get; }
        ICommand DisconnectCommand { get; }
        IRdpConnectionFactory RdpConnectionFactory { set; }
    }
}
