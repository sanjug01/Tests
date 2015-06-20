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
                this.SessionTelemetry.AddMetric("userInitiated", 1.0);
                this.SessionTelemetry.AddMetric("disconnectReason", (double)RdpDisconnectCode.UserInitiated);
                this.SessionTelemetry.AddMetric("success", this.Session._hasConnected ? 1.0 : 0.0);
                this.SessionTelemetry.Report();

                if (this.Session._hasConnected)
                {
                    this.SessionDuration.PauseStopwatch(SessionDurationStopwatchName);
                    this.SessionDuration.Report();
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
