namespace RdClient.Models
{
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    sealed class RejectSavedPasswordLogicFactory : ImitationRdpConnectionSource.ISimulationLogicFactory
    {
        ImitationRdpConnectionSource.ISimulationLogic ImitationRdpConnectionSource.ISimulationLogicFactory.Create(ImitationRdpConnectionSource.ISimulatedConnection connection)
        {
            return new Logic(connection);
        }

        private sealed class Logic : DisposableObject, ImitationRdpConnectionSource.ISimulationLogic
        {
            private readonly ImitationRdpConnectionSource.ISimulatedConnection _connection;
            private readonly CancellationTokenSource _cts;
            private readonly ReaderWriterLockSlim _monitor;
            private Task _task;
            private bool _savedCredentials;

            public Logic(ImitationRdpConnectionSource.ISimulatedConnection connection)
            {
                Contract.Assert(null != connection);
                _connection = connection;
                _cts = new CancellationTokenSource();
                _monitor = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            }

            protected override void DisposeManagedState()
            {
                _monitor.Dispose();
                _cts.Dispose();
            }

            void ImitationRdpConnectionSource.ISimulationLogic.Connect(CredentialsModel credentials, bool savedCredentials)
            {
                Contract.Assert(null == _task);
                //
                // If savedCredentials is true, emit an async disconnect failure;
                // otherwise, connect successfully.
                //

                using (ReadWriteMonitor.Write(_monitor))
                {
                    _savedCredentials = savedCredentials;

                    if(savedCredentials)
                    {
                        _task = new Task(async delegate
                        {
                            try
                            {
                                await Task.Delay(300, _cts.Token);
                                _connection.EmitAsyncDisconnect(new RdpDisconnectReason(RdpDisconnectCode.FreshCredsRequired, 0, 0));
                            }
                            catch(OperationCanceledException)
                            {
                                _connection.EmitDisconnected(new RdpDisconnectReason(RdpDisconnectCode.UserInitiated, 0, 0));
                            }
                        });
                    }
                    else
                    {
                        _task = new Task(async delegate
                        {
                            try
                            {
                                await Task.Delay(250, _cts.Token);
                                _connection.EmitConnected();

                                _cts.Token.WaitHandle.WaitOne();
                                _connection.EmitDisconnected(new RdpDisconnectReason(RdpDisconnectCode.UserInitiated, 0, 0));
                            }
                            catch(OperationCanceledException)
                            {
                                _connection.EmitDisconnected(new RdpDisconnectReason(RdpDisconnectCode.UserInitiated, 0, 0));
                            }

                            using (ReadWriteMonitor.Write(_monitor))
                                _task = null;
                        }, _cts.Token, TaskCreationOptions.LongRunning);
                    }

                    _task.Start();
                }
            }

            void ImitationRdpConnectionSource.ISimulationLogic.SetCredentials(CredentialsModel credentials, bool savedCredentials)
            {
                //
                // Simply remember if the last credentials were saved save the new credentials;
                //
                using(ReadWriteMonitor.Write(_monitor))
                {
                    _savedCredentials = savedCredentials;
                }
            }

            void ImitationRdpConnectionSource.ISimulationLogic.Disconnect()
            {
                Contract.Assert(null != _task);

                Task.Run(async delegate
                {
                    await Task.Delay(100);
                    _cts.Cancel();
                });
            }

            void ImitationRdpConnectionSource.ISimulationLogic.HandleAsyncDisconnect(Shared.CxWrappers.Errors.RdpDisconnectReason reason, bool reconnect)
            {
                if (RdpDisconnectCode.FreshCredsRequired == reason.Code)
                {
                    Task t;

                    using (ReadWriteMonitor.Read(_monitor))
                        t = _task;

                    if (null != t)
                        t.Wait();

                    using(ReadWriteMonitor.Write(_monitor))
                    {
                        if(_savedCredentials)
                        {
                            _task = new Task(async delegate
                            {
                                try
                                {
                                    await Task.Delay(300, _cts.Token);
                                    _connection.EmitAsyncDisconnect(new RdpDisconnectReason(RdpDisconnectCode.FreshCredsRequired, 0, 0));
                                }
                                catch (OperationCanceledException)
                                {
                                    _connection.EmitDisconnected(new RdpDisconnectReason(RdpDisconnectCode.UserInitiated, 0, 0));
                                }
                            });
                        }
                        else
                        {
                            _task = new Task(async delegate
                            {
                                try
                                {
                                    await Task.Delay(250, _cts.Token);
                                    _connection.EmitConnected();

                                    _cts.Token.WaitHandle.WaitOne();
                                    _connection.EmitDisconnected(new RdpDisconnectReason(RdpDisconnectCode.UserInitiated, 0, 0));
                                }
                                catch (OperationCanceledException)
                                {
                                    _connection.EmitDisconnected(new RdpDisconnectReason(RdpDisconnectCode.UserInitiated, 0, 0));
                                }

                                using (ReadWriteMonitor.Write(_monitor))
                                    _task = null;
                            }, _cts.Token, TaskCreationOptions.LongRunning);
                        }

                        _task.Start();
                    }
                }
            }
        }
    }
}
