namespace RdClient.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    sealed class SuccessfulLogicFactory : ImitationRdpConnectionSource
    {

        protected override IRdpConnection CreateConnection(IRenderingPanel renderingPanel)
        {
            throw new NotImplementedException();
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
                    _task = new Task(async delegate
                    {
                        await Task.Delay(250);
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

                Task.Run(async delegate
                {
                    await Task.Delay(100);
                    _disconnect.Set();
                });
            }
        }
    }
}
