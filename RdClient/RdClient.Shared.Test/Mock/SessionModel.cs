using RdClient.Shared.CxWrappers;
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

        public void Connect(ConnectionInformation connectionInformation)
        {
            Invoke(new object[] { connectionInformation });
        }

        public void Disconnect()
        {
            Invoke(new object[] { });
        }
    }
}
