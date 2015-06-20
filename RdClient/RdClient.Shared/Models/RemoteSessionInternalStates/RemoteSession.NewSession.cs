namespace RdClient.Shared.Models
{
    using System.Diagnostics.Contracts;

    partial class RemoteSession
    {
        private sealed class NewSession : InternalState
        {
            private readonly RemoteSessionSetup _sessionSetup;

            public NewSession(RemoteSessionSetup sessionSetup, InternalState otherState)
                : base(SessionState.Idle, otherState)
            {
                _sessionSetup = sessionSetup;
            }

            protected override void Activated()
            {
                Contract.Assert(null != _sessionSetup.SessionCredentials);
                this.Session.InternalStartSession(_sessionSetup);
            }
        }
    }
}
