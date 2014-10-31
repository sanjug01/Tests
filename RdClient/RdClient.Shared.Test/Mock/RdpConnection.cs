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

        public int HandleAsyncDisconnectResult(RdpDisconnectReason disconnectReason, bool reconnectToServer)
        {
            return (int) Invoke(new object[] { disconnectReason, reconnectToServer });
        }
    }
}
