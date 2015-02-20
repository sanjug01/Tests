﻿namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.ViewModels;
    using RdClient.Shared.ViewModels.EditCredentialsTasks;
    using System;
    using System.Diagnostics.Contracts;

    public sealed class RemoteSession : MutableObject, IRemoteSession
    {
        private readonly RemoteSessionSetup _sessionSetup;
        private readonly IDeferredExecution _deferredExecution;
        private readonly IRdpConnectionSource _connectionSource;
        private readonly ICertificateTrust _certificateTrust;

        private EventHandler<CredentialsNeededEventArgs> _credentialsNeeded;
        private EventHandler _cancelled;

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

            _sessionSetup = sessionSetup;
            _deferredExecution = deferredExecution;
            _connectionSource = connectionSource;
            _certificateTrust = new CertificateTrust();
        }

        ICertificateTrust IRemoteSession.CertificateTrust
        {
            get { return _certificateTrust; }
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

            if (null == _connection)
            {
                //
                // Ask the connection source to create a new session.
                // The connection source comes all the way from XAML of the main page.
                //
                //_connection = _connectionSource.CreateConnection(_renderingPanel);

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
                        AlwaysAskCredentialsTask task = new AlwaysAskCredentialsTask(dtm.HostName, new CredentialsModel(), _sessionSetup.DataModel);
                        task.Submitted += this.SessionCredentialsSubmitted;
                        task.Cancelled += this.SessionCredentialsCancelled;
                        EmitCredentialsNeeded(task);
                    }
                    else
                    {
                        Contract.Assert(null != _sessionSetup.Credentials);
                        InternalStartSession(_sessionSetup.Connection, _sessionSetup.Credentials, true);
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

            return new RemoteSessionControl(_connection);
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
            //
            // Recycle the rendering panel;
            //
            _sessionView.RecycleRenderingPanel(_renderingPanel);
            _renderingPanel = null;
            _sessionView = null;
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

        private void SessionCredentialsSubmitted(object sender, SessionCredentialsEventArgs e)
        {
            Contract.Assert(sender is AlwaysAskCredentialsTask);

            AlwaysAskCredentialsTask task = (AlwaysAskCredentialsTask)sender;
            task.Cancelled -= this.SessionCredentialsCancelled;
            task.Submitted -= this.SessionCredentialsSubmitted;

            if (e.UserWantsToSavePassword)
            {
                Guid credentialsId = e.CredentialsId;
                //
                // Save credentials;
                //
                if(credentialsId.Equals(Guid.Empty))
                {
                    //
                    // Returned credentials must be inserted in the data model;
                    //
                    credentialsId = _sessionSetup.DataModel.LocalWorkspace.Credentials.AddNewModel(e.Credentials);
                }
                else
                {
                    //
                    // Original credentials may simply be updated;
                    //
                }

                _sessionSetup.SetCredentials(e.Credentials);

                if (_sessionSetup.Connection is DesktopModel)
                {
                    ((DesktopModel)_sessionSetup.Connection).CredentialsId = credentialsId;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            InternalStartSession(_sessionSetup.Connection, e.Credentials, e.IsPasswordChanged);
        }

        private void SessionCredentialsCancelled(object sender, EventArgs e)
        {
            Contract.Assert(sender is AlwaysAskCredentialsTask);

            AlwaysAskCredentialsTask task = (AlwaysAskCredentialsTask)sender;
            task.Cancelled -= this.SessionCredentialsCancelled;
            task.Submitted -= this.SessionCredentialsSubmitted;
            //
            // Emit the Cancelled event so the session view model can navigate to the home page
            //
            EmitCancelled();
        }

        private void InternalStartSession(RemoteConnectionModel connection, CredentialsModel credentials, bool savedPassword)
        {
            //
            //
            //
        }
    }
}
