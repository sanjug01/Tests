namespace RdClient.Shared.ViewModels
{
using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Keyboard;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
using System;
using System.Diagnostics.Contracts;
using System.Windows.Input;
using Windows.UI.Xaml;

    public class SessionViewModel : DeferringViewModelBase, IElephantEarsViewModel, ITimerFactorySite
{
        private ConnectionInformation _connectionInformation;
        private IKeyboardCapture _keyboardCapture;
        private ITimerFactory _timerFactory;
        private bool _capturingKeyboard;
        private IRdpConnection _currentRdpConnection; // TODO: get rid of _currentRdpConnectionshortcut after the session view will have been re-architected
        private bool  _isReconnecting;
        private bool _isCancelledReconnect;
        private int _reconnectAttempts;
        private readonly ICommand _disconnectCommand;
        private readonly ICommand _connectCommand;
        private readonly ICommand _cancelReconnectCommand;
        private ICommand _connectionBarcommand;
        private Visibility _elephantEarsVisible;
        private bool _userCancelled;

        public SessionViewModel()
        { 
            _isReconnecting = false;
            _reconnectAttempts = 0;
            _elephantEarsVisible = Visibility.Collapsed;
            _disconnectCommand = new RelayCommand(new Action<object>(Disconnect));
            _connectCommand = new RelayCommand(new Action<object>(Connect));
            _cancelReconnectCommand = new RelayCommand(o => { _isCancelledReconnect = true; IsReconnecting = false; });            
            this.ConnectionBarCommand = new RelayCommand(o =>
            {
                if (this.ElephantEarsVisible == Visibility.Visible)
                {
                    this.ElephantEarsVisible = Visibility.Collapsed;
                }
                else
                {
                    this.ElephantEarsVisible = Visibility.Visible;
                }
            });
            }

        public ISessionModel SessionModel { get; set; }

        public DisconnectString DisconnectString { get; set; }

        public MouseViewModel MouseViewModel { get; set; }

        public ICommand DisconnectCommand { get { return _disconnectCommand; } }

        public ICommand ConnectCommand { get { return _connectCommand; } }

        public ICommand CancelReconnectCommand { get { return _cancelReconnectCommand; } }


        public ZoomPanViewModel ZoomPanViewModel { get; set; }
        public IKeyboardCapture KeyboardCapture
        {
            get { return _keyboardCapture; }
            set { this.SetProperty<IKeyboardCapture>(ref _keyboardCapture, value); }
        }

        public bool IsReconnecting
        {
            get { return _isReconnecting; }
            set { this.SetProperty(ref _isReconnecting, value); }
        }

        public int ReconnectAttempts
        {
            get { return _reconnectAttempts; }
            set { this.SetProperty(ref _reconnectAttempts, value); }
        }

        public Visibility ElephantEarsVisible
        {
            get { return _elephantEarsVisible; }
            set { SetProperty(ref _elephantEarsVisible, value); }
        }

        public ICommand ConnectionBarCommand
        {
            get { return _connectionBarcommand; }
            set { SetProperty(ref _connectionBarcommand, value); }
        }

        public string HostName
            {
            get
                {
                if (_connectionInformation != null)
                {
                    return _connectionInformation.Desktop.HostName;
                }
                else
                {
                    return String.Empty;
                }
        }
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Requires(null != activationParameter as ConnectionInformation);
            Contract.Assert(null != _keyboardCapture);

            _connectionInformation = activationParameter as ConnectionInformation;
            //
            // TODO: heavy refactoring of the session UI is needed to support multiple sessions
            //       as a part of thye refactoring tracking current session must be tracked, keyboard
            //       capture must be started when a new session may send input (either when a connected session
            //       is presented or a presented session is connected); then, when a session is deactivated
            //       or disconnected, the keyboard capture must be stopped.
            //
            StartKeyboardCapture();
        }

        protected override void OnDismissed()
        {
            StopKeyboardCapture();
            base.OnDismissed();
        }

        private void StartKeyboardCapture()
        {
            if (!_capturingKeyboard && _keyboardCapture != null)
            {
            _keyboardCapture.Keystroke += this.OnKeystroke;
            _keyboardCapture.Start();
                _capturingKeyboard = true;
        }
        }

        private void StopKeyboardCapture()
        {
            if (_capturingKeyboard && _keyboardCapture != null)
            {
            _keyboardCapture.Stop();
            _keyboardCapture.Keystroke -= this.OnKeystroke;
                _capturingKeyboard = false;
            }
        }

        void ITimerFactorySite.SetTimerFactory(ITimerFactory timerFactory)
        {
            _timerFactory = timerFactory;
        }

        private void Connect(object o)
        {            
            Contract.Assert(null != _connectionInformation);

            if( null == this.SessionModel )
            {
                //
                // Session model may be passed to the command as a parameter.
                //
                this.SessionModel = o as ISessionModel;
            }
            Contract.Assert(null != SessionModel);

            SessionModel.ConnectionCreated += (sender, args) => {
                args.RdpConnection.Events.ClientConnected += HandleConnected;
                args.RdpConnection.Events.ClientDisconnected += HandleDisconnected;
                args.RdpConnection.Events.ClientAsyncDisconnect += HandleAsyncDisconnect;
                this.MouseViewModel.RdpConnection = args.RdpConnection;
                this.MouseViewModel.DeferredExecution = this;
                this.MouseViewModel.ElephantEarsViewModel = this;
            };

            SessionModel.ConnectionAutoReconnecting += SessionModel_ConnectionAutoReconnecting;
            SessionModel.ConnectionAutoReconnectComplete += SessionModel_ConnectionAutoReconnectComplete;

            _isCancelledReconnect = false;

            Contract.Assert(null != _timerFactory);
            SessionModel.Connect(_connectionInformation, _timerFactory, this.DataModel.Settings);
        }

        private void SessionModel_ConnectionAutoReconnectComplete(object sender, ConnectionAutoReconnectCompleteArgs e)
        {
            this.DeferToUI(() =>
                {
                    this.IsReconnecting = false;
                }
            );
        }

        private void SessionModel_ConnectionAutoReconnecting(object sender, ConnectionAutoReconnectingArgs e)
        {
            Contract.Assert(null != e);

            // TODO: may need to disable user input for the session: keyboard/mouse 
            if(0 == e.AttemptCount)
            {
                // reset cancelledReconnect state
                _isCancelledReconnect = false;
            }

            // always continue reconnecting if not canceled
            e.ContinueDelegate.Invoke(!_isCancelledReconnect);
            this.DeferToUI(() =>
                {
                    this.ReconnectAttempts = e.AttemptCount;
                    this.IsReconnecting = !_isCancelledReconnect;
                }
            );
        }

        private void HandleAsyncDisconnect(object sender, ClientAsyncDisconnectArgs args)
        {
            TryDeferToUI(() =>
            {
                _currentRdpConnection = null;
            IRdpConnection rdpConnection = sender as IRdpConnection;
            RdpDisconnectReason reason = args.DisconnectReason;
            bool reconnect = false;

                switch (reason.Code)
                {
                    case RdpDisconnectCode.CertValidationFailed:
                        HandleCertValidationFailed(rdpConnection, reason);
                        break;
                    case RdpDisconnectCode.PreAuthLogonFailed:
                    case RdpDisconnectCode.FreshCredsRequired:
                        HandleBadCredentials(rdpConnection, reason);
                        break;
                    default:
                        // May need to further manage CredSSPUnsupported
                        // For all other reasons, we just disconnect.
                        // We'll handle showing any appropriate dialogs to the user in OnClientDisconnectedHandler.
                        reconnect = false;
                        rdpConnection.HandleAsyncDisconnectResult(args.DisconnectReason, reconnect);
                        break;
                }
            });
        }      

        private void HandleBadCredentials(IRdpConnection rdpConnection, RdpDisconnectReason reason)
        {
            CredentialPromptMode mode;
            if (reason.Code == RdpDisconnectCode.PreAuthLogonFailed)
            {
                mode = CredentialPromptMode.InvalidCredentials;
            }
            else if (reason.Code == RdpDisconnectCode.FreshCredsRequired)
            {
                mode = CredentialPromptMode.FreshCredentialsNeeded;
            }
            else
            {
                throw new InvalidOperationException();
            }
            Credentials copyOfCreds = new Credentials();
            copyOfCreds.CopyValuesFrom(_connectionInformation.Credentials);
            AddUserViewArgs args = new AddUserViewArgs(copyOfCreds, true, mode);
            ModalPresentationCompletion completionContext = new ModalPresentationCompletion();
            completionContext.Completed += (s, e) =>
            {
                this.StartKeyboardCapture();//restart keyboard capture that was stopped when credential prompt was shown
                CredentialPromptResult result = e.Result as CredentialPromptResult;
                if (result != null && !result.UserCancelled)
                {
                    Credentials cred = result.Credential;                    
                    if (result.Save)//if user chose to save credentials then persist them
                    {
                        _connectionInformation.Credentials.CopyValuesFrom(cred);//overwrite previous credentials
                        _connectionInformation.Desktop.CredentialId = _connectionInformation.Credentials.Id;//update desktop to use these creds
                        if (!DataModel.LocalWorkspace.Credentials.ContainsItemWithId(_connectionInformation.Credentials.Id))
                        {
                            DataModel.LocalWorkspace.Credentials.Add(_connectionInformation.Credentials);//add creds to data model if not already there
                        }
                    }
                    else //just use the creds for this connection (do not persist them)
                    {
                        _connectionInformation.Credentials = cred;
                    }
                    rdpConnection.SetCredentials(_connectionInformation.Credentials, false);
                    rdpConnection.HandleAsyncDisconnectResult(reason, true);
                }
                else
                {
                    _userCancelled = true;
                    rdpConnection.HandleAsyncDisconnectResult(reason, false);
                }
            };
            this.StopKeyboardCapture();//stop keyboard capture so that text can be entered at credential prompt
            this.NavigationService.PushModalView("AddUserView", args, completionContext);
        }
  
        private void HandleCertValidationFailed(IRdpConnection rdpConnection, RdpDisconnectReason reason)
            {
            bool reconnect = false;
                IRdpCertificate serverCertificate = rdpConnection.GetServerCertificate();
            if (reason.Code != RdpDisconnectCode.CertValidationFailed)
            {
                throw new InvalidOperationException();
            }
            else if (this.SessionModel.IsCertificateAccepted(serverCertificate) || this.DataModel.IsCertificateTrusted(serverCertificate))
                {
                    reconnect = true;
                rdpConnection.HandleAsyncDisconnectResult(reason, reconnect);
                }
                else
                {
                    // present CertificateValidation dialog and reconnect only if certificate is accepted
                    CertificateValidationViewModelArgs certArgs = new CertificateValidationViewModelArgs(
                        _connectionInformation.Desktop.HostName,
                        serverCertificate);
                    ModalPresentationCompletion certValidationCompletion = new ModalPresentationCompletion();

                    certValidationCompletion.Completed += (s, e) =>
                        {
                            Contract.Assert(e.Result is CertificateValidationResult);
                            CertificateValidationResult acceptCertificateResult = e.Result as CertificateValidationResult;

                            switch (acceptCertificateResult.Result)
                            {
                                case CertificateValidationResult.CertificateTrustLevel.Denied:
                                    reconnect = false;
                                    break;
                                case CertificateValidationResult.CertificateTrustLevel.AcceptedOnce:
                                    reconnect = true;
                                    this.SessionModel.AcceptCertificate(serverCertificate);
                                    break;
                                case CertificateValidationResult.CertificateTrustLevel.AcceptedAlways:
                                    reconnect = true;
                                    this.DataModel.TrustCertificate(serverCertificate);
                                    break;
                            }
                    rdpConnection.HandleAsyncDisconnectResult(reason, reconnect);
                        };
                    this.NavigationService.PushModalView("CertificateValidationView", certArgs, certValidationCompletion);
                }
        }        

        private void HandleConnected(object sender, ClientConnectedArgs e)
        {
            TryDeferToUI(() =>
            {
                _currentRdpConnection = sender as IRdpConnection;
                Contract.Assert(null != _currentRdpConnection);
            });
        }

        private void HandleDisconnected(object sender, ClientDisconnectedArgs args)
        {
            TryDeferToUI(() =>
            {
                _currentRdpConnection = null;
                IRdpConnection rdpConnection = sender as IRdpConnection;
                rdpConnection.Events.ClientDisconnected -= HandleDisconnected;
                rdpConnection.Cleanup();

                RdpDisconnectReason reason = args.DisconnectReason;

                if (!_userCancelled && reason.Code != RdpDisconnectCode.UserInitiated)
                {
                    ErrorMessageArgs dialogArgs = new ErrorMessageArgs(reason, () =>
                    {
                    }, null);
                    this.NavigationService.PushModalView("ErrorMessageView", dialogArgs);
                }

                NavigationService.NavigateToView("ConnectionCenterView", null);
            });
        }

        private void Disconnect(object o)
        {
            SessionModel.ConnectionAutoReconnecting -= SessionModel_ConnectionAutoReconnecting;
            SessionModel.ConnectionAutoReconnectComplete -= SessionModel_ConnectionAutoReconnectComplete;
            SessionModel.Disconnect();
        }

        private void OnKeystroke(object sender, KeystrokeEventArgs e)
        {
            //
            // TODO: send the keystroke over to the session properly
            //
            if(null != _currentRdpConnection)
            {
                _currentRdpConnection.SendKeyEvent(e.KeyCode, e.IsScanCode, e.IsExtendedKey, e.IsKeyReleased);
            }
        }
    }
}
