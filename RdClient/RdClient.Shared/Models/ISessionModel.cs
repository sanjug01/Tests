using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using System;
namespace RdClient.Shared.Models
{
    public interface ISessionModel
    {
        event EventHandler<ConnectionCreatedArgs> ConnectionCreated;
        event EventHandler<ConnectionAutoReconnectingArgs> ConnectionAutoReconnecting;
        event EventHandler<ConnectionAutoReconnectCompleteArgs> ConnectionAutoReconnectComplete;

        void Connect(ConnectionInformation connectionInformation, ITimerFactory timerFactory, GeneralSettings settings);
        void Disconnect();
        void AcceptCertificate(IRdpCertificate certificate);
        bool IsCertificateAccepted(IRdpCertificate certificate);
    }
}
