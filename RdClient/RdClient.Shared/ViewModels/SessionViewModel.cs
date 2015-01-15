using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.Helpers;
using RdClient.Shared.Input;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using System;
using System.Diagnostics.Contracts;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{

    public class SessionViewModel : DeferringViewModelBase
    {

        private ConnectionInformation _connectionInformation;
        private IKeyboardCapture _keyboardCapture;

        //
        // TODO: get rid of _currentRdpConnectionshortcut after the session view will have been re-architected
        //
        private IRdpConnection _currentRdpConnection;

        private bool  _isReconnecting;
        private bool _isCancelledReconnect;
        private int _reconnectAttempts;

        private readonly ICommand _disconnectCommand;
        public ICommand DisconnectCommand { get { return _disconnectCommand; } }

        private readonly ICommand _connectCommand;
        public ICommand ConnectCommand { get { return _connectCommand; } }

        private readonly ICommand _cancelReconnectCommand;
        public ICommand CancelReconnectCommand { get { return _cancelReconnectCommand; } }

        public ISessionModel SessionModel { get; set; }
        public DisconnectString DisconnectString { get; set; }
        public MouseViewModel MouseViewModel { get; set; }
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


        public SessionViewModel()
        {
            _isReconnecting = false;
            _reconnectAttempts = 0;
            _disconnectCommand = new RelayCommand(new Action<object>(Disconnect));
            _connectCommand = new RelayCommand(new Action<object>(Connect));
            _cancelReconnectCommand = new RelayCommand(o => { _isCancelledReconnect = true; IsReconnecting = false; });
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
            _keyboardCapture.Keystroke += this.OnKeystroke;
            _keyboardCapture.Start();
        }

        protected override void OnDismissed()
        {
            Contract.Assert(null != _keyboardCapture);
            _keyboardCapture.Stop();
            _keyboardCapture.Keystroke -= this.OnKeystroke;
            base.OnDismissed();
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
                //args.RdpConnection.Events.ConnectionHealthStateChanged += HandleConnectionHealthStateChanged;
                //args.RdpConnection.Events.ClientAutoReconnecting += HanldeClientAutoReconnecting;
                //args.RdpConnection.Events.ClientAutoReconnectComplete += HandleClientAutoReconnectComplete;
                this.MouseViewModel.RdpConnection = args.RdpConnection;
                this.MouseViewModel.DeferredExecution = this;
            };

            SessionModel.ConnectionAutoReconnecting += SessionModel_ConnectionAutoReconnecting;
            SessionModel.ConnectionAutoReconnectComplete += SessionModel_ConnectionAutoReconnectComplete;

            _isCancelledReconnect = false;
            SessionModel.Connect(_connectionInformation, new WinrtThreadPoolTimerFactory(), this.DataModel.Settings);
        }

        void SessionModel_ConnectionAutoReconnectComplete(object sender, ConnectionAutoReconnectCompleteArgs e)
        {
            this.DeferToUI(() =>
                {
                    this.IsReconnecting = false;
                }
            );
        }

        void SessionModel_ConnectionAutoReconnecting(object sender, ConnectionAutoReconnectingArgs e)
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

        void HandleAsyncDisconnect(object sender, ClientAsyncDisconnectArgs args)
        {
            TryDeferToUI(() =>
            {
                _currentRdpConnection = null;
            IRdpConnection rdpConnection = sender as IRdpConnection;
            RdpDisconnectReason reason = args.DisconnectReason;
            bool reconnect = false;

            if (reason.Code == RdpDisconnectCode.CertValidationFailed)
            {
                IRdpCertificate serverCertificate = rdpConnection.GetServerCertificate();
                if (this.SessionModel.IsCertificateAccepted(serverCertificate) || this.DataModel.IsCertificateTrusted(serverCertificate))
                {
                    reconnect = true;
                    rdpConnection.HandleAsyncDisconnectResult(args.DisconnectReason, reconnect);
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
                            rdpConnection.HandleAsyncDisconnectResult(args.DisconnectReason, reconnect);
                        };
                    this.NavigationService.PushModalView("CertificateValidationView", certArgs, certValidationCompletion);
                }
            }
            else
            {
                // May need to further manage PreAuthLogonFailed/FreshCredsRequired/CredSSPUnsupported
                // For all other reasons, we just disconnect.
                // We'll handle showing any appropriate dialogs to the user in OnClientDisconnectedHandler.
                reconnect = false;
                rdpConnection.HandleAsyncDisconnectResult(args.DisconnectReason, reconnect);
            }
            });
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

                if (reason.Code != RdpDisconnectCode.UserInitiated)
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
