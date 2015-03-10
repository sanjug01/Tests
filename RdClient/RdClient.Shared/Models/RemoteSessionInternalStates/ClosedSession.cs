namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using System.Diagnostics.Contracts;

    partial class RemoteSession
    {
        private sealed class ClosedSession : InternalState
        {
            private readonly IRdpConnection _connection;

            public override void Activate(RemoteSession session)
            {
                _connection.Cleanup();
                session.EmitClosed();
                session.InternalSetState(new InactiveSession(this));
            }

            public ClosedSession(IRdpConnection connection, InternalState otherState) : base(SessionState.Closed, otherState)
            {
                Contract.Assert(null != connection);

                _connection = connection;
            }
        }
    }
}
