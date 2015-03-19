namespace RdClient.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    sealed class SuccessfulConnectionSource : ImitationRdpConnectionSource
    {
        protected override IRdpConnectionFactory CreateConnectionFactory(IRenderingPanel renderingPanel)
        {
            return new Factory(renderingPanel);
        }

        private sealed class Factory : IRdpConnectionFactory
        {
            private readonly IRenderingPanel _renderingPanel;

            public Factory(IRenderingPanel renderingPanel)
            {
                _renderingPanel = renderingPanel;
            }

            IRdpConnection IRdpConnectionFactory.CreateDesktop(string rdpFile)
            {
                return new Logic(_renderingPanel);
            }

            IRdpConnection IRdpConnectionFactory.CreateApplication(string rdpFile)
            {
                return new Logic(_renderingPanel);
            }
        }

        private sealed class Logic : ImitationRdpConnectionSource.Connection
        {
            private readonly ReaderWriterLockSlim _monitor;
            private readonly ManualResetEventSlim _disconnect;
            private Task _task;

            public Logic(IRenderingPanel renderingPanel) : base(renderingPanel)
            {
                _monitor = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
                _disconnect = new ManualResetEventSlim();
            }

            protected override void DisposeManagedState()
            {
                _disconnect.Dispose();
            }

            protected override void Connect(CredentialsModel credentials, bool savedCredentials)
            {
                Contract.Assert(null == _task);

                using (ReadWriteMonitor.Write(_monitor))
                {
                    _task = new Task(() =>
                    {
                        Task.Delay(250).Wait();
                        this.EmitConnected();
                        _disconnect.Wait();
                        this.EmitDisconnected(new RdpDisconnectReason(RdpDisconnectCode.UserInitiated, 0, 0));
                        using (ReadWriteMonitor.Write(_monitor))
                            _task = null;
                    }, TaskCreationOptions.LongRunning);
                    _task.Start();
                }
            }

            protected override void Disconnect()
            {
                Contract.Assert(null != _task);

                Task.Run(() =>
                {
                    Task.Delay(100).Wait();
                    _disconnect.Set();
                });
            }
        }
    }
}
