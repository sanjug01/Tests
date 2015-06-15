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

            private bool _userRejected;

            protected override void Activated()
            {
                using(LockWrite())
                {
                    this.Session._syncEvents.ClientConnected += this.OnClientConnected;
                    this.Session._syncEvents.ClientAsyncDisconnect += this.OnClientAsyncDisconnect;
                    this.Session._syncEvents.ClientDisconnected += this.OnClientDisconnected;
                }

                this.Session.EmitBadServerIdentity(new BadServerIdentityEventArgs(_reason, this));
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

            public ValidateServerIdentity(IRdpConnection connection, RdpDisconnectReason reason, InternalState otherState)
                : base(SessionState.Idle, otherState)
            {
                Contract.Assert(null != connection);
                Contract.Assert(null != reason);

                _connection = connection;
                _reason = reason;
                _userRejected = false;
            }
            

            void IValidation.Accept()
            {
                Contract.Assert(null != this.Session);
                this.Session._isServerTrusted = true;

                using (LockUpgradeableRead())
                    _connection.HandleAsyncDisconnectResult(_reason, true);
            }

            void IValidation.Reject()
            {
                using (LockUpgradeableRead())
                {
                    _userRejected = true;
                    _connection.HandleAsyncDisconnectResult(_reason, false);
                }
            }

            DesktopModel IServerIdentityValidation.Desktop
            {
                get
                {
                    if (null != this.Session && null != this.Session._sessionSetup)
                    {
                        // hostname is stored in _session._sessionSetup.
                        return this.Session._sessionSetup.Connection as DesktopModel;
                    }
                    else
                    {
                        return null;
                    }
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
                Contract.Assert(null != this.Session);

                if (_userRejected)
                    ChangeState(new ClosedSession(_connection, this));
                else
                    ChangeState(new FailedSession(_connection, e.DisconnectReason, this));
            }
        }
    }
}
