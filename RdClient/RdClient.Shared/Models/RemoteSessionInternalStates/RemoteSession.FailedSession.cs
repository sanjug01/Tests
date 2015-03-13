namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;

    partial class RemoteSession
    {
        private sealed class FailedSession : InternalState
        {
            private readonly IRdpConnection _connection;
            private readonly RdpDisconnectReason _reason;

            public override void Activate(RemoteSession session)
            {
                //
                // Set the session state to Failed
                //
                _connection.Cleanup();
                session.DeferEmitFailed(_reason.Code);
            }

            public FailedSession(IRdpConnection connection, RdpDisconnectReason reason, InternalState otherState)
                : base(SessionState.Failed, otherState)
            {
                _connection = connection;
                _reason = reason;
            }
        }
    }
}
