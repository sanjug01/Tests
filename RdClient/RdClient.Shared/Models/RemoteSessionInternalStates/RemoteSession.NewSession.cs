namespace RdClient.Shared.Models
{
    using RdClient.Shared.Telemetry;
    using System.Diagnostics.Contracts;
    using System.Threading;

    partial class RemoteSession
    {
        private sealed class NewSession : InternalState
        {
            private readonly RemoteSessionSetup _sessionSetup;
            private RemoteSession _session;

            public NewSession(RemoteSessionSetup sessionSetup, ReaderWriterLockSlim _monitor, ITelemetryClient telemetryClient)
                : base(SessionState.Idle, _monitor, telemetryClient)
            {
                _sessionSetup = sessionSetup;
            }

            public override void Activate(RemoteSession session)
            {
                Contract.Assert(null == _session);
                Contract.Assert(null != _sessionSetup.SessionCredentials);

                _session = session;
                _session.InternalStartSession(_sessionSetup);
            }

            public override void Deactivate(RemoteSession session)
            {
                Contract.Assert(null == _session);
            }

            public override void Complete(RemoteSession session)
            {
                Contract.Assert(object.ReferenceEquals(_session, session));
                _session = null;
            }
        }
    }
}
