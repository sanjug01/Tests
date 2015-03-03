using RdClient.Shared.CxWrappers.Errors;
namespace RdClient.Shared.Models
{
    partial class RemoteSession
    {
        private sealed class FailedSession : InternalState
        {
            private readonly RdpDisconnectReason _reason;

            public override void Activate(RemoteSession session)
            {
                //
                // Set the session state to Failed
                //
                //session._state.SetDisconnectCode(_reason.Code);
                //session._state.SetState(SessionState.Failed);
                session.DeferEmitFailed(_reason.Code);
            }

            public FailedSession(RdpDisconnectReason reason, InternalState otherState)
                : base(SessionState.Failed, otherState)
            {
                _reason = reason;
            }
        }
    }
}
