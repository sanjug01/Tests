namespace RdClient.Shared.Models
{
    partial class RemoteSession
    {
        private sealed class CancelledSession : InternalState
        {
            public override void Activate(RemoteSession session)
            {
                session.EmitClosed();
                session.InternalSetState(new InactiveSession(this));
            }

            public CancelledSession(InternalState otherState) : base(SessionState.Closed, otherState)
            {
            }
        }
    }
}
