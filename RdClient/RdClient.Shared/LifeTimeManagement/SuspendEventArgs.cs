namespace RdClient.Shared.LifeTimeManagement
{
    using System;

    public sealed class SuspendEventArgs : EventArgs
    {
        private readonly ISuspensionArgs _suspensionArgs;

        public ISuspensionArgs SuspendArgs { get { return _suspensionArgs; } }

        public SuspendEventArgs(ISuspensionArgs suspensionArgs)
        {
            _suspensionArgs = suspensionArgs;
        }
    }
}
