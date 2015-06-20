namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using System.Diagnostics.Contracts;

    partial class RemoteSession
    {
        private sealed class ValidateCertificate : InternalState, ICertificateValidation
        {
            private readonly IRdpConnection _connection;
            private readonly RdpDisconnectReason _reason;
            private readonly IRdpCertificate _certificate;
            private readonly string _hostName;

            //
            // Rendering panel is not used by the state but it is passed to it by the Connecting state
            // that will want it back after user will have accepted the failed certificate.
            //
            private readonly IRenderingPanel _renderingPanel;

            private bool _userRejected;

            protected override void Activated()
            {
                using(LockWrite())
                {
                    this.Session._syncEvents.ClientConnected += this.OnClientConnected;
                    this.Session._syncEvents.ClientAsyncDisconnect += this.OnClientAsyncDisconnect;
                    this.Session._syncEvents.ClientDisconnected += this.OnClientDisconnected;
                }

                this.Session.EmitBadCertificate(new BadCertificateEventArgs(_reason, _hostName, this));
            }

            protected override void Completed()
            {
                using (LockWrite())
                {
                    this.Session._syncEvents.ClientConnected -= this.OnClientConnected;
                    this.Session._syncEvents.ClientAsyncDisconnect -= this.OnClientAsyncDisconnect;
                    this.Session._syncEvents.ClientDisconnected -= this.OnClientDisconnected;
                }
            }

            public ValidateCertificate(IRenderingPanel renderingPanel, IRdpConnection connection, RdpDisconnectReason reason, string hostName, InternalState otherState)
                : base(SessionState.Idle, otherState)
            {
                Contract.Assert(null != connection);
                Contract.Assert(null != reason);
                Contract.Assert(null != renderingPanel);

                _connection = connection;
                _reason = reason;
                _hostName = hostName;
                if(RdpDisconnectCode.ProxyInvalidCA == reason.Code)
                {
                    // gateway certificate validation
                    _certificate = _connection.GetGatewayCertificate();
                }
                else
                {
                    _certificate = _connection.GetServerCertificate();
                }
                _renderingPanel = renderingPanel;
                _userRejected = false;
            }

            IRdpCertificate ICertificateValidation.Certificate
            {
                get { return _certificate; }
            }

            void IValidation.Accept()
            {
                using (LockUpgradeableRead())
                {
                    _connection.HandleAsyncDisconnectResult(_reason, true);
                    //
                    // Switch back to the Connecting state and give it back the rendering panel
                    // and connection.
                    //
                    ChangeState(new ConnectingSession(_renderingPanel, _connection, this));
                }
            }

            void IValidation.Reject()
            {
                using (LockUpgradeableRead())
                {
                    _userRejected = true;
                    _connection.HandleAsyncDisconnectResult(_reason, false);
                }
            }

            private void OnClientConnected(object sender, ClientConnectedArgs e)
            {
                ChangeState(new ConnectedSession(_connection, this));
            }

            private void OnClientAsyncDisconnect(object sender, ClientAsyncDisconnectArgs e)
            {
                using (LockUpgradeableRead())
                    _connection.HandleAsyncDisconnectResult(e.DisconnectReason, false);
            }

            private void OnClientDisconnected(object sender, ClientDisconnectedArgs e)
            {
                if(_userRejected)
                    ChangeState(new ClosedSession(_connection, this));
                else
                    ChangeState(new FailedSession(_connection, e.DisconnectReason, this));
            }
        }
    }
}
