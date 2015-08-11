namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.CxWrappers.Utils;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Telemetry;
    using RdClient.Shared.ViewModels;
    using RdClient.Shared.ViewModels.EditCredentialsTasks;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Text;
    using System.Threading;
    using Windows.Networking.Connectivity;

    partial class RemoteSession
    {
        private sealed class ConnectingSession : InternalState
        {
            private readonly IRenderingPanel _renderingPanel;
            private IRdpConnection _connection;
            private bool _cancelledCredentials;

            public ConnectingSession(IRenderingPanel renderingPanel, InternalState otherState)
                : base(SessionState.Connecting, otherState)
            {
                Contract.Assert(null != renderingPanel);

                _renderingPanel = renderingPanel;
                _cancelledCredentials = false;
            }

            public ConnectingSession(IRenderingPanel renderingPanel, IRdpConnection connection, InternalState otherState)
                : base(SessionState.Connecting, otherState)
            {
                Contract.Assert(null != renderingPanel);
                Contract.Assert(null != connection);

                _renderingPanel = renderingPanel;
                _connection = connection;
                _cancelledCredentials = false;
            }

            protected override void Activated()
            {
                _renderingPanel.Ready += this.OnRenderingPanelReady;

                if (!this.Session._networkTypeReported)
                {
                    CollectNetworkTelemetry();
                    this.Session._networkTypeReported = true;
                }
            }

            protected override void Terminate()
            {
                this.TelemetryClient.ReportEvent(new Telemetry.Events.UserAction()
                {
                    action = Telemetry.Events.UserAction.Action.CancelConnectingSession,
                    source = Telemetry.Events.UserAction.Source.ConnectingSessionState
                });

                if (null != _connection)
                    _connection.Disconnect();
            }

            protected override void Completed()
            {
                _renderingPanel.Ready -= this.OnRenderingPanelReady;

                if (null != _connection)
                {
                    this.Session._syncEvents.ClientConnected -= this.OnClientConnected;
                    this.Session._syncEvents.ClientAsyncDisconnect -= this.OnClientAsyncDisconnect;
                    this.Session._syncEvents.ClientDisconnected -= this.OnClientDisconnected;
                    this.Session._syncEvents.StatusInfoReceived -= this.OnStatusInfoReceived;
                    this.Session._syncEvents.CheckGatewayCertificateTrust -= this.OnCheckGatewayCertificateTrust;
                }
            }

            private void OnRenderingPanelReady(object sender, EventArgs e)
            {
                if (null == _connection)
                {
                    _connection = this.Session.InternalCreateConnection(_renderingPanel);
                    Contract.Assert(null != _connection);
                    Contract.Assert(null != this.Session._syncEvents);

                    this.Session._syncEvents.ClientConnected += this.OnClientConnected;
                    this.Session._syncEvents.ClientAsyncDisconnect += this.OnClientAsyncDisconnect;
                    this.Session._syncEvents.ClientDisconnected += this.OnClientDisconnected;
                    this.Session._syncEvents.StatusInfoReceived += this.OnStatusInfoReceived;
                    this.Session._syncEvents.CheckGatewayCertificateTrust += this.OnCheckGatewayCertificateTrust;

                    _connection.SetCredentials(this.Session._sessionSetup.SessionCredentials.Credentials,
                        !this.Session._sessionSetup.SessionCredentials.IsNewPassword);

                    // pass gateway, if necessary
                    if (this.Session._sessionSetup.SessionGateway.HasGateway)
                    {
                        _connection.SetGateway(
                            this.Session._sessionSetup.SessionGateway.Gateway,
                            this.Session._sessionSetup.SessionGateway.Credentials);
                    }


                    RdpPropertyApplier.ApplyScaleFactor((IRdpProperties) _connection, _renderingPanel.ScaleFactor);

                    _connection.Connect();
                }
                else
                {
                    this.Session._syncEvents.ClientConnected += this.OnClientConnected;
                    this.Session._syncEvents.ClientAsyncDisconnect += this.OnClientAsyncDisconnect;
                    this.Session._syncEvents.ClientDisconnected += this.OnClientDisconnected;
                    this.Session._syncEvents.StatusInfoReceived += this.OnStatusInfoReceived;
                    this.Session._syncEvents.CheckGatewayCertificateTrust += this.OnCheckGatewayCertificateTrust;
                }
            }

            private static string GetIanaInterfaceTypeString(uint ianaType)
            {
                return ((IanaInterfaceType)ianaType).ToString();
            }

            private void OnCheckGatewayCertificateTrust(object sender, CheckGatewayCertificateTrustArgs e)
            {
                // (pre)validation of the gateway certificate, per RDPConnection request
                IRdpCertificate certificate = e.Certificate;
                Contract.Assert(null != certificate);
                
                if (this.Session._certificateTrust.IsCertificateTrusted(certificate)
                    || this.Session._sessionSetup.DataModel.CertificateTrust.IsCertificateTrusted(certificate))
                {
                    // The certificate has been accepted already;
                    e.TrustDelegate.Invoke(true);
                }
                else
                {
                    // The certificate has not been trusted yet
                    e.TrustDelegate.Invoke(false);
                }
            }

            private void OnClientConnected(object sender, ClientConnectedArgs e)
            {
                using(LockWrite())
                {
                    //
                    // Set the internal sesion state to Connected.
                    //
                    ChangeState(new ConnectedSession(_connection, this));
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
                        ValidateCertificate(connection.GetServerCertificate(), e.DisconnectReason, this.Session._sessionSetup.HostName);
                        break;

                    case RdpDisconnectCode.PreAuthLogonFailed:
                        RequestValidCredentials(e.DisconnectReason);
                        break;

                    case RdpDisconnectCode.FreshCredsRequired:
                        RequestNewPassword(e.DisconnectReason);
                        break;

                    case RdpDisconnectCode.ProxyNeedCredentials:
                        // Gateway needs credentials
                        RequestNewGatewayCredentials(e.DisconnectReason);
                        break;

                    case RdpDisconnectCode.ProxyLogonFailed:
                        // Gateway credentials failed - prompt for new credentials
                        RequestValidGatewayCredentials(e.DisconnectReason);
                        break;

                    case RdpDisconnectCode.ProxyInvalidCA:
                        // Gateway certificate needs validation
                        ValidateCertificate(connection.GetGatewayCertificate(), e.DisconnectReason,
                            this.Session._sessionSetup.SessionGateway.Gateway.HostName);
                        break;

                    case RdpDisconnectCode.CredSSPUnsupported:
                        // Set the internal state to "certificate validation needed"
                        // Should prompt that the server identity cannot be verified 
                        ValidateServerIdentity(this.Session._sessionSetup.HostName , e.DisconnectReason);
                        break;
                    default:
                        connection.HandleAsyncDisconnectResult(e.DisconnectReason, false);
                        break;
                }
            }
            private void OnClientDisconnected(object sender, ClientDisconnectedArgs e)
            {
                Contract.Assert(sender is IRdpConnection);
                Contract.Assert(object.ReferenceEquals(sender, _connection));
                Contract.Ensures(null == _connection);

                if (RdpDisconnectCode.UserInitiated == e.DisconnectReason.Code || _cancelledCredentials)
                {
                    //
                    // If user has disconnected (unlikely), or the credentials prompt was cancelled,
                    // go to the Closed state, so the session view will navigate to the connection center page.
                    //
                    ChangeState(new ClosedSession(_connection, this));
                }
                else
                {
                    //
                    // For all other failures go to the Failed state so the sessoin view will show the error UI.
                    //
                    ChangeState(new FailedSession(_connection, e.DisconnectReason, this));
                }
            }

            private void OnStatusInfoReceived(object sender, StatusInfoReceivedArgs e)
            {
                Debug.WriteLine("Connecting|StatusInfoReceived|StatusCode={0}", e.StatusCode);
            }

            private void ValidateCertificate(IRdpCertificate certificate, RdpDisconnectReason reason, string serverName)
            {
                Contract.Assert(null != certificate);
                Contract.Assert(null != _connection);

                if (this.Session._certificateTrust.IsCertificateTrusted(certificate)
                    || this.Session._sessionSetup.DataModel.CertificateTrust.IsCertificateTrusted(certificate))
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
                    ChangeState(new ValidateCertificate(_renderingPanel, _connection, reason, serverName, this));
                }
            }

            private void ValidateServerIdentity(String hostName, RdpDisconnectReason reason)
            {
                Contract.Assert(null != this.Session._sessionSetup);
                Contract.Assert(null != _connection);

                if(
                    this.Session._isServerTrusted || 
                    ( null != (this.Session._sessionSetup.Connection as DesktopModel)
                    && (this.Session._sessionSetup.Connection as DesktopModel).IsTrusted )
                  )
                {
                    _connection.HandleAsyncDisconnectResult(reason, true);
                }
                else
                {
                    ChangeState(new ValidateServerIdentity(_connection, reason, this));
                }
            }

            private void RequestValidCredentials(RdpDisconnectReason reason)
            {
                //
                // Emit an event with a credentials editor task.
                //
                InSessionCredentialsTask task = new InSessionCredentialsTask(this.Session._sessionSetup.SessionCredentials,
                    this.Session._sessionSetup.DataModel,
                    CredentialPromptMode.InvalidCredentials,
                    reason);

                task.Submitted += this.NewPasswordSubmitted;
                task.Cancelled += this.NewPasswordCancelled;

                this.Session.DeferEmitCredentialsNeeded(task);
            }

            private void RequestNewPassword(RdpDisconnectReason reason)
            {
                //
                // Emit an event with a credentials editor task.
                //
                InSessionCredentialsTask task = new InSessionCredentialsTask(this.Session._sessionSetup.SessionCredentials,
                    this.Session._sessionSetup.DataModel,
                    CredentialPromptMode.FreshCredentialsNeeded,
                    reason);

                task.Submitted += this.NewPasswordSubmitted;
                task.Cancelled += this.NewPasswordCancelled;

                this.Session.DeferEmitCredentialsNeeded(task);
            }

            private void NewPasswordSubmitted(object sender, InSessionCredentialsTask.SubmittedEventArgs e)
            {
                InSessionCredentialsTask task = (InSessionCredentialsTask)sender;

                task.Submitted -= this.NewPasswordSubmitted;
                task.Cancelled -= this.NewPasswordCancelled;

                if (e.SaveCredentials)
                    this.Session._sessionSetup.SaveCredentials();
                //
                // Go ahead and try to re-connect with new credentials.
                // Stay in the same state, update the session credentials and call HandleAsyncDisconnectResult
                // to re-connect.
                //
                using (LockWrite())
                {
                    _connection.SetCredentials(this.Session._sessionSetup.SessionCredentials.Credentials,
                        !this.Session._sessionSetup.SessionCredentials.IsNewPassword);
                    _connection.HandleAsyncDisconnectResult((RdpDisconnectReason)e.State, true);
                }
            }

            private void NewPasswordCancelled(object sender, InSessionCredentialsTask.ResultEventArgs e)
            {
                InSessionCredentialsTask task = (InSessionCredentialsTask)sender;

                this.TelemetryClient.ReportEvent(new Telemetry.Events.UserAction()
                {
                    action = Telemetry.Events.UserAction.Action.CancelCredentials,
                    source = Telemetry.Events.UserAction.Source.ConnectingSessionState
                });

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


            private void RequestValidGatewayCredentials(RdpDisconnectReason reason)
            {
                //
                // Emit an event with a credentials editor task.
                //
                InSessionCredentialsTask task = new InSessionCredentialsTask(
                    this.Session._sessionSetup.SessionGateway,
                    this.Session._sessionSetup.DataModel,
                    CredentialPromptMode.InvalidGatewayCredentials,
                    reason);

                task.Submitted += this.NewGatewayCredentialsSubmitted;
                task.Cancelled += this.NewGatewayCredentialsCancelled;

                this.Session.DeferEmitCredentialsNeeded(task);
            }

            private void RequestNewGatewayCredentials(RdpDisconnectReason reason)
            {
                //
                // Emit an event with a credentials editor task.
                //
                InSessionCredentialsTask task = new InSessionCredentialsTask(
                    this.Session._sessionSetup.SessionGateway,
                    this.Session._sessionSetup.DataModel,
                    CredentialPromptMode.FreshGatewayCredentialsNeeded,
                    reason);

                task.Submitted += this.NewGatewayCredentialsSubmitted;
                task.Cancelled += this.NewGatewayCredentialsCancelled;

                this.Session.DeferEmitCredentialsNeeded(task);
            }

            private void NewGatewayCredentialsSubmitted(object sender, InSessionCredentialsTask.SubmittedEventArgs e)
            {
                InSessionCredentialsTask task = (InSessionCredentialsTask)sender;

                task.Submitted -= this.NewGatewayCredentialsSubmitted;
                task.Cancelled -= this.NewGatewayCredentialsCancelled;

                if (e.SaveCredentials)
                    this.Session._sessionSetup.SaveGatewayCredentials();
                //
                // Go ahead and try to re-connect with new gateway credentials.
                // Stay in the same state, update the session credentials and call HandleAsyncDisconnectResult
                // to re-connect.
                //
                using (LockWrite())
                {
                    _connection.SetGateway(this.Session._sessionSetup.SessionGateway.Gateway,
                        this.Session._sessionSetup.SessionGateway.Credentials);
                    _connection.HandleAsyncDisconnectResult((RdpDisconnectReason)e.State, true);
                }
            }

            private void NewGatewayCredentialsCancelled(object sender, InSessionCredentialsTask.ResultEventArgs e)
            {
                InSessionCredentialsTask task = (InSessionCredentialsTask)sender;

                this.TelemetryClient.ReportEvent(new Telemetry.Events.UserAction()
                {
                    action = Telemetry.Events.UserAction.Action.CancelGatewayCredentials,
                    source = Telemetry.Events.UserAction.Source.ConnectingSessionState
                });

                task.Submitted -= this.NewGatewayCredentialsCancelled;
                task.Cancelled -= this.NewGatewayCredentialsCancelled;
                //
                // User has cancelled the credentials dialog.
                // Stay in the state and wait for the connection to terminate.
                //
                using (LockWrite())
                {
                    _cancelledCredentials = true;
                    _connection.HandleAsyncDisconnectResult((RdpDisconnectReason)e.State, false);
                }
            }

            private void CollectNetworkTelemetry()
            {
                //
                // Collect network information and cram it into
                //
                ConnectionProfileFilter filter = new ConnectionProfileFilter() { IsConnected = true };
                IReadOnlyList<ConnectionProfile> profiles = NetworkInformation.FindConnectionProfilesAsync(filter).AsTask().Result;
                StringBuilder sb = null;

                foreach (ConnectionProfile profile in profiles)
                {
                    if (null == sb)
                        sb = new StringBuilder();
                    else
                        sb.Append(',');
                    sb.Append(GetIanaInterfaceTypeString(profile.NetworkAdapter.IanaInterfaceType));

                    if (profile.IsWwanConnectionProfile)
                    {
                        WwanDataClass dataClass = profile.WwanConnectionProfileDetails.GetCurrentDataClass();

                        foreach (WwanDataClass wwdc in Enum.GetValues(typeof(WwanDataClass)))
                        {
                            if (WwanDataClass.None != wwdc && wwdc == (dataClass & wwdc))
                            {
                                sb.Append(':');
                                sb.Append(wwdc);
                            }
                        }
                    }
                }

                this.SessionLaunch.networkType = null != sb ? sb.ToString() : "unknown";
            }
        }
    }
}
