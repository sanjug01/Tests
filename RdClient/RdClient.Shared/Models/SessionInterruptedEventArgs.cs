namespace RdClient.Shared.Models
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Arguments of the IRemoteSession.Interrupted event emitted when the connection has been interrupted
    /// and the session has began to re-establish the connection.
    /// One subscriber may obtain a continuation token from the arguments object and use it to terminate the
    /// re-connect process.
    /// </summary>
    public sealed class SessionInterruptedEventArgs : EventArgs
    {
        private readonly Action _cancelDelegate;
        private bool _continuationObtained;

        public SessionInterruptedEventArgs(Action cancelDelegate)
        {
            Contract.Assert(null != cancelDelegate);
            Contract.Ensures(null != _cancelDelegate);

            _cancelDelegate = cancelDelegate;
            _continuationObtained = false;
        }

        public bool IsContinuationObtained
        {
            get { return _continuationObtained; }
        }

        public InterruptedSessionContinuation ObtainContinuation()
        {
            if (_continuationObtained)
                throw new InvalidOperationException("Continuation has been obtained already");

            _continuationObtained = true;

            return new InterruptedSessionContinuation(_cancelDelegate);
        }
    }
}
