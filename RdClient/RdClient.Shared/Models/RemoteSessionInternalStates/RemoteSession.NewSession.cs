namespace RdClient.Shared.Models
{
    using RdClient.Shared.ViewModels.EditCredentialsTasks;
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;

    partial class RemoteSession
    {
        private sealed class NewSession : InternalState
        {
            private readonly RemoteSessionSetup _sessionSetup;
            private RemoteSession _session;

            public NewSession(RemoteSessionSetup sessionSetup, ReaderWriterLockSlim _monitor)
                : base(SessionState.Idle, _monitor)
            {
                _sessionSetup = sessionSetup;
            }

            public override void Activate(RemoteSession session)
            {
                Contract.Assert(null == _session);

                _session = session;

                if (_sessionSetup.Connection is DesktopModel)
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
                            _sessionSetup.DataModel, "d:Connection is set up to always ask credentials", null);
                        task.Submitted += this.MissingCredentialsSubmitted;
                        task.Cancelled += this.MissingCredentialsCancelled;

                        _session.EmitCredentialsNeeded(task);
                    }
                    else
                    {
                        _session.InternalStartSession(_sessionSetup);
                    }
                }
                else
                {
                    _session.InternalStartSession(_sessionSetup);
                }
            }

            public override void Deactivate(RemoteSession session)
            {
                Contract.Assert(null == _session);
            }

            public override void Complete(RemoteSession session)
            {
                Contract.Assert(object.ReferenceEquals(_session, session));
                _session = null;
            }

            private void MissingCredentialsSubmitted(object sender, InSessionCredentialsTask.SubmittedEventArgs e)
            {
                Contract.Assert(null != _session);

                InSessionCredentialsTask task = (InSessionCredentialsTask)sender;
                task.Submitted -= this.MissingCredentialsSubmitted;
                task.Cancelled -= this.MissingCredentialsCancelled;

                if (e.SaveCredentials)
                {
                    _sessionSetup.SaveCredentials();
                }

                _session.InternalStartSession(_sessionSetup);
            }

            private void MissingCredentialsCancelled(object sender, InSessionCredentialsTask.ResultEventArgs e)
            {
                Contract.Assert(null != _session);

                InSessionCredentialsTask task = (InSessionCredentialsTask)sender;
                task.Submitted -= this.MissingCredentialsSubmitted;
                task.Cancelled -= this.MissingCredentialsCancelled;
                //
                // Switch to the Cancelled state; the new state will emit Closed state through the sessuion and switch
                // to the Idle state (InactiveSession).
                //
                _session.InternalSetState(new CancelledSession(this));
                //
                // Do nothing to change the internal state of the session; this object will be retained
                // by the session as its internal state and will be asked to re-activate the session.
                //
            }
        }
    }
}
