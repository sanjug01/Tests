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
                            _sessionSetup.DataModel, "d:Connection is set up to always ask credentials");
                        task.Submitted += this.MissingCredentialsSubmitted;
                        task.Cancelled += this.MissingCredentialsCancelled;

                        session.EmitCredentialsNeeded(task);
                    }
                    else
                    {
                        session.InternalStartSession(_sessionSetup);
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

            public override void Deactivate(RemoteSession session)
            {
                Contract.Assert(null == _session);
            }

            public override void Complete(RemoteSession session)
            {
                Contract.Assert(null == _session);
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

                RemoteSession s = _session;
                _session = null;
                s.InternalStartSession(_sessionSetup);
            }

            private void MissingCredentialsCancelled(object sender, EventArgs e)
            {
                Contract.Assert(null != _session);

                InSessionCredentialsTask task = (InSessionCredentialsTask)sender;
                task.Submitted -= this.MissingCredentialsSubmitted;
                task.Cancelled -= this.MissingCredentialsCancelled;
                //
                // Emit the Cancelled event so the session view model can navigate to the home page
                //
                RemoteSession s = _session;
                _session = null;
                s.EmitClosed();
                //
                // Do nothing to change the internal state of the session; this object will be retained
                // by the session as its internal state and will be asked to re-activate the session.
                //
            }
        }
    }
}
