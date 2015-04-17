namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Data;
    using System;
    using System.Diagnostics.Contracts;

    partial class RemoteSession
    {
        private sealed class ValidateServerIdentity: InternalState, IServerIdentityValidation
        {
            private readonly IRdpConnection _connection;
            private readonly RdpDisconnectReason _reason;

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

                _session.EmitBadServerIdentity(new BadServerIdentityEventArgs(_reason, this));
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

            public ValidateServerIdentity(IRdpConnection connection, RdpDisconnectReason reason, InternalState otherState)
                : base(SessionState.Idle, otherState)
            {
                Contract.Assert(null != connection);
                Contract.Assert(null != reason);

                _connection = connection;
                _reason = reason;
            }
            

            void IValidation.Accept()
            {
                Contract.Assert(null != _session);
                _session._isServerTrusted = true;

                using (LockUpgradeableRead())
                    _connection.HandleAsyncDisconnectResult(_reason, true);

            }

            void IValidation.Reject()
            {
                using (LockUpgradeableRead())
                    _connection.HandleAsyncDisconnectResult(_reason, false);
            }

            DesktopModel IServerIdentityValidation.Desktop
            {
                get
                {
                    if (null != _session && null != _session._sessionSetup)
                    {
                        // hostname is stored in _session._sessionSetup.
                        return _session._sessionSetup.Connection as DesktopModel;
                    }
                    else
                    {
                        return null;
                    }
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

                _session.InternalSetState(new FailedSession(_connection, e.DisconnectReason, this));
            }
        }
    }
}
