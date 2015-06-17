namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Input.Keyboard;
    using RdClient.Shared.Input.Pointer;
    using RdClient.Shared.LifeTimeManagement;
    using RdClient.Shared.Models;
    using RdClient.Shared.Models.PanKnobModel;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using Windows.UI.ViewManagement;
    using Windows.UI.Xaml;

    public sealed class RemoteSessionViewModel : DeferringViewModelBase,
        IRemoteSessionViewSite,
        ITimerFactorySite,
        IDeviceCapabilitiesSite,
        ILifeTimeSite,
        IInputPanelFactorySite
    {
        private readonly RelayCommand _invokeKeyboard;
        private readonly SymbolBarButtonModel _invokeKeyboardModel;

        public ZoomPanModel ZoomPanModel
        {
            private get; set;
        }

        //
        // Device capabilities objecvt injected by the navigation service through IDeviceCapabilitiesSite.
        //
        private IDeviceCapabilities _deviceCapabilities;
        //
        // Input Pane is another name for the touch keyboard.
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
        private ReadOnlyObservableCollection<object> _connectionBarItems;

        private ITimerFactory _timerFactory;

        private ILifeTimeManager _lifeTimeManager;

        public IPointerPosition PointerPosition
        {
            private get; set;
        }

        public IScrollBarModel ScrollBarModel
        {
            get; set;
        }

        public IRightSideBarViewModel RightSideBarViewModel
        {
            get; set;
        }

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

        IDeferredExecution IRemoteSessionViewSite.DeferredExecution
        {
            get { return this.Dispatcher; }
        }

        private IBellyBandViewModel _bellyBandViewModel;

        public SessionState SessionState
        {
            get { return _sessionState; }
            private set { SetProperty(ref _sessionState, value); }
        }

        public ITimerFactory TimerFactory
        {
            get { return _timerFactory; }
        }

        /// <summary>
        /// Property into that the keyboard capture object is injected in XAML.
        /// </summary>
        public IKeyboardCapture KeyboardCapture
        {
            set { _keyboardCapture = value; }
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
        /// <summary>
        /// View model of the view shown in the belly band across the session view when input is needed from user.
        /// Setting the property to a non-null value shows the belly band.
        /// </summary>
        public IBellyBandViewModel BellyBandViewModel
        {
            get { return _bellyBandViewModel; }
            private set { this.SetProperty(ref _bellyBandViewModel, value); }
        }

        private IFullScreenModel _fullScreenModel;
        public IFullScreenModel FullScreenModel
        {
            private get
            {
                return _fullScreenModel;
            }
            set
            {
                _fullScreenModel = value;
            }
        }

        public RemoteSessionViewModel()
        {
            _invokeKeyboard = new RelayCommand(this.InternalInvokeKeyboard, this.InternalCanInvokeKeyboard);
            _invokeKeyboardModel = new SymbolBarButtonModel() { Glyph = SegoeGlyph.Keyboard, Command = _invokeKeyboard };
            _sessionState = SessionState.Idle;

            this.ZoomPanModel = new ZoomPanModel();
            
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(null == _activeSession);
            Contract.Assert(null != _keyboardCapture);
            Contract.Assert(null != _inputPanelFactory);

            ObservableCollection<object> items = new ObservableCollection<object>();
            items.Add(new SymbolBarButtonModel() { Glyph = SegoeGlyph.ZoomIn, Command = this.ZoomPanModel.ZoomInCommand });
            items.Add(new SymbolBarButtonModel() { Glyph = SegoeGlyph.ZoomOut, Command = this.ZoomPanModel.ZoomOutCommand });
            items.Add(new SymbolBarButtonModel() { Glyph = SegoeGlyph.More, Command = this.RightSideBarViewModel.ToggleVisiblity });
            items.Add(_invokeKeyboardModel);
            _connectionBarItems = new ReadOnlyObservableCollection<object>(items);

            base.OnPresenting(activationParameter);

            _inputPanel = _inputPanelFactory.GetInputPanel();

            Contract.Assert(null != activationParameter, "Cannot navigate to remote session without activation parameter");
            Contract.Assert(activationParameter is IRemoteSession, "Remote session view model activation parameter is not IRemoteSession");
            //
            // TODO:    if the session view model had obtained the session view for the session, it may activate
            //          the session with that view. Otherwise it should just remember the session and wait for
            //          the view.
            //
            IRemoteSession newSession = (IRemoteSession)activationParameter;

            _activeSession = newSession;
            this.SessionState = _activeSession.State.State;
            
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

            _lifeTimeManager.Suspending += OnAppSuspending;
            _lifeTimeManager.Resuming += OnAppResuming;

            _invokeKeyboard.EmitCanExecuteChanged();
        }

        protected override void OnDismissed()
        {
            _lifeTimeManager.Resuming -= OnAppResuming;
            _lifeTimeManager.Suspending -= OnAppSuspending;

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
            _inputPanel = null;

            //
            // TODO:    restore the belly band view model from the session state.
            // TFS:     2679737
            //
            this.BellyBandViewModel = null;

            base.OnDismissed();
        }

        protected override void OnNavigatingBack(IBackCommandArgs backArgs)
        {
            this.BellyBandViewModel?.Terminate();
            this.BellyBandViewModel = null;
            this.RightSideBarViewModel.Disconnect.Execute(null);
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
            if (null != _deviceCapabilities)
            {
                _deviceCapabilities.PropertyChanged -= this.OnDeviceCapabilitiesPropertyChanged;
            }

            _deviceCapabilities = deviceCapabilities;

            if (null != _deviceCapabilities)
            {
                _deviceCapabilities.PropertyChanged += this.OnDeviceCapabilitiesPropertyChanged;
            }

            _invokeKeyboard.EmitCanExecuteChanged();
        }


        void ILifeTimeSite.SetLifeTimeManager(ILifeTimeManager lifeTimeManager)
        {
            _lifeTimeManager = lifeTimeManager;
        }

        private void OnCredentialsNeeded(object sender, CredentialsNeededEventArgs e)
        {
            this.NavigationService.PushModalView("InSessionEditCredentialsView", e.Task);
        }

        private void OnSessionFailed(object sender, SessionFailureEventArgs e)
        {
            
            //Restore the local mouse
            this._activeSessionControl.RenderingPanel.ChangeMouseVisibility(Visibility.Collapsed);
            //
            // Show the failure UI
            //
            this.BellyBandViewModel = new RemoteSessionFailureViewModel(e.DisconnectCode,()=>
            {
                Contract.Assert(SessionState.Failed == _activeSession.State.State);

                this.BellyBandViewModel = null;
                this.NavigationService.NavigateToView("ConnectionCenterView", null);
            });
        }

        private void OnSessionInterrupted(object sender, SessionInterruptedEventArgs e)
        {
            this.BellyBandViewModel = new RemoteSessionInterruptionViewModel(_activeSession, e.ObtainContinuation());
            this.IsConnectionBarVisible = true;
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
                    new CertificateValidationViewModelArgs(e.HostName, validation.Certificate),
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
            if(_activeSessionControl != null && _activeSessionControl.RenderingPanel != null)
            {
                _activeSessionControl.RenderingPanel.ChangeMouseVisibility(Visibility.Collapsed);
            }

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
                        Contract.Assert(null == this.BellyBandViewModel);
                        this.BellyBandViewModel = new RemoteSessionConnectingViewModel(
                            _activeSession.HostName,
                            () => _activeSession.Disconnect() );
                        this.IsConnectionBarVisible = false;
                        this.RightSideBarViewModel.Visibility = Visibility.Collapsed;
                        break;

                    case SessionState.Connected:

                        //
                        // Remove any belly-band view that may be shown (transitioning from reconnect or connecting state)
                        //
                        this.BellyBandViewModel = null;
                        this.RightSideBarViewModel.RemoteSession = _activeSession;

                        //
                        // If the session has been interrupted but reconnected automatically, clear the IsInterrupted flag
                        // to remote any interruption UI.
                        //
                        _keyboardCapture.Keystroke += this.OnKeystroke;
                        _keyboardCapture.Start();

                        this.PointerPosition.Reset(_activeSessionControl.RenderingPanel, this);
                        _activeSessionControl.RenderingPanel.Viewport.Reset();

                        this.RightSideBarViewModel.PropertyChanged += OnRightSideBarPropertyChanged;

                        this.PointerCapture = new PointerCapture(
                            this.PointerPosition, 
                            _activeSessionControl, 
                            _activeSessionControl.RenderingPanel, 
                            _timerFactory,
                            this.Dispatcher);

                        this.RightSideBarViewModel.PointerCapture = this.PointerCapture;

                        this.PanKnobSite = new PanKnobSite(this.TimerFactory);

                        this.ZoomPanModel.Reset(_activeSessionControl.RenderingPanel.Viewport);
                        this.ScrollBarModel.Viewport = _activeSessionControl.RenderingPanel.Viewport;

                        this.PanKnobSite.Viewport = _activeSessionControl.RenderingPanel.Viewport;
                        this.PanKnobSite.OnConsumptionModeChanged(this, _pointerCapture.ConsumptionMode.ConsumptionMode);
                        this.PanKnobSite.Reset();


                        this.PointerCapture.ConsumptionMode.ConsumptionModeChanged += _panKnobSite.OnConsumptionModeChanged;
                        this.PointerCapture.ConsumptionMode.ConsumptionModeChanged += this.ZoomPanModel.OnConsumptionModeChanged;
                        this.PointerCapture.ConsumptionMode.ConsumptionMode = ConsumptionModeType.Pointer;

                        _activeSession.MouseCursorShapeChanged += this.PointerCapture.OnMouseCursorShapeChanged;
                        _activeSession.MultiTouchEnabledChanged += this.PointerCapture.OnMultiTouchEnabledChanged;
                        _sessionView.PointerChanged += this.PointerCapture.OnPointerChanged;
                        _sessionView.PointerChanged += this.ScrollBarModel.OnPointerChanged;

                        _activeSessionControl.RenderingPanel.ChangeMouseVisibility(Visibility.Visible);
                        EmitPropertyChanged("IsRenderingPanelActive");
                        EmitPropertyChanged("IsConnecting");

                        if (this.RightSideBarViewModel.FullScreenModel.UserInteractionMode == UserInteractionMode.Mouse)
                        {
                            this.FullScreenModel.EnterFullScreenCommand.Execute(null);
                        }

                        this.IsConnectionBarVisible = true;
                        break;

                    default:
                        //
                        // Remove the belly-band message
                        //
                        this.BellyBandViewModel = null;
                        this.RightSideBarViewModel.RemoteSession = null;
                        this.RightSideBarViewModel.PointerCapture = null;

                        //
                        // If changing state from Connected, remove all event handlers specific to the connected state.
                        //
                        if (SessionState.Connected == _sessionState)
                        {

                            _keyboardCapture.Stop();
                            _keyboardCapture.Keystroke -= this.OnKeystroke;
                            _activeSession.MouseCursorShapeChanged -= this.PointerCapture.OnMouseCursorShapeChanged;
                            _activeSession.MultiTouchEnabledChanged -= this.PointerCapture.OnMultiTouchEnabledChanged;
                            _sessionView.PointerChanged -= this.PointerCapture.OnPointerChanged;

                            this.PointerCapture.ConsumptionMode.ConsumptionModeChanged -= _panKnobSite.OnConsumptionModeChanged;
                            this.PointerCapture.ConsumptionMode.ConsumptionModeChanged -= this.ZoomPanModel.OnConsumptionModeChanged;

                            //
                            // The connection bar and side bars are not available in any non-connected state.
                            //
                            this.IsConnectionBarVisible = false;
                            this.RightSideBarViewModel.Visibility = Visibility.Collapsed;

                            // significant side effects on _panKnobSite.PanKnob setter, which is called implicitly outside the view model,
                            // potentially on a different thread
                            // TODO: Bug 2782808 fix code to avoid these side effects
                            if (null != _panKnobSite.PanKnob)
                            {
                                _panKnobSite.PanKnob.IsVisible = false;
                            }

                            if(this.RightSideBarViewModel.FullScreenModel.IsFullScreenMode)
                            {
                                this.FullScreenModel.ExitFullScreenCommand.Execute(null);
                            }
                        }
                        break;
                }

                this.SessionState = _activeSession.State.State;
            }
        }

        private void OnRightSideBarPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Visibility") && sender is IRightSideBarViewModel)
            {
                Visibility visibility = ((IRightSideBarViewModel)sender).Visibility;
                if(visibility == Visibility.Visible)
                {
                    visibility = Visibility.Collapsed;
                }
                else
                {
                    visibility = Visibility.Visible;
                }

                this.ScrollBarModel.SetScrollbarVisibility(visibility);

                if (_activeSessionControl != null && _activeSessionControl.RenderingPanel != null)
                {
                    _activeSessionControl.RenderingPanel.ChangeMouseVisibility(visibility);
                }
                
            }
        }

        private void OnKeystroke(object sender, KeystrokeEventArgs e)
        {
            Contract.Assert(null != _activeSessionControl);
            _activeSessionControl.SendKeystroke(e.KeyCode, e.IsScanCode, e.IsExtendedKey, e.IsKeyReleased);
        }


        private void OnAppSuspending(object sender, SuspendEventArgs e)
        {
            Contract.Assert(null != _activeSession);
            _activeSession.Suspend();
        }

        private void OnAppResuming(object sender, ResumeEventArgs e)
        {
            Contract.Assert(null != _activeSession);
            _activeSession.Resume();
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
            return null != _inputPanel && null != _deviceCapabilities && _deviceCapabilities.CanShowInputPanel;
        }

        private void OnDeviceCapabilitiesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName.Equals("CanShowInputPanel"))
            {
                _invokeKeyboard.EmitCanExecuteChanged();
            }
        }

        void IInputPanelFactorySite.SetInputPanelFactory(IInputPanelFactory inputPanelFactory)
        {
            _inputPanelFactory = inputPanelFactory;
        }
    }
}
