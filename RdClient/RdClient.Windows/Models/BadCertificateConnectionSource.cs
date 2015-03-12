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

    sealed class BadCertificateConnectionSource : ImitationRdpConnectionSource
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

        private sealed class Certificate : IRdpCertificate
        {
            private readonly byte[] _serialNumber, _hashValue;
            private readonly DateTimeOffset _validFrom, _validTo;
            private readonly IRdpCertificateError _error;

            private sealed class Error : IRdpCertificateError
            {

                int IRdpCertificateError.ErrorCode
                {
                    get { return 42; }
                }

                CertificateError IRdpCertificateError.ErrorFlags
                {
                    get { return CertificateError.UntrustedRoot|CertificateError.CertOrChainInvalid; }
                }

                ServerCertificateErrorSource IRdpCertificateError.ErrorSource
                {
                    get { return ServerCertificateErrorSource.None; }
                }
            }

            public Certificate()
            {
                Random rnd = new Random();

                _serialNumber = new byte[16];
                rnd.NextBytes(_serialNumber);
                _hashValue = new byte[16];
                rnd.NextBytes(_hashValue);
                _validFrom = DateTimeOffset.Now;
                _validTo = DateTimeOffset.MaxValue;
                _error = new Error();
            }

            string IRdpCertificate.FriendlyName
            {
                get { return "Test Certificate"; }
            }

            bool IRdpCertificate.HasPrivateKey
            {
                get { return false; }
            }

            bool IRdpCertificate.IsStronglyProtected
            {
                get { return true; }
            }

            string IRdpCertificate.Issuer
            {
                get { return "Universal RdClient Test"; }
            }

            byte[] IRdpCertificate.SerialNumber
            {
                get { return _serialNumber; }
            }

            string IRdpCertificate.Subject
            {
                get { return "Certificate Subject"; }
            }

            DateTimeOffset IRdpCertificate.ValidFrom
            {
                get { return _validFrom; }
            }

            DateTimeOffset IRdpCertificate.ValidTo
            {
                get { return _validTo; }
            }

            byte[] IRdpCertificate.GetHashValue()
            {
                return _hashValue;
            }

            byte[] IRdpCertificate.GetHashValue(string hashAlgorithmName)
            {
                return _hashValue;
            }

            IRdpCertificateError IRdpCertificate.Error
            {
                get { return _error; }
            }
        }

        private sealed class Logic : ImitationRdpConnectionSource.Connection
        {
            private readonly CancellationTokenSource _cts;
            private readonly ReaderWriterLockSlim _monitor;
            private readonly IRdpCertificate _certificate;
            private Task _task;

            public Logic(IRenderingPanel renderingPanel)
                : base(renderingPanel)
            {
                _cts = new CancellationTokenSource();
                _monitor = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
                _certificate = new Certificate();
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
                    try
                    {
                        _task = new Task(() =>
                        {
                            Task.Delay(300, _cts.Token).Wait();
                            this.EmitAsyncDisconnect(new RdpDisconnectReason(RdpDisconnectCode.CertValidationFailed, 0, 0));
                            using (ReadWriteMonitor.Write(_monitor))
                                _task = null;
                        });
                    }
                    catch (OperationCanceledException)
                    {
                        this.EmitDisconnected(new RdpDisconnectReason(RdpDisconnectCode.UserInitiated, 0, 0));
                        using (ReadWriteMonitor.Write(_monitor))
                            _task = null;
                    }

                    _task.Start();
                }
            }

            protected override void Disconnect()
            {
                using(LockWrite())
                {
                    Contract.Assert(null != _task);

                    Task.Run(() =>
                    {
                        Task.Delay(100).Wait();
                        _cts.Cancel();
                    });
                }
            }

            protected override void HandleAsyncDisconnectResult(RdpDisconnectReason reason, bool reconnect)
            {
                if (RdpDisconnectCode.CertValidationFailed == reason.Code)
                {
                    Task t;

                    using (ReadWriteMonitor.Read(_monitor))
                        t = _task;

                    if (null != t)
                        t.Wait();

                    using (ReadWriteMonitor.Write(_monitor))
                    {
                        if(reconnect)
                        {
                            _task = new Task(() =>
                            {
                                Task.Delay(150, _cts.Token).Wait();
                                this.EmitConnected();
                                _cts.Token.WaitHandle.WaitOne();
                                Task.Delay(100).Wait();
                                this.EmitDisconnected(new RdpDisconnectReason(RdpDisconnectCode.UserInitiated, 0, 0));
                                using (ReadWriteMonitor.Write(_monitor))
                                    _task = null;
                            }, _cts.Token, TaskCreationOptions.LongRunning);
                        }
                        else
                        {
                            _task = new Task(() =>
                            {
                                Task.Delay(50, _cts.Token).Wait();
                                this.EmitDisconnected(new RdpDisconnectReason(RdpDisconnectCode.CertValidationFailed, 0, 0));
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

            protected override IRdpCertificate GetServerCertificate()
            {
                return _certificate;
            }
        }
    }
}
