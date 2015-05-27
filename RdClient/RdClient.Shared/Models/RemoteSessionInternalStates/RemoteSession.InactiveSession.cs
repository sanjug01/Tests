namespace RdClient.Shared.Models
{
    using RdClient.Shared.Telemetry;
    using System.Threading;

    partial class RemoteSession
    {
        private sealed class InactiveSession : InternalState
        {
            public override void Activate(RemoteSession session)
            {
            }

            public InactiveSession(ReaderWriterLockSlim _monitor, ITelemetryClient telemetryClient)
                : base(SessionState.Idle, _monitor, telemetryClient)
            {
            }

            public InactiveSession(InternalState otherState) : base(SessionState.Idle, otherState)
            {
            }
        }
    }
}
