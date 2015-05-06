namespace RdClient.Shared.LifeTimeManagement
{
    using System;

    public sealed class ResumeEventArgs : EventArgs
    {
        private readonly IResumingArgs _resumingArgs;

        public IResumingArgs SuspendArgs { get { return _resumingArgs; } }

        public ResumeEventArgs(IResumingArgs resumingArgs)
        {
            _resumingArgs = resumingArgs;
        }
    }
}
