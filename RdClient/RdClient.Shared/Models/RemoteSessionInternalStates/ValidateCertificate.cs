namespace RdClient.Shared.Models
{
    using System;

    partial class RemoteSession
    {
        private sealed class ValidateCertificate : InternalState
        {
            public override void Activate(RemoteSession session)
            {
                throw new NotImplementedException();
            }

            public override void Complete(RemoteSession session)
            {
            }

            public ValidateCertificate(InternalState otherState)
                : base(SessionState.Idle, otherState)
            {
            }
        }
    }
}
