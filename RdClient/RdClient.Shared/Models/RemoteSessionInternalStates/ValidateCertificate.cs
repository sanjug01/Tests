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

            private RemoteSession _session;

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

                _session.EmitBadCertificate(new BadCertificateEventArgs(_reason, this));
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

            public ValidateCertificate(IRdpConnection connection, RdpDisconnectReason reason, InternalState otherState)
                : base(SessionState.Idle, otherState)
            {
                Contract.Assert(null != connection);
                Contract.Assert(null != reason);

                _connection = connection;
                _reason = reason;
                _certificate = _connection.GetServerCertificate();
            }

            IRdpCertificate ICertificateValidation.Certificate
            {
                get { return _certificate; }
            }

            void ICertificateValidation.Accept()
            {
                using (LockUpgradeableRead())
                    _connection.HandleAsyncDisconnectResult(_reason, true);
            }

            void ICertificateValidation.Reject()
            {
                using (LockUpgradeableRead())
                    _connection.HandleAsyncDisconnectResult(_reason, false);
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

                _session.InternalSetState(new FailedSession(e.DisconnectReason, this));
            }
        }
    }
}
