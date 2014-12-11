using RdClient.Shared.CxWrappers;
using System;
namespace RdClient.Shared.Models
{
    public interface ISessionModel
    {
        event EventHandler<ConnectionCreatedArgs> ConnectionCreated;

        void Connect(ConnectionInformation connectionInformation);
        void Disconnect();
        void AcceptCertificate(IRdpCertificate certificate, bool alwaysAccept = false);
        bool IsCertificateAccepted(IRdpCertificate certificate);
    }
}
