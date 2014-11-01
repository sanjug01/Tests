using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using RdMock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Test.Mock
{
    class RdpConnection : MockBase, IRdpConnection
    {
        IRdpEvents _events;

        public RdpConnection(IRdpEvents events)
        {
            _events = events;
        }

        public void Connect(Credentials credentials, bool fUsingSavedCreds)
        {
            Invoke(new object[] { credentials, fUsingSavedCreds });
        }

        public void Disconnect()
        {
            Invoke(new object[] {});
        }

        public void Suspend()
        {
            Invoke(new object[] { });
        }

        public void Resume()
        {
            Invoke(new object[] { });
        }

        public void TerminateInstance()
        {
            Invoke(new object[] { });
        }

        public void HandleAsyncDisconnectResult(RdpDisconnectReason disconnectReason, bool reconnectToServer)
        {
            Invoke(new object[] { disconnectReason, reconnectToServer });
        }

        public IRdpEvents Events
        {
            get { return _events; }
        }
    }
}
