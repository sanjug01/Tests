namespace RdClient.Shared.Models
{
    using RdClient.Shared.Helpers;
    using System;
    using System.Diagnostics.Contracts;

    public sealed class SessionFactory : ISessionFactory
    {
        private readonly IRdpConnectionSource _connectionSource;
        private readonly IDeferredExecution _deferredExecution;

        public SessionFactory(IRdpConnectionSource connectionSource, IDeferredExecution deferredExecution)
        {
            _connectionSource = connectionSource;
            _deferredExecution = deferredExecution;
        }

        IRemoteSession ISessionFactory.CreateSession(RemoteSessionSetup sessionSetup)
        {
            Contract.Assert(null != _deferredExecution, "SessionFactory.CreateSession|Cannot create session without a deferred execution object");
            Contract.Ensures(null != Contract.Result<IRemoteSession>());
            return new RemoteSession(sessionSetup, _deferredExecution, _connectionSource);
        }
    }
}
