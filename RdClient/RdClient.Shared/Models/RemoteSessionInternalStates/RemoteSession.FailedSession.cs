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

            protected override void Activated()
            {
                //
                // Set the session state to Failed
                //
                _connection.Cleanup();
                this.TelemetryClient.Event(string.Format("ConnectionFailure:{0}",_reason.Code));
                this.Session.DeferEmitFailed(_reason.Code);
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
