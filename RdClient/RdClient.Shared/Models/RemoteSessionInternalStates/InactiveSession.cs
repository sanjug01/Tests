using System.Threading;
namespace RdClient.Shared.Models
{
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
        }
    }
}
