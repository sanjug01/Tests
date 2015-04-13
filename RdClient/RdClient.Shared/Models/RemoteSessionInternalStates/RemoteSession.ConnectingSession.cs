namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.ViewModels.EditCredentialsTasks;
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Threading;

    partial class RemoteSession
    {
        private sealed class ConnectingSession : InternalState
        {
            private readonly IRenderingPanel _renderingPanel;
            private RemoteSession _session;
            private IRdpConnection _connection;
            private bool _cancelledCredentials;

            public ConnectingSession(IRenderingPanel renderingPanel, ReaderWriterLockSlim monitor)
                : base(SessionState.Connecting, monitor)
            {
                Contract.Assert(null != renderingPanel);
                Contract.Assert(null != monitor);

                _renderingPanel = renderingPanel;
                _cancelledCredentials = false;
            }

            public override void Activate(RemoteSession session)
            {
                Contract.Assert(null == _session);

                _session = session;
                _renderingPanel.Ready += this.OnRenderingPanelReady;
            }

            public override void Deactivate(RemoteSession session)
            {
                Contract.Assert(null != _session);
            }

            public override void Terminate(RemoteSession session)
            {
                if(null != _connection)
                    _connection.Disconnect();
            }

            public override void Complete(RemoteSession session)
            {
                Contract.Assert(null != _session);
                Contract.Assert(object.ReferenceEquals(_session, session));

                _renderingPanel.Ready -= this.OnRenderingPanelReady;
                if (null != _connection)
                {
                    _session._syncEvents.ClientConnected -= this.OnClientConnected;
                    _session._syncEvents.ClientAsyncDisconnect -= this.OnClientAsyncDisconnect;
                    _session._syncEvents.ClientDisconnected -= this.OnClientDisconnected;
                    _session._syncEvents.StatusInfoReceived -= this.OnStatusInfoReceived;
                }
                _session = null;
            }

            private void OnRenderingPanelReady(object sender, EventArgs e)
            {
                _connection = _session.InternalCreateConnection(_renderingPanel);
                Contract.Assert(null != _connection);
                Contract.Assert(null != _session._syncEvents);

                _session._syncEvents.ClientConnected += this.OnClientConnected;
                _session._syncEvents.ClientAsyncDisconnect += this.OnClientAsyncDisconnect;
                _session._syncEvents.ClientDisconnected += this.OnClientDisconnected;
                _session._syncEvents.StatusInfoReceived += this.OnStatusInfoReceived;

                _connection.SetCredentials(_session._sessionSetup.SessionCredentials.Credentials,
                    !_session._sessionSetup.SessionCredentials.IsNewPassword);

                // pass gateway, if necessary
                if (_session._sessionSetup.SessionGateway.HasGateway)
                {
                    _connection.SetGateway(
                        _session._sessionSetup.SessionGateway.Gateway,
                        _session._sessionSetup.SessionGateway.Credentials);
                }

                _connection.Connect();
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
                        RequestValidCredentials(e.DisconnectReason);
                        break;

                    case RdpDisconnectCode.FreshCredsRequired:
                        RequestNewPassword(e.DisconnectReason);
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

                if (RdpDisconnectCode.UserInitiated == e.DisconnectReason.Code || _cancelledCredentials)
                {
                    //
                    // If user has disconnected (unlikely), or the credentials prompt was cancelled,
                    // go to the Closed state, so the session view will navigate to the connection center page.
                    //
                    _session.InternalSetState(new ClosedSession(_connection, this));
                }
                else
                {
                    //
                    // For all other failures go to the Failed state so the sessoin view will show the error UI.
                    //
                    _session.InternalSetState(new FailedSession(_connection, e.DisconnectReason, this));
                }
            }

            private void OnStatusInfoReceived(object sender, StatusInfoReceivedArgs e)
            {
                Debug.WriteLine("StatusInfoReceived|StatusCode={0}", e.StatusCode);
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

            private void RequestValidCredentials(RdpDisconnectReason reason)
            {
                //
                // Emit an event with a credentials editor task.
                //
                InSessionCredentialsTask task = new InSessionCredentialsTask(_session._sessionSetup.SessionCredentials,
                    _session._sessionSetup.DataModel,
                    "d:Invalid user name or password",
                    reason);

                task.Submitted += this.NewPasswordSubmitted;
                task.Cancelled += this.NewPasswordCancelled;

                _session.DeferEmitCredentialsNeeded(task);
            }

            private void RequestNewPassword(RdpDisconnectReason reason)
            {
                //
                // Emit an event with a credentials editor task.
                //
                InSessionCredentialsTask task = new InSessionCredentialsTask(_session._sessionSetup.SessionCredentials,
                    _session._sessionSetup.DataModel,
                    "d:Server has requested a new password to be typed in",
                    reason);

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
                // Stay in the same state, update the session credentials and call HandleAsyncDisconnectResult
                // to re-connect.
                //
                using (LockWrite())
                {
                    _connection.SetCredentials(_session._sessionSetup.SessionCredentials.Credentials,
                        !_session._sessionSetup.SessionCredentials.IsNewPassword);
                    _connection.HandleAsyncDisconnectResult((RdpDisconnectReason)e.State, true);
                }
            }

            private void NewPasswordCancelled(object sender, InSessionCredentialsTask.ResultEventArgs e)
            {
                InSessionCredentialsTask task = (InSessionCredentialsTask)sender;

                task.Submitted -= this.NewPasswordSubmitted;
                task.Cancelled -= this.NewPasswordCancelled;
                //
                // User has cancelled the credentials dialog.
                // Stay in the state and wait for the connection to terminate.
                //
                using(LockWrite())
                {
                    _cancelledCredentials = true;
                    _connection.HandleAsyncDisconnectResult((RdpDisconnectReason)e.State, false);
                }
            }
        }
    }
}
