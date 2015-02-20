namespace RdClient.Shared.Models
{
    using RdClient.Shared.ViewModels;
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Arguments object for the CredentialsNeeded event emitted by IRemoteSession.
    /// </summary>
    public sealed class CredentialsNeededEventArgs : EventArgs
    {
        private readonly IEditCredentialsTask _task;

        public IEditCredentialsTask Task
        {
            get { return _task; }
        }

        public CredentialsNeededEventArgs(IEditCredentialsTask task)
        {
            Contract.Assert(null != task);
            _task = task;
        }
    }
}
