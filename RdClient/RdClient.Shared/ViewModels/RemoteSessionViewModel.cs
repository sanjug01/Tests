namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Models;
    using System;
    using System.Diagnostics.Contracts;

    public sealed class RemoteSessionViewModel : ViewModelBase, IRemoteSessionViewSite
    {
        private IRemoteSessionView _sessionView;
        private IRemoteSession _activeSession;

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(null == _activeSession);

            base.OnPresenting(activationParameter);

            Contract.Assert(null != activationParameter, "Cannot navigate to remote session without activation parameter");
            Contract.Assert(activationParameter is IRemoteSession, "Remote session view model activation parameter is not IRemoteSession");
            //
            // TODO:    if the session view model had obtained the session view for the session, it may activate
            //          the session with that view. Otherwise it should just remember the session and wait for
            //          the view.
            //
            IRemoteSession newSession = (IRemoteSession)activationParameter;

            _activeSession = newSession;
            _activeSession.CredentialsNeeded += this.OnCredentialsNeeded;
            _activeSession.Cancelled += this.OnSessionCancelled;

            if (null != _sessionView)
            {
                _activeSession.Activate(_sessionView);
            }
        }

        protected override void OnDismissed()
        {
            _activeSession.CredentialsNeeded -= this.OnCredentialsNeeded;
            _activeSession.Cancelled -= this.OnSessionCancelled;
            _activeSession = null;
            _sessionView = null;

            base.OnDismissed();
        }

        void IRemoteSessionViewSite.SetRemoteSessionView(IRemoteSessionView sessionView)
        {
            Contract.Assert((null == sessionView && null != _sessionView) || (null != sessionView && null == _sessionView));

            _sessionView = sessionView;

            if(null != _sessionView && null != _activeSession)
            {
                _activeSession.Activate(_sessionView);
            }
        }

        private void OnCredentialsNeeded(object sender, CredentialsNeededEventArgs e)
        {
            this.NavigationService.PushModalView("EditCredentialsView", e.Task);
        }

        private void OnSessionCancelled(object sender, EventArgs e)
        {
            this.NavigationService.NavigateToView("ConnectionCenterView", null);
        }
    }
}
