namespace RdClient.Shared.Models
{
    using System;

    public sealed class InterruptedSessionContinuation
    {
        private readonly Action _cancelDelegate;

        public InterruptedSessionContinuation(Action cancelDelegate)
        {
            _cancelDelegate = cancelDelegate;
        }

        public void Cancel()
        {
            _cancelDelegate();
        }
    }
}
