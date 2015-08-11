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
                this.Session.DeferEmitFailed(_reason.Code);
                this.SessionLaunch.userInitiated = false;
                this.SessionLaunch.disconnectReason = (int)_reason.Code;
                this.SessionLaunch.disconnectCode = _reason.ULegacyCode.ToString("X8");
                this.SessionLaunch.disconnectExtendedCode = _reason.ULegacyExtendedCode.ToString("X8");
                this.SessionLaunch.success = this.Session._hasConnected;
                this.TelemetryClient.ReportEvent(this.SessionLaunch);

                if (this.Session._hasConnected)
                {
                    this.SessionDuration.Stop();
                    this.TelemetryClient.ReportEvent(this.SessionDuration);
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
