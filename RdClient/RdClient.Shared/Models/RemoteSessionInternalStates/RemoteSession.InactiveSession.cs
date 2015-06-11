namespace RdClient.Shared.Models
{
    using RdClient.Shared.Telemetry;
    using System.Threading;

    partial class RemoteSession
    {
        private sealed class InactiveSession : InternalState
        {
            protected override void Activated()
            {
            }

            public InactiveSession(ReaderWriterLockSlim _monitor, ITelemetryClient telemetryClient, ITelemetryEvent sessionTelemetry)
                : base(SessionState.Idle, _monitor, telemetryClient, sessionTelemetry)
            {
            }

            public InactiveSession(InternalState otherState) : base(SessionState.Idle, otherState)
            {
            }
        }
    }
}
