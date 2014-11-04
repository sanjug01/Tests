﻿using RdClient.Navigation;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Utils;
using RdClient.Shared.Models;
using System;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public class ConnectionCreatedArgs : EventArgs
    {
        public IRdpConnection RdpConnection { get; private set; }
        public ConnectionCreatedArgs(IRdpConnection rdpConnection)
        {
            RdpConnection = rdpConnection;
        }
    }

    public class SessionViewModel : ViewModelBase, ISessionViewModel
    {
        public event EventHandler<ConnectionCreatedArgs> ConnectionCreated;
        public ICommand DisconnectCommand { get; private set; }
        public ICommand ConnectCommand { get; private set; }
        public IRdpConnectionFactory RdpConnectionFactory { private get; set; }
        public INavigationService NavigationService { private get; set; }

        private IRdpConnection _rdpConnection;

        public SessionViewModel()
        {
            DisconnectCommand = new RelayCommand(new Action<object>(Disconnect));
            ConnectCommand = new RelayCommand(new Action<object>(Connect));
        }

        private void Connect(object o)
        {
            _rdpConnection = RdpConnectionFactory.CreateInstance();
            if(ConnectionCreated != null)
            {
                ConnectionCreated(this, new ConnectionCreatedArgs(_rdpConnection));
            }

            ConnectionInformation connectionInformation = o as ConnectionInformation;
            Desktop desktop = connectionInformation.Desktop;
            Credentials credentials = connectionInformation.Credentials;

            RdpPropertyApplier.ApplyDesktop(_rdpConnection as IRdpProperties, desktop);
            _rdpConnection.Connect(credentials, credentials.haveBeenPersisted);

            _rdpConnection.Events.ClientAsyncDisconnect += ClientAsyncDisconnectHandler;
        }

        private void Disconnect(object o)
        {
            _rdpConnection.Disconnect();
            if (NavigationService != null)
            {
                NavigationService.NavigateToView("view1", null);
            }

            _rdpConnection.Events.ClientAsyncDisconnect -= ClientAsyncDisconnectHandler;
        }

        public void ClientAsyncDisconnectHandler(object sender, ClientAsyncDisconnectArgs args)
        {
            bool reconnect;

            switch (args.DisconnectReason.Code)
            {
                case RdpDisconnectCode.PreAuthLogonFailed:
                    {
                        reconnect = false;
                    }
                    break;
                case RdpDisconnectCode.FreshCredsRequired:
                    {
                        reconnect = false;
                    }
                    break;

                case RdpDisconnectCode.CertValidationFailed:
                    {
                        reconnect = true;
                    }
                    break;

                case RdpDisconnectCode.CredSSPUnsupported:
                    {
                        reconnect = false;
                    }
                    break;

                default:
                    {
                        //
                        // For all other reasons, we just disconnect.
                        // We'll handle showing any appropriate dialogs to the user in OnClientDisconnectedHandler.
                        //
                        reconnect = false;
                    }
                    break;
            }

            _rdpConnection.HandleAsyncDisconnectResult(args.DisconnectReason, reconnect);
        }

    }
}
