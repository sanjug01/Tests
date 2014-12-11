﻿using RdClient.Shared.Navigation;
using RdClient.Shared.Models;
using System;
using System.Diagnostics.Contracts;
using System.Windows.Input;
using RdClient.Shared.Helpers;
using RdClient.Shared.CxWrappers;

namespace RdClient.Shared.ViewModels
{
    public class SessionViewModel : ViewModelBase, IViewModelDisconnectString
    {
        private ConnectionInformation _connectionInformation;

        private readonly ICommand _disconnectCommand;
        public ICommand DisconnectCommand { get { return _disconnectCommand; } }

        private readonly ICommand _connectCommand;
        public ICommand ConnectCommand { get { return _connectCommand; } }

        public ISessionModel SessionModel { get; set; }
        public DisconnectString DisconnectString { get; set; }

        public SessionViewModel()
        {
            _disconnectCommand = new RelayCommand(new Action<object>(Disconnect));
            _connectCommand = new RelayCommand(new Action<object>(Connect));
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Requires(null != activationParameter as ConnectionInformation);
            _connectionInformation = activationParameter as ConnectionInformation;
        }


        private void Connect(object o)
        {            
            Contract.Assert(null != _connectionInformation);
            Contract.Assert(null != SessionModel);

            SessionModel.ConnectionCreated += (sender, args) => {
                args.RdpConnection.Events.ClientDisconnected += HandleDisconnected;
                args.RdpConnection.Events.ClientAsyncDisconnect += HandleAsyncDisconnect;
            };

            SessionModel.Connect(_connectionInformation);
        }

        void HandleAsyncDisconnect(object sender, ClientAsyncDisconnectArgs args)
        {
            IRdpConnection rdpConnection = sender as IRdpConnection;
            RdpDisconnectReason reason = args.DisconnectReason;
            bool reconnect = false;

            if (reason.Code == RdpDisconnectCode.CertValidationFailed)
            {
                IRdpCertificate serverCertificate = rdpConnection.GetServerCertificate();
                if(this.SessionModel.IsCertificateAccepted(serverCertificate))
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

                            switch(acceptCertificateResult.Result)
                            {
                                case CertificateValidationResult.AcceptType.Denied:
                                    reconnect = false;
                                    break;
                                case CertificateValidationResult.AcceptType.AcceptedOnce:
                                    reconnect = true;
                                    this.SessionModel.AcceptCertificate(serverCertificate, false);
                                    break;
                                case CertificateValidationResult.AcceptType.AcceptedAlways:
                                    reconnect = true;
                                    this.SessionModel.AcceptCertificate(serverCertificate, true);
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
        }        

        private void HandleDisconnected(object sender, ClientDisconnectedArgs args)
        {
            IRdpConnection rdpConnection = sender as IRdpConnection;
            rdpConnection.Events.ClientDisconnected -= HandleDisconnected;

            RdpDisconnectReason reason = args.DisconnectReason;

            if(reason.Code != RdpDisconnectCode.UserInitiated)
            {
                string errorString = DisconnectString.GetDisconnectString(reason);
                DialogMessageArgs dialogArgs = new DialogMessageArgs(errorString, () =>
                {
                }, null);
                this.NavigationService.PushModalView("DialogMessage", dialogArgs);
            }

            NavigationService.NavigateToView("ConnectionCenterView", null);            
        }

        private void Disconnect(object o)
        {
            SessionModel.Disconnect();
        }
    }
}
