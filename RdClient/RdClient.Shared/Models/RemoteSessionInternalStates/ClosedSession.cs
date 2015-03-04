namespace RdClient.Shared.Models
{
    partial class RemoteSession
    {
        private sealed class ClosedSession : InternalState
        {
            public override void Activate(RemoteSession session)
            {
                session.EmitClosed();
                session.InternalSetState(new InactiveSession(this));
            }

            public ClosedSession(InternalState otherState) : base(SessionState.Closed, otherState)
            {
            }
        }
    }
}
