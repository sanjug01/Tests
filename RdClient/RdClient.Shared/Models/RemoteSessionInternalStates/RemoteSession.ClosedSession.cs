namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClientCx;
    using System.Diagnostics.Contracts;

    partial class RemoteSession
    {
        private sealed class ClosedSession : InternalState
        {
            private readonly IRdpConnection _connection;

            protected override void Activated()
            {
                _connection.Cleanup();
                this.Session.DeferEmitClosed();

                this.SessionLaunch.userInitiated = true;
                this.SessionLaunch.disconnectReason = (int)RdpDisconnectCode.UserInitiated;
                this.SessionLaunch.success = this.Session._hasConnected;
                this.TelemetryClient.ReportEvent(this.SessionLaunch);

                if (this.Session._hasConnected)
                {
                    this.SessionDuration.Stop();
                    this.TelemetryClient.ReportEvent(this.SessionDuration);
                }

                ChangeState(new InactiveSession(this));
            }

            public ClosedSession(IRdpConnection connection, InternalState otherState) : base(SessionState.Closed, otherState)
            {
                Contract.Assert(null != connection);
                _connection = connection;
            }
        }
    }
}
