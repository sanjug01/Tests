namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.ViewModels;
    using RdClient.Shared.ViewModels.EditCredentialsTasks;
    using System;
    using System.Diagnostics.Contracts;

    public sealed class RemoteSession : MutableObject, IRemoteSession
    {
        private readonly RemoteSessionSetup _sessionSetup;
        private readonly RemoteSessionState _state;
        private readonly IDeferredExecution _deferredExecution;
        private readonly IRdpConnectionSource _connectionSource;
        private readonly ICertificateTrust _certificateTrust;

        private EventHandler<CredentialsNeededEventArgs> _credentialsNeeded;
        private EventHandler _cancelled;
        private EventHandler<SessionFailureEventArgs> _failed;
        private EventHandler _closed;

        private IRemoteSessionView _sessionView;
        private IRenderingPanel _renderingPanel;
        private IRdpConnection _connection;

        public RemoteSession(RemoteSessionSetup sessionSetup, IDeferredExecution deferredExecution, IRdpConnectionSource connectionSource)
        {
            Contract.Requires(null != sessionSetup);
            Contract.Requires(null != deferredExecution);
            Contract.Requires(null != connectionSource);
            Contract.Ensures(null != _sessionSetup);
            Contract.Ensures(null != _deferredExecution);
            Contract.Ensures(null != _connectionSource);
            Contract.Ensures(null != _certificateTrust);
            Contract.Ensures(null != _state);

            _sessionSetup = sessionSetup;
            _deferredExecution = deferredExecution;
            _connectionSource = connectionSource;
            _state = new RemoteSessionState(deferredExecution);
            _certificateTrust = new CertificateTrust();
        }

        IRemoteSessionState IRemoteSession.State
        {
            get
            {
                Contract.Ensures(null != Contract.Result<IRemoteSessionState>());
                return _state;
            }
        }

        ICertificateTrust IRemoteSession.CertificateTrust
        {
            get
            {
                Contract.Ensures(null != Contract.Result<ICertificateTrust>());
                return _certificateTrust;
            }
        }

        event EventHandler<CredentialsNeededEventArgs> IRemoteSession.CredentialsNeeded
        {
            add
            {
                using(LockWrite())
                    _credentialsNeeded += value;
            }

            remove
            {
                using (LockWrite())
                    _credentialsNeeded -= value;
            }
        }

        event EventHandler IRemoteSession.Cancelled
        {
            add
            {
                using (LockWrite())
                    _cancelled += value;
            }

            remove
            {
                using (LockWrite())
                    _cancelled -= value;
            }
        }

        event EventHandler<SessionFailureEventArgs> IRemoteSession.Failed
        {
            add
            {
                using (LockWrite())
                    _failed += value;
            }

            remove
            {
                using (LockWrite())
                    _failed -= value;
            }
        }

        event EventHandler IRemoteSession.Closed
        {
            add
            {
                using (LockWrite())
                    _closed += value;
            }

            remove
            {
                using (LockWrite())
                    _closed -= value;
            }
        }

        IRemoteSessionControl IRemoteSession.Activate(IRemoteSessionView sessionView)
        {
            Contract.Assert(null == _sessionView);
            Contract.Requires(null != sessionView);
            Contract.Ensures(null != _sessionView);

            //
            // Obtain a rendering panel from the session view and set up an RDP connection using the panel.
            //
            _sessionView = sessionView;
            _renderingPanel = _sessionView.ActivateNewRenderingPanel();

            using(LockUpgradeableRead())
            {
                if (null == _connection)
                {
                    if(_sessionSetup.Connection is DesktopModel)
                    {
                        DesktopModel dtm = (DesktopModel)_sessionSetup.Connection;
                        //
                        // If the desktop is configured to always ask credentials, emit the CredentialsNeeded event with a task
                        // that handles entry of new credentials.
                        //
                        if (Guid.Empty.Equals(dtm.CredentialsId))
                        {
                            //
                            // Request on the calling thread, that is the UI thread;
                            //
                            InSessionCredentialsTask task = new InSessionCredentialsTask(_sessionSetup.SessionCredentials,
                                _sessionSetup.DataModel, "d:Connection is set up to always ask credentials");
                            task.Submitted += this.MissingCredentialsSubmitted;
                            task.Cancelled += this.MissingCredentialsCancelled;
                            EmitCredentialsNeeded(task);
                        }
                        else
                        {
                            InternalStartSession(_sessionSetup);
                        }
                    }
                    else
                    {
                        //
                        // Can only connect to desktops
                        //
                        throw new NotImplementedException();
                    }
                }
                else
                {
                    //
                    // TODO: re-activate the connection.
                    //
                }
            }

            return new RemoteSessionControl(_connection);
        }

        IRenderingPanel IRemoteSession.Deactivate()
        {
            Contract.Assert(null != _renderingPanel);
            IRenderingPanel renderingPanel = _renderingPanel;
            _renderingPanel = null;
            return renderingPanel;
        }

        void IRemoteSession.Suspend()
        {
            Contract.Assert(null != _renderingPanel);
            Contract.Assert(null != _sessionView);
            //
            // TODO: Find out whether suspension is an asynchronous process
            //       and we must wait for some events from the connection.
            //
            _connection.Suspend();
            //
            // Recycle the rendering panel;
            //
            _sessionView.RecycleRenderingPanel(_renderingPanel);
            _renderingPanel = null;
            _sessionView = null;
        }

        void IRemoteSession.Disconnect()
        {
            //
            // TODO: Wait for the connection to disconnect and recycle the rendering panel after that.
            //
            _connection.Disconnect();
        }

        private void DeferEmitCredentialsNeeded(IEditCredentialsTask task)
        {
            //
            // Simply defer emitting the event to the dispatcher.
            //
            _deferredExecution.Defer(() => EmitCredentialsNeeded(task));
        }

        private void EmitCredentialsNeeded(IEditCredentialsTask task)
        {
            Contract.Requires(null != task);

            using (LockUpgradeableRead())
            {
                if (null != _credentialsNeeded)
                    _credentialsNeeded(this, new CredentialsNeededEventArgs(task));
            }
        }

        private void EmitCancelled()
        {
            using(LockUpgradeableRead())
            {
                if (null != _cancelled)
                    _cancelled(this, EventArgs.Empty);
            }
        }

        private void EmitFailed(RdpDisconnectCode disconnectCode)
        {
            using(LockUpgradeableRead())
            {
                if (null != _failed)
                    _failed(this, new SessionFailureEventArgs(disconnectCode));
            }
        }

        private void DeferEmitFailed(RdpDisconnectCode disconnectCode)
        {
            _deferredExecution.Defer(() => EmitFailed(disconnectCode));
        }

        private void EmitClosed()
        {
            using(LockUpgradeableRead())
            {
                if (null != _closed)
                    _closed(this, EventArgs.Empty);
            }
        }

        private void DeferEmitClosed()
        {
            _deferredExecution.Defer(() => EmitClosed());
        }

        private void InternalStartSession(RemoteSessionSetup sessionSetup)
        {
            //
            // Ask the connection source to create a new session.
            // The connection source comes all the way from XAML of the main page.
            //
            using (LockWrite())
            {
                _connection = _connectionSource.CreateConnection(_renderingPanel);

                _connection.Events.ClientConnected += this.OnClientConnected;
                _connection.Events.ClientAsyncDisconnect += this.OnClientAsyncDisconnect;
                _connection.Events.ClientDisconnected += this.OnClientDisconnected;

                _state.SetState(SessionState.Connecting);
                _connection.Connect(_sessionSetup.SessionCredentials.Credentials,
                    !_sessionSetup.SessionCredentials.IsNewPassword);
            }
        }

        private void OnClientConnected(object sender, ClientConnectedArgs e)
        {
            _state.SetState(SessionState.Connected);
        }

        private void OnClientAsyncDisconnect(object sender, ClientAsyncDisconnectArgs e)
        {
            Contract.Assert(sender is IRdpConnection);

            IRdpConnection connection = (IRdpConnection)sender;
            Contract.Assert(object.ReferenceEquals(connection, _connection));

            _state.SetState(SessionState.Idle);

            switch (e.DisconnectReason.Code)
            {
                case RdpDisconnectCode.CertValidationFailed:
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
            Contract.Assert(sender is IRdpConnection);
            Contract.Assert(object.ReferenceEquals(sender, _connection));
            Contract.Ensures(null == _connection);

            using (LockWrite())
            {
                _connection.Events.ClientDisconnected -= this.OnClientDisconnected;
                _connection.Events.ClientConnected -= this.OnClientConnected;
                _connection.Events.ClientAsyncDisconnect -= this.OnClientAsyncDisconnect;
                _connection = null;
            }

            _state.SetDisconnectCode(e.DisconnectReason.Code);

            switch(e.DisconnectReason.Code)
            {
                case RdpDisconnectCode.UserInitiated:
                    _state.SetState(SessionState.Closed);
                    DeferEmitClosed();
                    break;

                default:
                    _state.SetState(SessionState.Failed);
                    DeferEmitFailed(e.DisconnectReason.Code);
                    break;
            }
        }

        private void RequestValidCredentials()
        {
            //
            // Emit an event with a credentials editor task.
            //
            InSessionCredentialsTask task = new InSessionCredentialsTask(_sessionSetup.SessionCredentials, _sessionSetup.DataModel,
                "d:Invalid user name or password");
            task.Submitted += this.NewPasswordSubmitted;
            task.Cancelled += this.NewPasswordCancelled;
            DeferEmitCredentialsNeeded(task);
        }

        private void RequestNewPassword()
        {
            //
            // Emit an event with a credentials editor task.
            //
            InSessionCredentialsTask task = new InSessionCredentialsTask(_sessionSetup.SessionCredentials, _sessionSetup.DataModel,
                "d:Server has requested a new password to be typed in");
            task.Submitted += this.NewPasswordSubmitted;
            task.Cancelled += this.NewPasswordCancelled;
            DeferEmitCredentialsNeeded(task);
        }

        private void MissingCredentialsSubmitted(object sender, InSessionCredentialsTask.SubmittedEventArgs e)
        {
            InSessionCredentialsTask task = (InSessionCredentialsTask)sender;
            task.Submitted -= this.MissingCredentialsSubmitted;
            task.Cancelled -= this.MissingCredentialsCancelled;

            if (e.SaveCredentials)
            {
                _sessionSetup.SaveCredentials();
            }

            InternalStartSession(_sessionSetup);
        }

        private void MissingCredentialsCancelled(object sender, EventArgs e)
        {
            InSessionCredentialsTask task = (InSessionCredentialsTask)sender;
            task.Submitted -= this.MissingCredentialsSubmitted;
            task.Cancelled -= this.MissingCredentialsCancelled;
            //
            // Emit the Cancelled event so the session view model can navigate to the home page
            //
            EmitCancelled();
        }

        private void NewPasswordSubmitted(object sender, InSessionCredentialsTask.SubmittedEventArgs e)
        {
            InSessionCredentialsTask task = (InSessionCredentialsTask)sender;
            task.Submitted -= this.NewPasswordSubmitted;
            task.Cancelled -= this.NewPasswordCancelled;

            if (e.SaveCredentials)
                _sessionSetup.SaveCredentials();
            //
            // Go ahead and try to re-connect with new credentials.
            //
            _state.SetState(SessionState.Connecting);
            _connection.Connect(_sessionSetup.SessionCredentials.Credentials,
                !_sessionSetup.SessionCredentials.IsNewPassword);
        }

        private void NewPasswordCancelled(object sender, EventArgs e)
        {
            InSessionCredentialsTask task = (InSessionCredentialsTask)sender;
            task.Submitted -= this.NewPasswordSubmitted;
            task.Cancelled -= this.NewPasswordCancelled;
            //
            // User has cancelled the credentials dialog, tell the subscribers about the cancellation.
            //
            EmitCancelled();
        }
    }
}
