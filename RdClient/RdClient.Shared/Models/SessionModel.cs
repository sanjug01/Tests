﻿using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.CxWrappers.Utils;
using RdClient.Shared.Helpers;
using System;
using System.Diagnostics.Contracts;

namespace RdClient.Shared.Models
{
    public class ConnectionCreatedArgs : EventArgs
    {
        public IRdpConnection RdpConnection { get; private set; }
        public ConnectionCreatedArgs(IRdpConnection rdpConnection)
        {
            RdpConnection = rdpConnection;
        }
    }

    public class SessionModel : ISessionModel
    {
        public event EventHandler<ConnectionCreatedArgs> ConnectionCreated;

        private readonly IRdpConnectionFactory _connectionFactory;

        private readonly ITimerFactory _timerFactory;

        private IRdpConnection _rdpConnection;

        private Snapshotter _snapshotter;
        
        private void EmitConnectionCreated(ConnectionCreatedArgs args)
        {
            if (ConnectionCreated != null)
            {
                ConnectionCreated(this, args);
            }
        }

        public SessionModel(IRdpConnectionFactory connectionFactory, ITimerFactory timerFactory)
        {
            Contract.Requires(connectionFactory != null);
            _connectionFactory = connectionFactory;
            _timerFactory = timerFactory;

            ConnectionCreated += OnConnectionCreated;
        }

        public void Connect(ConnectionInformation connectionInformation)
        {
            _rdpConnection = _connectionFactory.CreateInstance();
            EmitConnectionCreated(new ConnectionCreatedArgs(_rdpConnection));

            Desktop desktop = connectionInformation.Desktop;
            Credentials credentials = connectionInformation.Credentials;
            IThumbnail thumbnail = connectionInformation.Thumbnail;
            if (thumbnail != null)
            {
                _snapshotter = new Snapshotter(_rdpConnection, thumbnail, _timerFactory);
            }

            RdpPropertyApplier.ApplyDesktop(_rdpConnection as IRdpProperties, desktop);
            _rdpConnection.Connect(credentials, credentials.HaveBeenPersisted);
        }

        public void Disconnect()
        {
            _rdpConnection.Disconnect();

            _rdpConnection = null;
        }

        private void OnConnectionCreated(object sender, ConnectionCreatedArgs args)
        {
            args.RdpConnection.Events.ClientAsyncDisconnect += ClientAsyncDisconnectHandler;
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
