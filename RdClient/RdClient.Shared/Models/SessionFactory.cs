namespace RdClient.Shared.Models
{
    using System;
    using System.Diagnostics.Contracts;

    public sealed class SessionFactory : ISessionFactory
    {
        private Helpers.IDeferredExecution _deferredExecution;

        Helpers.IDeferredExecution ISessionFactory.DeferedExecution
        {
            get { return _deferredExecution; }
            set { _deferredExecution = value; }
        }

        IRemoteSession ISessionFactory.CreateSession(RemoteSessionSetup sessionSetup)
        {
            Contract.Assert(null != _deferredExecution, "SessionFactory.CreateSession|Cannot create session without a deferred execution object");
            Contract.Ensures(null != Contract.Result<IRemoteSession>());
            return new RemoteSession(sessionSetup);
        }
    }
}
