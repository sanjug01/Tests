namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.Models;
using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Windows.Input;

    public sealed class RemoteSessionViewModel : ViewModelBase, IRemoteSessionViewSite
    {
        private IRemoteSessionView _sessionView;
        private IRemoteSession _activeSession;

        private bool _failureMessageVisible;
        private RdpDisconnectCode _failureCode;
        private RelayCommand _dismissFailureMessage;

        public bool IsConnected
        {
            get
            {
                return null != _activeSession && SessionState.Connected == _activeSession.State.State;
            }
        }

        public bool IsFailureMessageVisible
        {
            get { return _failureMessageVisible; }
            private set { this.SetProperty(ref _failureMessageVisible, value); }
        }

        public RdpDisconnectCode FailureCode
        {
            get { return _failureCode; }
            private set { this.SetProperty(ref _failureCode, value); }
        }

        public ICommand DismissFailureMessage
        {
            get { return _dismissFailureMessage; }
        }

        public RemoteSessionViewModel()
        {
            _dismissFailureMessage = new RelayCommand(this.InternalDismissFailureMessage);
        }

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
            _activeSession.Closed += this.OnSessionClosed;
            _activeSession.Failed += this.OnSessionFailed;
            _activeSession.State.PropertyChanged += this.OnSessionStatePropertyChanged;

            if (null != _sessionView)
            {
                _activeSession.Activate(_sessionView);
            }
        }

        protected override void OnDismissed()
        {
            _activeSession.CredentialsNeeded -= this.OnCredentialsNeeded;
            _activeSession.Cancelled -= this.OnSessionCancelled;
            _activeSession.Closed -= this.OnSessionClosed;
            _activeSession.Failed -= this.OnSessionFailed;
            _activeSession.State.PropertyChanged -= this.OnSessionStatePropertyChanged;
            //
            // TODO: detach the rendering panel from _activeSession
            //
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
            this.NavigationService.PushModalView("InSessionEditCredentialsView", e.Task);
        }

        private void OnSessionCancelled(object sender, EventArgs e)
        {
            this.NavigationService.NavigateToView("ConnectionCenterView", null);
        }

        private void OnSessionFailed(object sender, SessionFailureEventArgs e)
        {
            //
            // Show the failure UI
            //
            this.FailureCode = e.DisconnectCode;
            this.IsFailureMessageVisible = true;
        }

        private void OnSessionClosed(object sender, EventArgs e)
        {
            this.NavigationService.NavigateToView("ConnectionCenterView", null);
        }

        private void OnSessionStatePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName.Equals("State"))
            {
                //
                // Session state has changed
                //
                EmitPropertyChanged("IsConnected");
            }
        }

        private void InternalDismissFailureMessage(object parameter)
        {
            Contract.Assert(SessionState.Closed == _activeSession.State.State);

            _failureMessageVisible = false;
            this.NavigationService.NavigateToView("ConnectionCenterView", null);
        }
    }
}
