namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Data;
    using System;
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

            private RemoteSession _session;
            private bool _userRejected;

            public override void Activate(RemoteSession session)
            {
                Contract.Assert(null == _session);
                Contract.Ensures(null != _session);

                _session = session;

                using(LockWrite())
                {
                    _session._syncEvents.ClientConnected += this.OnClientConnected;
                    _session._syncEvents.ClientAsyncDisconnect += this.OnClientAsyncDisconnect;
                    _session._syncEvents.ClientDisconnected += this.OnClientDisconnected;
                }

                _session.EmitBadCertificate(new BadCertificateEventArgs(_reason, _hostName, this));
            }

            public override void Complete(RemoteSession session)
            {
                Contract.Assert(object.ReferenceEquals(_session, session));
                Contract.Ensures(null == _session);

                using (LockWrite())
                {
                    _session._syncEvents.ClientConnected -= this.OnClientConnected;
                    _session._syncEvents.ClientAsyncDisconnect -= this.OnClientAsyncDisconnect;
                    _session._syncEvents.ClientDisconnected -= this.OnClientDisconnected;
                }

                _session = null;
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
                    _session.InternalSetState(new ConnectingSession(_renderingPanel, _connection, this));
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
                _session.InternalSetState(new ConnectedSession(_connection, this));
            }

            private void OnClientAsyncDisconnect(object sender, ClientAsyncDisconnectArgs e)
            {
                using (LockUpgradeableRead())
                    _connection.HandleAsyncDisconnectResult(e.DisconnectReason, false);
            }

            private void OnClientDisconnected(object sender, ClientDisconnectedArgs e)
            {
                Contract.Assert(null != _session);

                if(_userRejected)
                    _session.InternalSetState(new ClosedSession(_connection, this));
                else
                    _session.InternalSetState(new FailedSession(_connection, e.DisconnectReason, this));
            }
        }
    }
}
