namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Input.Pointer;
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Input.Keyboard;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models.PanKnobModel;

    public sealed class RemoteSessionViewModel : DeferringViewModelBase, IRemoteSessionViewSite, ITimerFactorySite, IDeviceCapabilitiesSite
    {
        private readonly RelayCommand _dismissFailureMessage;
        private readonly RelayCommand _cancelAutoReconnect;
        private readonly RelayCommand _showSideBars;
        private readonly RelayCommand _invokeKeyboard;
        private readonly SymbolBarToggleButtonModel _invokeKeyboardModel;
        private readonly RelayCommand _navigateHome;
        private readonly RelayCommand _mouseMode;

        //
        // Input Pane is a fancy name for the touch keyboard.
        //
        private IInputPanel _inputPanel;

        private IRemoteSessionView _sessionView;
        private IRemoteSession _activeSession;
        private IRemoteSessionControl _activeSessionControl;
        private IKeyboardCapture _keyboardCapture;
        private IInputPanelFactory _inputPanelFactory;
        private IPointerCapture _pointerCapture;
        private SessionState _sessionState;
        private bool _isConnectionBarVisible;
        private readonly ReadOnlyObservableCollection<object> _connectionBarItems;
        private bool _isRightSideBarVisible;

        private bool _failureMessageVisible;
        private RdpDisconnectCode _failureCode;

        private bool _interrupted;
        private InterruptedSessionContinuation _interruptedContinuation;
        private int _reconnectAttempt;

        private ITimerFactory _timerFactory;
        public ITimerFactory TimerFactory { get { return _timerFactory; } }

        private readonly PointerPosition _pointerPosition = new PointerPosition();
        private readonly ConsumptionModeTracker _consumptionMode = new ConsumptionModeTracker();

        private readonly ZoomPanMultiTouchModel _zoomPanModel = new ZoomPanMultiTouchModel();

        private IPanKnobSite _panKnobSite;
        public IPanKnobSite PanKnobSite
        {
            get
            {
                return _panKnobSite;
            }

            set
            {
                this.SetProperty(ref _panKnobSite, value);
            }
        }

        public bool IsConnecting
        {
            get
            {
                return null != _activeSession
                    && (SessionState.Connecting == _activeSession.State.State);
            }
        }

        public bool IsRenderingPanelActive
        {
            get
            {
                return null != _activeSession
                    && (SessionState.Connected == _activeSession.State.State || SessionState.Connecting == _activeSession.State.State);
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

        /// <summary>
        /// Property into that the keyboard capture object is injected in XAML.
        /// </summary>
        public IKeyboardCapture KeyboardCapture
        {
            set { _keyboardCapture = value; }
        }

        /// <summary>
        /// Property into that a factory of input panels is injected in XAML.
        /// </summary>
        public IInputPanelFactory InputPanelFactory
        {
            set { _inputPanelFactory = value; }
        }

        public IPointerCapture PointerCapture
        {
            get { return _pointerCapture; }
            set { this.SetProperty(ref _pointerCapture, value); }
        }

        public bool IsConnectionBarVisible
        {
            get { return _isConnectionBarVisible; }
            private set { this.SetProperty(ref _isConnectionBarVisible, value); }
        }

        public ReadOnlyObservableCollection<object> ConnectionBarItems
        {
            get { return _connectionBarItems; }
        }

        public ICommand ShowSideBars
        {
            get { return _showSideBars; }
        }

        public bool IsRightSideBarVisible
        {
            get { return _isRightSideBarVisible; }
            set { this.SetProperty(ref _isRightSideBarVisible, value); }
        }

        public ICommand NavigateHome
        {
            get { return _navigateHome; }
        }

        public ICommand MouseMode
        {
            get { return _mouseMode; }
        }

        public RemoteSessionViewModel()
        {
            _dismissFailureMessage = new RelayCommand(this.InternalDismissFailureMessage);
            _cancelAutoReconnect = new RelayCommand(this.InternalCancelAutoReconnect, this.InternalCanAutoReconnect);
            _showSideBars = new RelayCommand(this.InternalShowRightSideBar);
            _invokeKeyboard = new RelayCommand(this.InternalInvokeKeyboard, this.InternalCanInvokeKeyboard);
            _invokeKeyboardModel = new SymbolBarToggleButtonModel() { Glyph = SegoeGlyph.Keyboard, Command = _invokeKeyboard };
            _navigateHome = new RelayCommand(this.InternalNavigateHome);
            _mouseMode = new RelayCommand(this.InternalMouseMode);
            _isRightSideBarVisible = false;

            ObservableCollection<object> items = new ObservableCollection<object>();
            items.Add(new SymbolBarButtonModel() { Glyph = SegoeGlyph.ZoomIn, Command = _zoomPanModel.ZoomInCommand });
            items.Add(new SymbolBarButtonModel() { Glyph = SegoeGlyph.ZoomOut, Command = _zoomPanModel.ZoomOutCommand });
            items.Add(new SymbolBarButtonModel() { Glyph = SegoeGlyph.HorizontalEllipsis, Command = _showSideBars });
            items.Add(_invokeKeyboardModel);
            _connectionBarItems = new ReadOnlyObservableCollection<object>(items);
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(null == _activeSession);
            Contract.Assert(null != _keyboardCapture);
            Contract.Assert(null != _inputPanelFactory);

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
            _activeSession.BadServerIdentity += this.OnBadServerIdentity;
            _activeSession.State.PropertyChanged += this.OnSessionStatePropertyChanged;

            if (null != _sessionView && SessionState.Idle == _sessionState)
            {
                Contract.Assert(null == _activeSessionControl);
                
                _activeSessionControl = _activeSession.Activate(_sessionView);
            }

            EmitPropertyChanged("IsRenderingPanelActive");
            EmitPropertyChanged("IsConnecting");
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
            _activeSession.BadServerIdentity -= this.OnBadServerIdentity;
            _activeSession.State.PropertyChanged -= this.OnSessionStatePropertyChanged;
            _sessionView.RecycleRenderingPanel(_activeSession.Deactivate());
            _activeSessionControl = null;
            _activeSession = null;
            _sessionView = null;
            _failureMessageVisible = false;
            _interrupted = false;
            _interruptedContinuation = null;

            base.OnDismissed();
        }

        protected override void OnNavigatingBack(IBackCommandArgs backArgs)
        {
            if (this.IsFailureMessageVisible)
            {
                this.DismissFailureMessage.Execute(null);
            }
            else
            {
                this.NavigateHome.Execute(null);
            }
            backArgs.Handled = true;
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

        void ITimerFactorySite.SetTimerFactory(ITimerFactory timerFactory)
        {
            _timerFactory = timerFactory;
        }

        void IDeviceCapabilitiesSite.SetDeviceCapabilities(IDeviceCapabilities deviceCapabilities)
        {
            if(null != _inputPanel)
            {
                _inputPanel.IsVisibleChanged -= this.OnInputPanelIsVisibleChanged;
                _inputPanel = null;
            }

            if (null != deviceCapabilities && deviceCapabilities.TouchPresent)
            {
                _inputPanel = _inputPanelFactory.GetInputPanel();
                _inputPanel.IsVisibleChanged += this.OnInputPanelIsVisibleChanged;
                _invokeKeyboardModel.IsChecked = _inputPanel.IsVisible;
            }

            _invokeKeyboard.EmitCanExecuteChanged();
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
            EmitPropertyChanged("IsRenderingPanelActive");
            EmitPropertyChanged("IsConnecting");
        }

        private void OnBadCertificate(object sender, BadCertificateEventArgs e)
        {
            Contract.Assert(sender is IRemoteSession);

            ICertificateValidation validation = e.ObtainValidation();
            IRdpCertificate certificate = validation.Certificate;
            IRemoteSession session = (IRemoteSession)sender;

            if(session.CertificateTrust.IsCertificateTrusted(certificate) || this.ApplicationDataModel.CertificateTrust.IsCertificateTrusted(certificate))
            {
                //
                // The certificate is in one of the collections of trusted certificates, simply accept the certificate and let the session proceed.
                //
                validation.Accept();
            }
            else
            {
                //
                // Present the certificate validation modal dialog that will use the validation object to accept of reject the certificate;
                // also, the completion object pased to the dialog will add the certificate to one of the certificate trust objects.
                //
                this.NavigationService.PushModalView("CertificateValidationView",
                    new CertificateValidationViewModelArgs(session.HostName, validation.Certificate),
                    new CertificateValidationCompletion(validation, this.ApplicationDataModel.CertificateTrust, session.CertificateTrust));
            }
        }

        private void OnBadServerIdentity(object sender, BadServerIdentityEventArgs e)
        {
            Contract.Assert(sender is IRemoteSession);

            IServerIdentityValidation validation = e.ObtainValidation();
            IRemoteSession session = (IRemoteSession)sender;

            if (session.IsServerTrusted
                || validation.Desktop.IsTrusted)
            {
                // previously accepted - do not ask again
                validation.Accept();
            }
            else
            {
                // Prompt user to trust or not the server
                this.NavigationService.PushModalView("DesktopIdentityValidationView",
                    new DesktopIdentityValidationViewModelArgs(session.HostName),
                    new DesktopIdentityValidationCompletion(validation)
                    );
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
                    case SessionState.Connecting:
                        _interruptedContinuation = null;
                        this.IsInterrupted = false;
                        _cancelAutoReconnect.EmitCanExecuteChanged();
                        EmitPropertyChanged("IsRenderingPanelActive");
                        EmitPropertyChanged("IsConnecting");
                        this.IsConnectionBarVisible = false;
                        break;

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
                        _pointerPosition.Reset(_activeSessionControl, this);
                        _zoomPanModel.Reset(_activeSessionControl.RenderingPanel.Viewport, _pointerPosition, _timerFactory.CreateTimer(), this.ExecutionDeferrer);
                        this.PointerCapture = new PointerCapture(_pointerPosition, _activeSessionControl, _activeSessionControl.RenderingPanel, _timerFactory);
                        this.PointerCapture.ConsumptionMode.ConsumptionModeChanged += _zoomPanModel.OnConsumptionModeChanged;
                        _activeSession.MouseCursorShapeChanged += this.PointerCapture.OnMouseCursorShapeChanged;
                        _activeSession.MultiTouchEnabledChanged += this.PointerCapture.OnMultiTouchEnabledChanged;
                        _activeSessionControl.RenderingPanel.PointerChanged += this.PointerCapture.OnPointerChanged;
                        this.PanKnobSite = new PanKnobSite(this.TimerFactory);
                        _panKnobSite.Viewport = _activeSessionControl.RenderingPanel.Viewport;
                        _panKnobSite.OnConsumptionModeChanged(this, _pointerCapture.ConsumptionMode.ConsumptionMode);
                        this.PointerCapture.ConsumptionMode.ConsumptionModeChanged += _panKnobSite.OnConsumptionModeChanged;
                        EmitPropertyChanged("IsRenderingPanelActive");
                        EmitPropertyChanged("IsConnecting");
                        this.IsConnectionBarVisible = true;
                        break;

                    default:
                        if (SessionState.Connected == _sessionState)
                        {
                            _keyboardCapture.Stop();
                            _keyboardCapture.Keystroke -= this.OnKeystroke;
                            _activeSession.MouseCursorShapeChanged -= this.PointerCapture.OnMouseCursorShapeChanged;
                            _activeSession.MultiTouchEnabledChanged -= this.PointerCapture.OnMultiTouchEnabledChanged;
                            _activeSessionControl.RenderingPanel.PointerChanged -= this.PointerCapture.OnPointerChanged;
                            EmitPropertyChanged("IsRenderingPanelActive");
                            EmitPropertyChanged("IsConnecting");
                            //
                            // The connection bar and side bars are not available in any non-connected state.
                            //
                            this.IsConnectionBarVisible = false;
                            this.IsRightSideBarVisible = false;
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

        private void InternalShowRightSideBar(object parameter)
        {
            this.IsRightSideBarVisible = !this.IsRightSideBarVisible;
        }

        private void InternalInvokeKeyboard(object parameter)
        {
            Contract.Assert(null != _inputPanel);

            if (_inputPanel.IsVisible)
                _inputPanel.Hide();
            else
                _inputPanel.Show();
        }

        private bool InternalCanInvokeKeyboard(object parameter)
        {
            return null != _inputPanel;
        }

        private void OnInputPanelIsVisibleChanged(object sender, EventArgs e)
        {
            IInputPanel panel = (IInputPanel)sender;
            _invokeKeyboardModel.IsChecked = panel.IsVisible;
        }

        private void InternalNavigateHome(object parameter)
        {
            this.IsRightSideBarVisible = false;
            _activeSession.Disconnect();
        }

        private void InternalMouseMode(object parameter)
        {
            if(this.PointerCapture != null)
            {
                this.PointerCapture.OnMouseModeChanged(this, EventArgs.Empty);
            }
        }
    }
}
