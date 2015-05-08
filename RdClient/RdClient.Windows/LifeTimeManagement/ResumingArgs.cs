namespace RdClient.LifeTimeManagement
{
    using RdClient.Shared.LifeTimeManagement;
    using System;
    using Windows.ApplicationModel;

    public class ResumingArgs : IResumingArgs
    {
        public class ResumingOperationWrapper : IResumingOperationWrapper
        {
            private object _resumeParameters;
            public object Parameters { get { return _resumeParameters; } }

            public ResumingOperationWrapper(object o)
            {
                _resumeParameters = o;
            }
        }

        private IResumingOperationWrapper _resumingOperation;

        public IResumingOperationWrapper ResumingOperation
        { get { return _resumingOperation; } }

        public ResumingArgs(IResumingOperationWrapper resumingOperation)
        {
            _resumingOperation = resumingOperation;
        }
    }
}

