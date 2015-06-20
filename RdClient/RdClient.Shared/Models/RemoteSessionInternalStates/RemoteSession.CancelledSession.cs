namespace RdClient.Shared.Models
{
    partial class RemoteSession
    {
        private sealed class CancelledSession : InternalState
        {
            protected override void Activated()
            {
                this.Session.EmitClosed();
                this.Session.InternalSetState(new InactiveSession(this));
            }

            public CancelledSession(InternalState otherState) : base(SessionState.Closed, otherState)
            {
            }
        }
    }
}
