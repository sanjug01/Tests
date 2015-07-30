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
    using RdClient.Shared.Telemetry;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using Windows.UI.Xaml;

    public sealed class RemoteSessionViewModel : DeferringViewModelBase,
        IRemoteSessionViewSite,
        IDeviceCapabilitiesSite,
        ILifeTimeSite,
        IInputPanelFactorySite,
        ITelemetryClientSite
    {
        private readonly Stopwatch _presentationStopwatch;
        private readonly RelayCommand _invokeKeyboard;
        private readonly SymbolBarButtonModel _invokeKeyboardModel;
        private readonly FocusControllerProxy _inputFocusController;
        private IFullScreenModel _fullScreenModel;
        private EventHandler _enteredFullScreenHandler;

        private double
            _enterFullScreenCount,
            _exitFullScreenCount;

        /// <summary>
        /// Proxy object that delegates calls of the IInputFocusController interface to an injected instance of the interface.
        /// The proxy is needed because the view model needs the controller object before it gets attached to a view that provides
        /// the actual implementation of IInputFocusController.
        /// The class creates a read-only instance of FocusControllerProxy and updates its controller when the view injects it.
        /// </summary>
        private sealed class FocusControllerProxy : IInputFocusController
        {
            private IInputFocusController _controller;

            public IInputFocusController Controller
            {
                get { return _controller; }
                set { _controller = value; }
            }

            void IInputFocusController.SetDefault()
            {
                if (null != _controller)
                    _controller.SetDefault();
            }
        }

        public ZoomPanModel ZoomPanModel
        {
            private get; set;
        }

        /// <summary>
        /// The property gets set by a XAML binding.
        /// </summary>
        public IInputFocusController InputFocusController
        {
            get { return _inputFocusController.Controller; }
            set { _inputFocusController.Controller = value; }
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
        private ITelemetryClient _telemetryClient;
        private IPointerCapture _pointerCapture;
        private SessionState _sessionState;
        private bool _isConnectionBarVisible;
        private ReadOnlyObservableCollection<object> _connectionBarItems;

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
            get; set;
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
            private set { SetProperty(ref _connectionBarItems, value); }
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

        public IFullScreenModel FullScreenModel
        {
            private get
            {
                return _fullScreenModel;
            }
            set
            {
                if (!object.ReferenceEquals(_fullScreenModel, value))
                {
                    if(null != _fullScreenModel)
                    {
                        _fullScreenModel.EnteringFullScreen -= this.OnEnteringFullScreen;
                        _fullScreenModel.ExitingFullScreen -= this.OnExitingFullScreen;
                    }
                    _fullScreenModel = value;
                    if (null != _fullScreenModel)
                    {
                        _fullScreenModel.EnteringFullScreen += this.OnEnteringFullScreen;
                        _fullScreenModel.ExitingFullScreen += this.OnExitingFullScreen;
                    }
                }
            }
        }

        public RemoteSessionViewModel()
        {
            _presentationStopwatch = new Stopwatch();
            _invokeKeyboard = new RelayCommand(this.InternalInvokeKeyboard, this.InternalCanInvokeKeyboard);
            _invokeKeyboardModel = new SymbolBarButtonModel() { Glyph = SegoeGlyph.Keyboard, Command = _invokeKeyboard };
            _inputFocusController = new FocusControllerProxy();
            _sessionState = SessionState.Idle;
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(null == _activeSession);
            Contract.Assert(null != _keyboardCapture);
            Contract.Assert(null != _inputPanelFactory);

            _presentationStopwatch.Start();
            _enterFullScreenCount = 0.0;
            _exitFullScreenCount = 0.0;

            base.OnPresenting(activationParameter);

            this.ZoomPanModel = new ZoomPanModel(_inputFocusController, _telemetryClient);

            ObservableCollection<object> items = new ObservableCollection<object>();
            items.Add(new SymbolBarButtonModel() { Glyph = SegoeGlyph.ZoomIn, Command = this.ZoomPanModel.ZoomInCommand });
            items.Add(new SymbolBarButtonModel() { Glyph = SegoeGlyph.ZoomOut, Command = this.ZoomPanModel.ZoomOutCommand });
            items.Add(new SymbolBarButtonModel() { Glyph = SegoeGlyph.More, Command = new FocusStealingRelayCommand(_inputFocusController, RightSideBarVisibilityToggle) });
            items.Add(_invokeKeyboardModel);
            this.ConnectionBarItems = new ReadOnlyObservableCollection<object>(items);

            if (null != _telemetryClient)
            {
                ITelemetryEvent te = _telemetryClient.MakeEvent("InputPanelAvailability");
                te.AddTag("canShow", _deviceCapabilities.CanShowInputPanel ? "true" : "false");
                te.Report();
            }

            _enteredFullScreenHandler = (s, o) => this.DeferToUI(() => CompleteActivation(activationParameter));
            this.FullScreenModel.EnteredFullScreen += _enteredFullScreenHandler;
            this.FullScreenModel.EnterFullScreen();
        }

        private void CompleteActivation(object activationParameter)
        {

            this.FullScreenModel.EnteredFullScreen -= _enteredFullScreenHandler;

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

        private void RightSideBarVisibilityToggle(object parameter)
        {
            if(this.RightSideBarViewModel.Visibility == Visibility.Visible)
            {
                this.RightSideBarViewModel.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.RightSideBarViewModel.Visibility = Visibility.Visible;
            }

            if(null != _inputPanel)
                _inputPanel.Hide();
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

            _presentationStopwatch.Stop();
            _presentationStopwatch.Reset();            

            this.ZoomPanModel.Dispose();
            this.ZoomPanModel = null;

            base.OnDismissed();
        }

        protected override void OnNavigatingBack(IBackCommandArgs backArgs)
        {
            this.BellyBandViewModel?.Terminate();
            this.BellyBandViewModel = null;
            if(_activeSession != null)
            {
                _activeSession.Disconnect();
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

        void IInputPanelFactorySite.SetInputPanelFactory(IInputPanelFactory inputPanelFactory)
        {
            _inputPanelFactory = inputPanelFactory;
        }

        void ITelemetryClientSite.SetTelemetryClient(ITelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
            this.RightSideBarViewModel.SetTelemetryClient(telemetryClient);
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
            //Restore the local mouse
            this._activeSessionControl.RenderingPanel.ChangeMouseVisibility(Visibility.Collapsed);

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

        private void ConnectingAction()
        {
            Contract.Assert(null == this.BellyBandViewModel);
            this.BellyBandViewModel = new RemoteSessionConnectingViewModel(
                _activeSession.HostName,
                () => _activeSession.Disconnect());
            this.IsConnectionBarVisible = false;
            this.RightSideBarViewModel.Visibility = Visibility.Collapsed;
        }

        private void ConnectedAction()
        {

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
                this.TimerFactory,
                this.Dispatcher);

            this.RightSideBarViewModel.PointerCapture = this.PointerCapture;

            this.ZoomPanModel.Initialize(_activeSessionControl.RenderingPanel.Viewport);
            this.ScrollBarModel.Viewport = _activeSessionControl.RenderingPanel.Viewport;

            this.PanKnobSite.Viewport = _activeSessionControl.RenderingPanel.Viewport;
            this.PanKnobSite.OnConsumptionModeChanged(this, _pointerCapture.ConsumptionMode.ConsumptionMode);
            _activeSessionControl.RenderingPanel.Viewport.Changed += this.PanKnobSite.OnViewportChanged;
            this.PanKnobSite.Reset();

            this.PointerCapture.ConsumptionMode.ConsumptionModeChanged += this.PanKnobSite.OnConsumptionModeChanged;
            this.PointerCapture.ConsumptionMode.ConsumptionModeChanged += this.ZoomPanModel.OnConsumptionModeChanged;
            this.PointerCapture.InputDevice.InputDeviceChanged += this.ScrollBarModel.OnInputDeviceChanged;
            this.PointerCapture.ConsumptionMode.ConsumptionMode = ConsumptionModeType.Pointer;

            _activeSession.MouseCursorShapeChanged += this.PointerCapture.OnMouseCursorShapeChanged;
            _activeSession.MultiTouchEnabledChanged += this.PointerCapture.OnMultiTouchEnabledChanged;
            _sessionView.PointerChanged += this.PointerCapture.OnPointerChanged;

            _activeSessionControl.RenderingPanel.ChangeMouseVisibility(Visibility.Visible);
            EmitPropertyChanged("IsRenderingPanelActive");
            EmitPropertyChanged("IsConnecting");

            this.IsConnectionBarVisible = true;
        }

        private void DisconnectedAction()
        {
            _keyboardCapture.Stop();
            _keyboardCapture.Keystroke -= this.OnKeystroke;
            _activeSession.MouseCursorShapeChanged -= this.PointerCapture.OnMouseCursorShapeChanged;
            _activeSession.MultiTouchEnabledChanged -= this.PointerCapture.OnMultiTouchEnabledChanged;
            _sessionView.PointerChanged -= this.PointerCapture.OnPointerChanged;

            this.PointerCapture.ConsumptionMode.ConsumptionModeChanged -= _panKnobSite.OnConsumptionModeChanged;
            this.PointerCapture.ConsumptionMode.ConsumptionModeChanged -= this.ZoomPanModel.OnConsumptionModeChanged;
            this.PointerCapture.InputDevice.InputDeviceChanged -= this.ScrollBarModel.OnInputDeviceChanged;

            //
            // The connection bar and side bars are not available in any non-connected state.
            //
            this.IsConnectionBarVisible = false;
            this.RightSideBarViewModel.Visibility = Visibility.Collapsed;
            _panKnobSite.PanKnob.IsVisible = false;
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
                        ConnectingAction();
                        break;

                    case SessionState.Connected:
                        ConnectedAction();
                        break;

                    case SessionState.Failed:
                        this.FullScreenModel.ExitFullScreen();
                        this.RightSideBarViewModel.RemoteSession = null;
                        this.RightSideBarViewModel.PointerCapture = null;

                        //
                        // If changing state from Connected, remove all event handlers specific to the connected state.
                        //
                        if (SessionState.Connected == _sessionState)
                        {
                            DisconnectedAction();
                        }
                        break;
                    default:
                        if(SessionState.Closed == _activeSession.State.State)
                        {
                            this.FullScreenModel.ExitFullScreen();
                        }

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
                            DisconnectedAction();
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
                Visibility barVisibility = ((IRightSideBarViewModel)sender).Visibility;
                Visibility mouseVisibility = Visibility.Collapsed;
                if(barVisibility == Visibility.Visible)
                {
                    mouseVisibility = Visibility.Collapsed;
                }
                else
                {
                    mouseVisibility = Visibility.Visible;
                }

                this.ScrollBarModel.SetScrollbarVisibility(mouseVisibility);

                if (this.PointerCapture.ConsumptionMode.ConsumptionMode != ConsumptionModeType.Pointer)
                {
                    mouseVisibility = Visibility.Collapsed;
                }

                if (_activeSessionControl != null && _activeSessionControl.RenderingPanel != null)
                {
                    _activeSessionControl.RenderingPanel.ChangeMouseVisibility(mouseVisibility);
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
            {
                if(null != _telemetryClient)
                {
                    ITelemetryEvent te = _telemetryClient.MakeEvent("ShowTouchKeyboard");
                    te.AddMetric("duration", Math.Round(_presentationStopwatch.Elapsed.TotalSeconds));
                    te.Report();
                }
                _inputPanel.Hide();
            }
            else
            {
                if (null != _telemetryClient)
                {
                    ITelemetryEvent te = _telemetryClient.MakeEvent("HideTouchKeyboard");
                    te.AddMetric("duration", Math.Round(_presentationStopwatch.Elapsed.TotalSeconds));
                    te.Report();
                }
                _inputPanel.Show();
            }
        }

        private bool InternalCanInvokeKeyboard(object parameter)
        {
            return null != _inputPanel && null != _deviceCapabilities && _deviceCapabilities.CanShowInputPanel;
        }

        private void OnDeviceCapabilitiesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName.Equals("CanShowInputPanel"))
            {
                if(null != _telemetryClient)
                {
                    ITelemetryEvent te = _telemetryClient.MakeEvent("InputPanelAvailabilityChanged");
                    te.AddTag("canShow", ((IDeviceCapabilities)sender).CanShowInputPanel ? "true" : "false");
                    te.Report();
                }

                _invokeKeyboard.EmitCanExecuteChanged();
            }
        }

        private void OnEnteringFullScreen(object sender, EventArgs e)
        {
            if(null != _telemetryClient)
            {
                ITelemetryEvent te = _telemetryClient.MakeEvent("EnterFullScreen");
                te.AddMetric("duration", Math.Round(_presentationStopwatch.Elapsed.TotalSeconds));
                _enterFullScreenCount += 1.0;
                te.AddMetric("count", _enterFullScreenCount);
                te.Report();
            }
        }

        private void OnExitingFullScreen(object sender, EventArgs e)
        {
            if (null != _telemetryClient)
            {
                ITelemetryEvent te = _telemetryClient.MakeEvent("ExitFullScreen");
                te.AddMetric("duration", Math.Round(_presentationStopwatch.Elapsed.TotalSeconds));
                _exitFullScreenCount += 1.0;
                te.AddMetric("count", _exitFullScreenCount);
                te.Report();
            }
        }
    }
}
