namespace RdClient.Shared.Models
{
    using System.Threading;

    partial class RemoteSession
    {
        private sealed class InactiveSession : InternalState
        {
            public override void Activate(RemoteSession session)
            {
            }

            public InactiveSession(ReaderWriterLockSlim _monitor) : base(SessionState.Idle, _monitor)
            {
            }

            public InactiveSession(InternalState otherState) : base(SessionState.Idle, otherState)
            {
            }
        }
    }
}
