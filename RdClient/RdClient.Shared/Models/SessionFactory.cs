namespace RdClient.Shared.Models
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Telemetry;
    using System.Diagnostics.Contracts;

    public sealed class SessionFactory : ISessionFactory
    {
        private readonly IRdpConnectionSource _connectionSource;
        private readonly IDeferredExecution _deferredExecution;
        private readonly ITimerFactory _timerFactory;
        private readonly IDeviceCapabilities _deviceCapabilities;
        private readonly ITelemetryClient _telemetryClient;

        public SessionFactory(IRdpConnectionSource connectionSource, IDeferredExecution deferredExecution, ITimerFactory timerFactory,
            IDeviceCapabilities deviceCapabilities,
            ITelemetryClient telemetryClient)
        {
            Contract.Assert(null != connectionSource);
            Contract.Assert(null != deferredExecution);
            Contract.Assert(null != timerFactory);
            Contract.Assert(null != deviceCapabilities);
            Contract.Assert(null != telemetryClient);

            _connectionSource = connectionSource;
            _deferredExecution = deferredExecution;
            _timerFactory = timerFactory;
            _deviceCapabilities = deviceCapabilities;
            _telemetryClient = telemetryClient;
        }

        IRemoteSession ISessionFactory.CreateSession(RemoteSessionSetup sessionSetup)
        {
            Contract.Assert(null != _deferredExecution, "SessionFactory.CreateSession|Cannot create session without a deferred execution object");
            Contract.Ensures(null != Contract.Result<IRemoteSession>());

            return new RemoteSession(sessionSetup, _deferredExecution, _connectionSource, _timerFactory, _deviceCapabilities, _telemetryClient);
        }

    }
}
