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

    sealed class RejectSavedPasswordConnectionSource : ImitationRdpConnectionSource
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

            IRdpConnection IRdpConnectionFactory.CreateDesktop()
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

                    if (savedCredentials)
                    {
                        _task = new Task(() =>
                        {
                            try
                            {
                                Task.Delay(300, _cts.Token).Wait();
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
                        _task = new Task(() =>
                        {
                            try
                            {
                                Task.Delay(250, _cts.Token).Wait();
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

            protected override void Disconnect()
            {
                Contract.Assert(null != _task);

                Task.Run(() =>
                {
                    Task.Delay(100).Wait();
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
                        if (reconnect)
                        {
                            if (_savedCredentials)
                            {
                                _task = new Task(() =>
                                {
                                    try
                                    {
                                        Task.Delay(300, _cts.Token).Wait();
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
                                _task = new Task(() =>
                                {
                                    try
                                    {
                                        Task.Delay(250, _cts.Token).Wait();
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
                        }
                        else
                        {
                            _task = new Task(() =>
                            {
                                Task.Delay(50, _cts.Token).Wait();
                                this.EmitDisconnected(new RdpDisconnectReason(RdpDisconnectCode.FreshCredsRequired, 0, 0));
                                using (ReadWriteMonitor.Write(_monitor))
                                    _task = null;
                            }, _cts.Token, TaskCreationOptions.LongRunning);
                        }

                        _task.Start();
                    }
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Unexpected reason code {0}", reason.Code));
                }
            }
        }
    }
}
