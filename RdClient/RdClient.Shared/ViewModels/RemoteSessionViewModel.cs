﻿namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Data;
    using RdClient.Shared.Input.Keyboard;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    public sealed class RemoteSessionViewModel : ViewModelBase, IRemoteSessionViewSite
    {
        private readonly RelayCommand _dismissFailureMessage;
        private readonly RelayCommand _cancelAutoReconnect;

        private IRemoteSessionView _sessionView;
        private IRemoteSession _activeSession;
        private IRemoteSessionControl _activeSessionControl;
        private IKeyboardCapture _keyboardCapture;
        private SessionState _sessionState;

        private bool _failureMessageVisible;
        private RdpDisconnectCode _failureCode;

        private bool _interrupted;
        private InterruptedSessionContinuation _interruptedContinuation;
        private int _reconnectAttempt;

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

        public bool IsInterrupted
        {
            get { return _interrupted; }
            private set { this.SetProperty(ref _interrupted, value); }
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

        public int ReconnectAttempt
        {
            get { return _reconnectAttempt; }
            private set { this.SetProperty(ref _reconnectAttempt, value); }
        }

        public ICommand CancelAutoReconnect
        {
            get { return _cancelAutoReconnect; }
        }

        public IKeyboardCapture KeyboardCapture
        {
            get { return _keyboardCapture; }
            set { this.SetProperty<IKeyboardCapture>(ref _keyboardCapture, value); }
        }

        public RemoteSessionViewModel()
        {
            _dismissFailureMessage = new RelayCommand(this.InternalDismissFailureMessage);
            _cancelAutoReconnect = new RelayCommand(this.InternalCancelAutoReconnect, this.InternalCanAutoReconnect);
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(null == _activeSession);
            Contract.Assert(null != _keyboardCapture);

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
            _sessionState = _activeSession.State.State;

            _activeSession.CredentialsNeeded += this.OnCredentialsNeeded;
            _activeSession.Closed += this.OnSessionClosed;
            _activeSession.Failed += this.OnSessionFailed;
            _activeSession.Interrupted += this.OnSessionInterrupted;
            _activeSession.BadCertificate += this.OnBadCertificate;
            _activeSession.State.PropertyChanged += this.OnSessionStatePropertyChanged;

            if (null != _sessionView && SessionState.Idle == _sessionState)
            {
                Contract.Assert(null == _activeSessionControl);
                _activeSessionControl = _activeSession.Activate(_sessionView);
            }

            this.IsInterrupted = SessionState.Interrupted == _activeSession.State.State;
            this.IsFailureMessageVisible = SessionState.Failed == _activeSession.State.State;
        }

        protected override void OnDismissed()
        {
            _activeSession.CredentialsNeeded -= this.OnCredentialsNeeded;
            _activeSession.Closed -= this.OnSessionClosed;
            _activeSession.Failed -= this.OnSessionFailed;
            _activeSession.Interrupted -= this.OnSessionInterrupted;
            _activeSession.BadCertificate -= this.OnBadCertificate;
            _activeSession.State.PropertyChanged -= this.OnSessionStatePropertyChanged;
            _sessionView.RecycleRenderingPanel(_activeSession.Deactivate());
            _activeSessionControl = null;
            _activeSession = null;
            _sessionView = null;
            _failureMessageVisible = false;
            _interrupted = false;

            base.OnDismissed();
        }

        void IRemoteSessionViewSite.SetRemoteSessionView(IRemoteSessionView sessionView)
        {
            Contract.Assert((null == sessionView && null != _sessionView) || (null != sessionView && null == _sessionView));

            _sessionView = sessionView;

            if (null != _sessionView && null != _activeSession && SessionState.Idle == _activeSession.State.State)
            {
                Contract.Assert(null == _activeSessionControl);
                _activeSessionControl = _activeSession.Activate(_sessionView);
            }
        }

        private void OnCredentialsNeeded(object sender, CredentialsNeededEventArgs e)
        {
            this.NavigationService.PushModalView("InSessionEditCredentialsView", e.Task);
        }

        private void OnSessionFailed(object sender, SessionFailureEventArgs e)
        {
            //
            // Show the failure UI
            //
            this.FailureCode = e.DisconnectCode;
            this.IsFailureMessageVisible = true;
        }

        private void OnSessionInterrupted(object sender, SessionInterruptedEventArgs e)
        {
            Contract.Assert(null == _interruptedContinuation);

            _interruptedContinuation = e.ObtainContinuation();
            _cancelAutoReconnect.EmitCanExecuteChanged();
            this.ReconnectAttempt = _activeSession.State.ReconnectAttempt;
            this.IsInterrupted = true;
            EmitPropertyChanged("IsConnected");
        }

        private sealed class CertificateCompletion : IPresentationCompletion
        {
            private readonly ICertificateValidation _validation;
            private readonly ICertificateTrust _permanentTrust, _sessionTrust;

            public CertificateCompletion(ICertificateValidation validation, ICertificateTrust permanentTrust, ICertificateTrust sessionTrust)
            {
                Contract.Requires(null != validation);
                Contract.Assert(null != permanentTrust);
                Contract.Assert(null != sessionTrust);
                Contract.Ensures(null != _validation);
                Contract.Ensures(null != permanentTrust);
                Contract.Ensures(null != sessionTrust);

                _validation = validation;
                _permanentTrust = permanentTrust;
                _sessionTrust = sessionTrust;
            }

            void IPresentationCompletion.Completed(IPresentableView view, object result)
            {
                Contract.Assert(result is CertificateValidationResult);

                CertificateValidationResult r = (CertificateValidationResult)result;

                switch(r.Result)
                {
                    case CertificateValidationResult.CertificateTrustLevel.AcceptedAlways:
                        _permanentTrust.TrustCertificate(_validation.Certificate);
                        _validation.Accept();
                        break;

                    case CertificateValidationResult.CertificateTrustLevel.AcceptedOnce:
                        _sessionTrust.TrustCertificate(_validation.Certificate);
                        _validation.Accept();
                        break;

                    default:
                        _validation.Reject();
                        break;
                }
            }
        }

        private void OnBadCertificate(object sender, BadCertificateEventArgs e)
        {
            Contract.Assert(sender is IRemoteSession);

            ICertificateValidation validation = e.ObtainValidation();
            IRdpCertificate certificate = validation.Certificate;
            IRemoteSession session = (IRemoteSession)sender;

            if(session.CertificateTrust.IsCertificateTrusted(certificate) || this.ApplicationDataModel.CertificateTrust.IsCertificateTrusted(certificate))
            {
                validation.Accept();
            }
            else
            {
                CertificateCompletion completion = new CertificateCompletion(validation, this.ApplicationDataModel.CertificateTrust, session.CertificateTrust);
                CertificateValidationViewModelArgs args = new CertificateValidationViewModelArgs("Don Pedro", validation.Certificate);

                this.NavigationService.PushModalView("CertificateValidationView", args, completion);
            }
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
                switch(_activeSession.State.State)
                {
                    case SessionState.Connected:
                        //
                        // If the session has been interrupted but reconnected automatically, clear the IsInterrupted flag
                        // to remote any interruption UI.
                        //
                        _interruptedContinuation = null;
                        this.IsInterrupted = false;
                        _cancelAutoReconnect.EmitCanExecuteChanged();
                        _keyboardCapture.Keystroke += this.OnKeystroke;
                        _keyboardCapture.Start();
                        EmitPropertyChanged("IsConnected");
                        break;

                    default:
                        if (SessionState.Connected == _sessionState)
                        {
                            _keyboardCapture.Stop();
                            _keyboardCapture.Keystroke -= this.OnKeystroke;
                            EmitPropertyChanged("IsConnected");
                        }
                        break;
                }

                _sessionState = _activeSession.State.State;
            }
            else if(e.PropertyName.EndsWith("ReconnectAttempt"))
            {
                this.ReconnectAttempt = _activeSession.State.ReconnectAttempt;
            }
        }

        private void InternalDismissFailureMessage(object parameter)
        {
            Contract.Assert(SessionState.Failed == _activeSession.State.State);

            this.IsFailureMessageVisible = false;
            this.NavigationService.NavigateToView("ConnectionCenterView", null);
        }

        private void InternalCancelAutoReconnect(object parameter)
        {
            Contract.Assert(null != _interruptedContinuation);

            _interruptedContinuation.Cancel();
            _interruptedContinuation = null;
            _cancelAutoReconnect.EmitCanExecuteChanged();
            this.IsInterrupted = false;
        }

        private bool InternalCanAutoReconnect(object parameter)
        {
            return null != _interruptedContinuation;
        }

        private void OnKeystroke(object sender, KeystrokeEventArgs e)
        {
            Contract.Assert(null != _activeSessionControl);
            _activeSessionControl.SendKeystroke(e.KeyCode, e.IsScanCode, e.IsExtendedKey, e.IsKeyReleased);
        }
    }
}
