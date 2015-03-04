namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.ViewModels.EditCredentialsTasks;
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;

    partial class RemoteSession
    {
        private sealed class ConnectingSession : InternalState
        {
            private readonly IRdpConnection _connection;
            private RemoteSession _session;

            public ConnectingSession(IRdpConnection connection, ReaderWriterLockSlim monitor)
                : base(SessionState.Connecting, monitor)
            {
                _connection = connection;
            }

            public ConnectingSession(IRdpConnection connection, InternalState otherState)
                : base(SessionState.Connecting, otherState)
            {
                _connection = connection;
            }

            public override void Activate(RemoteSession session)
            {
                Contract.Assert(null == _session);

                _session = session;
                _session._syncEvents.ClientConnected += this.OnClientConnected;
                _session._syncEvents.ClientAsyncDisconnect += this.OnClientAsyncDisconnect;
                _session._syncEvents.ClientDisconnected += this.OnClientDisconnected;
                _connection.Connect(_session._sessionSetup.SessionCredentials.Credentials,
                    !_session._sessionSetup.SessionCredentials.IsNewPassword);
            }

            public override void Deactivate(RemoteSession session)
            {
                Contract.Assert(null != _session);
            }

            public override void Complete(RemoteSession session)
            {
                Contract.Assert(null != _session);
                Contract.Assert(object.ReferenceEquals(_session, session));

                _session._syncEvents.ClientConnected -= this.OnClientConnected;
                _session._syncEvents.ClientAsyncDisconnect -= this.OnClientAsyncDisconnect;
                _session._syncEvents.ClientDisconnected -= this.OnClientDisconnected;
                _session = null;
            }

            private void OnClientConnected(object sender, ClientConnectedArgs e)
            {
                using(LockWrite())
                {
                    //
                    // Set the internal sesion state to Connected.
                    //
                    _session.InternalSetState(new ConnectedSession(_connection, this));
                }
            }

            private void OnClientAsyncDisconnect(object sender, ClientAsyncDisconnectArgs e)
            {
                Contract.Assert(sender is IRdpConnection);

                IRdpConnection connection = (IRdpConnection)sender;
                Contract.Assert(object.ReferenceEquals(connection, _connection));

                switch (e.DisconnectReason.Code)
                {
                    case RdpDisconnectCode.CertValidationFailed:
                        //
                        // Set the internal state to "certificate validation needed"
                        //
                        ValidateCertificate(connection.GetServerCertificate(), e.DisconnectReason);
                        break;

                    case RdpDisconnectCode.PreAuthLogonFailed:
                        RequestValidCredentials();
                        break;

                    case RdpDisconnectCode.FreshCredsRequired:
                        RequestNewPassword();
                        break;

                    default:
                        connection.HandleAsyncDisconnectResult(e.DisconnectReason, false);
                        break;
                }
            }
            private void OnClientDisconnected(object sender, ClientDisconnectedArgs e)
            {
                Contract.Assert(null != _session);
                Contract.Assert(sender is IRdpConnection);
                Contract.Assert(object.ReferenceEquals(sender, _connection));
                Contract.Ensures(null == _connection);

                switch(e.DisconnectReason.Code)
                {
                    case RdpDisconnectCode.UserInitiated:
                        _session.InternalSetState(new ClosedSession(this));
                        break;

                    default:
                        _session.InternalSetState(new FailedSession(e.DisconnectReason, this));
                        break;
                }
            }

            private void ValidateCertificate(IRdpCertificate certificate, RdpDisconnectReason reason)
            {
                Contract.Assert(null != certificate);
                Contract.Assert(null != _session);
                Contract.Assert(null != _connection);

                if (_session._certificateTrust.IsCertificateTrusted(certificate)
                    || _session._sessionSetup.DataModel.CertificateTrust.IsCertificateTrusted(certificate))
                {
                    //
                    // The certificate has been accepted already;
                    //
                    _connection.HandleAsyncDisconnectResult(reason, true);
                }
                else
                {
                    //
                    // Set the state to ValidateCertificate, that will emit a BadCertificate event from the session
                    // and handle the user's response to the event.
                    //
                    _session.InternalSetState(new ValidateCertificate(_connection, reason, this));
                }
            }

            private void RequestValidCredentials()
            {
                //
                // Emit an event with a credentials editor task.
                //
                InSessionCredentialsTask task = new InSessionCredentialsTask(_session._sessionSetup.SessionCredentials,
                    _session._sessionSetup.DataModel,
                    "d:Invalid user name or password");

                task.Submitted += this.NewPasswordSubmitted;
                task.Cancelled += this.NewPasswordCancelled;

                _session.DeferEmitCredentialsNeeded(task);
            }

            private void RequestNewPassword()
            {
                //
                // Emit an event with a credentials editor task.
                //
                InSessionCredentialsTask task = new InSessionCredentialsTask(_session._sessionSetup.SessionCredentials,
                    _session._sessionSetup.DataModel,
                    "d:Server has requested a new password to be typed in");

                task.Submitted += this.NewPasswordSubmitted;
                task.Cancelled += this.NewPasswordCancelled;

                _session.DeferEmitCredentialsNeeded(task);
            }

            private void NewPasswordSubmitted(object sender, InSessionCredentialsTask.SubmittedEventArgs e)
            {
                InSessionCredentialsTask task = (InSessionCredentialsTask)sender;

                task.Submitted -= this.NewPasswordSubmitted;
                task.Cancelled -= this.NewPasswordCancelled;

                if (e.SaveCredentials)
                    _session._sessionSetup.SaveCredentials();
                //
                // Go ahead and try to re-connect with new credentials.
                //
                _session.InternalSetState(new ConnectingSession(_connection, this));
            }

            private void NewPasswordCancelled(object sender, EventArgs e)
            {
                InSessionCredentialsTask task = (InSessionCredentialsTask)sender;

                task.Submitted -= this.NewPasswordSubmitted;
                task.Cancelled -= this.NewPasswordCancelled;
                //
                // User has cancelled the credentials dialog, tell the subscribers about the cancellation.
                //
                _session.InternalSetState(new ClosedSession(this));
            }
        }
    }
}
