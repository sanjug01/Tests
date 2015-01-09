using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using RdMock;
using System;

namespace RdClient.Shared.Test.Mock
{
    public class SessionModel : MockBase, ISessionModel
    {
#pragma warning disable 67 // warning CS0067: the event <...> is never used
        public event EventHandler<ConnectionCreatedArgs> ConnectionCreated;

        public void EmitConnectionCreated(ConnectionCreatedArgs args)
        {
            ConnectionCreated(this, args);
        }

        public void Connect(ConnectionInformation connectionInformation, ITimerFactory timerFactory, GeneralSettings settings)
        {
            Invoke(new object[] { connectionInformation, timerFactory, settings });
        }

        public void Disconnect()
        {
            Invoke(new object[] { });
        }

        public void AcceptCertificate(IRdpCertificate certificate)
        {
            Invoke(new object[] { certificate });
        }
        public bool IsCertificateAccepted(IRdpCertificate certificate)
        {
            return (bool) Invoke(new object[] { certificate });
        }
    }
}
