namespace RdClient.Models
{
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    sealed class RejectSavedPasswordLogicFactory : ImitationRdpConnectionSource
    {
        protected override Shared.CxWrappers.IRdpConnection CreateConnection(IRenderingPanel renderingPanel)
        {
            return new Logic(renderingPanel);
        }

        private sealed class Logic : ImitationRdpConnectionSource.Connection
        {
            private readonly CancellationTokenSource _cts;
            private readonly ReaderWriterLockSlim _monitor;
            private Task _task;
            private bool _savedCredentials;

            public Logic(IRenderingPanel renderingPanel) : base(renderingPanel)
            {
                _cts = new CancellationTokenSource();
                _monitor = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            }

            protected override void DisposeManagedState()
            {
                _monitor.Dispose();
                _cts.Dispose();
            }

            protected override void Connect(CredentialsModel credentials, bool savedCredentials)
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
                                this.EmitAsyncDisconnect(new RdpDisconnectReason(RdpDisconnectCode.FreshCredsRequired, 0, 0));
                            }
                            catch(OperationCanceledException)
                            {
                                this.EmitDisconnected(new RdpDisconnectReason(RdpDisconnectCode.UserInitiated, 0, 0));
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
                                this.EmitConnected();

                                _cts.Token.WaitHandle.WaitOne();
                                this.EmitDisconnected(new RdpDisconnectReason(RdpDisconnectCode.UserInitiated, 0, 0));
                            }
                            catch(OperationCanceledException)
                            {
                                this.EmitDisconnected(new RdpDisconnectReason(RdpDisconnectCode.UserInitiated, 0, 0));
                            }

                            using (ReadWriteMonitor.Write(_monitor))
                                _task = null;
                        }, _cts.Token, TaskCreationOptions.LongRunning);
                    }

                    _task.Start();
                }
            }

            protected override void SetCredentials(CredentialsModel credentials, bool savedCredentials)
            {
                //
                // Simply remember if the last credentials were saved save the new credentials;
                //
                using(ReadWriteMonitor.Write(_monitor))
                {
                    _savedCredentials = savedCredentials;
                }
            }

            protected override void Disconnect()
            {
                Contract.Assert(null != _task);

                Task.Run(async delegate
                {
                    await Task.Delay(100);
                    _cts.Cancel();
                });
            }

            protected override void HandleAsyncDisconnectResult(RdpDisconnectReason reason, bool reconnect)
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
                                    this.EmitAsyncDisconnect(new RdpDisconnectReason(RdpDisconnectCode.FreshCredsRequired, 0, 0));
                                }
                                catch (OperationCanceledException)
                                {
                                    this.EmitDisconnected(new RdpDisconnectReason(RdpDisconnectCode.UserInitiated, 0, 0));
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
                                    this.EmitConnected();

                                    _cts.Token.WaitHandle.WaitOne();
                                    this.EmitDisconnected(new RdpDisconnectReason(RdpDisconnectCode.UserInitiated, 0, 0));
                                }
                                catch (OperationCanceledException)
                                {
                                    this.EmitDisconnected(new RdpDisconnectReason(RdpDisconnectCode.UserInitiated, 0, 0));
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
