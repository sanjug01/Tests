namespace RdClient.Models
{
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    sealed class SuccessfulLogicFactory : ImitationRdpConnectionSource.ISimulationLogicFactory
    {
        ImitationRdpConnectionSource.ISimulationLogic ImitationRdpConnectionSource.ISimulationLogicFactory.Create(ImitationRdpConnectionSource.ISimulatedConnection connection)
        {
            return new Logic(connection);
        }

        private sealed class Logic : DisposableObject, ImitationRdpConnectionSource.ISimulationLogic
        {
            private readonly ImitationRdpConnectionSource.ISimulatedConnection _connection;
            private readonly ReaderWriterLockSlim _monitor;
            private readonly ManualResetEventSlim _disconnect;
            private Task _task;

            public Logic(ImitationRdpConnectionSource.ISimulatedConnection connection)
            {
                Contract.Assert(null != connection);
                _connection = connection;
                _monitor = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
                _disconnect = new ManualResetEventSlim();
            }

            protected override void DisposeManagedState()
            {
                _disconnect.Dispose();
            }

            void ImitationRdpConnectionSource.ISimulationLogic.Connect(CredentialsModel credentials, bool savedCredentials)
            {
                Contract.Assert(null == _task);

                using (ReadWriteMonitor.Write(_monitor))
                {
                    _task = new Task(async delegate
                    {
                        await Task.Delay(250);
                        _connection.EmitConnected();
                        _disconnect.Wait();
                        _connection.EmitDisconnected(new RdpDisconnectReason(RdpDisconnectCode.UserInitiated, 0, 0));
                        using (ReadWriteMonitor.Write(_monitor))
                            _task = null;
                    }, TaskCreationOptions.LongRunning);
                    _task.Start();
                }
            }

            void ImitationRdpConnectionSource.ISimulationLogic.SetCredentials(CredentialsModel credentials, bool savedCredentials)
            {
                throw new NotImplementedException();
            }

            void ImitationRdpConnectionSource.ISimulationLogic.Disconnect()
            {
                Contract.Assert(null != _task);

                Task.Run(async delegate
                {
                    await Task.Delay(100);
                    _disconnect.Set();
                });
            }

            void ImitationRdpConnectionSource.ISimulationLogic.HandleAsyncDisconnect(Shared.CxWrappers.Errors.RdpDisconnectReason reason, bool reconnect)
            {
                throw new NotImplementedException();
            }
        }
    }
}
