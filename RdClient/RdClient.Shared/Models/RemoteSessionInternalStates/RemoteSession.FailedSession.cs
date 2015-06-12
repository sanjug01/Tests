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
                this.SessionTelemetry.AddMetric("userInitiated", 0.0);
                this.SessionTelemetry.AddMetric("disconnectReason", (double)_reason.Code);
                this.SessionTelemetry.AddTag("disconnectCode", _reason.ULegacyCode.ToString("X8"));
                this.SessionTelemetry.AddTag("disconnectExtendedCode", _reason.ULegacyExtendedCode.ToString("X8"));
                this.SessionTelemetry.AddMetric("success", this.Session._hasConnected ? 1.0 : 0.0);
                this.SessionTelemetry.Report();

                if (this.Session._hasConnected)
                {
                    this.SessionDuration.PauseStopwatch(SessionDurationStopwatchName);
                    this.SessionDuration.Report();
                }
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
