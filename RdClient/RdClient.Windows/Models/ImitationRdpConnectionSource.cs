namespace RdClient.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using System;
    using System.Threading.Tasks;
    using Windows.Foundation;

    sealed class ImitationRdpConnectionSource : IRdpConnectionSource
    {
        IRdpConnection IRdpConnectionSource.CreateConnection(IRenderingPanel renderingPanel)
        {
            return new Connection(renderingPanel);
        }

        private sealed class Events : MutableObject, IRdpEvents, IRdpEventSource
        {
            private EventHandler<ClientConnectedArgs> _clientConnected;
            private EventHandler<ClientAsyncDisconnectArgs> _clientAsyncDisconnect;
            private EventHandler<ClientDisconnectedArgs> _clientDisconnected;
            private EventHandler<UserCredentialsRequestArgs> _userCredentialsRequest;
            private EventHandler<MouseCursorShapeChangedArgs> _mouseCursorShapeChanged;
            private EventHandler<MouseCursorPositionChangedArgs> _mouseCursorPositionChanged;
            private EventHandler<MultiTouchEnabledChangedArgs> _multiTouchEnabledChanged;
            private EventHandler<ConnectionHealthStateChangedArgs> _connectionHealthStateChanged;
            private EventHandler<ClientAutoReconnectingArgs> _clientAutoReconnecting;
            private EventHandler<ClientAutoReconnectCompleteArgs> _clientAutoReconnectComplete;
            private EventHandler<LoginCompletedArgs> _loginCompleted;
            private EventHandler<StatusInfoReceivedArgs> _statusInfoReceived;
            private EventHandler<FirstGraphicsUpdateArgs> _firstGraphicsUpdate;
            private EventHandler<RemoteAppWindowCreatedArgs> _remoteAppWindowCreated;
            private EventHandler<RemoteAppWindowDeletedArgs> _remoteAppWindowDeleted;
            private EventHandler<RemoteAppWindowTitleUpdatedArgs> _remoteAppWindowTitleUpdated;
            private EventHandler<RemoteAppWindowIconUpdatedArgs> _remoteAppWindowIconUpdated;
            //
            // IRdpEvents
            //
            event EventHandler<ClientConnectedArgs> IRdpEvents.ClientConnected
            {
                add { using(LockWrite()) _clientConnected += value; }
                remove { using(LockWrite()) _clientConnected -= value; }
            }

            event EventHandler<ClientAsyncDisconnectArgs> IRdpEvents.ClientAsyncDisconnect
            {
                add { using (LockWrite()) _clientAsyncDisconnect += value; }
                remove { using (LockWrite()) _clientAsyncDisconnect -= value; }
            }

            event EventHandler<ClientDisconnectedArgs> IRdpEvents.ClientDisconnected
            {
                add { using (LockWrite()) _clientDisconnected += value; }
                remove { using (LockWrite()) _clientDisconnected -= value; }
            }

            event EventHandler<UserCredentialsRequestArgs> IRdpEvents.UserCredentialsRequest
            {
                add { using (LockWrite()) _userCredentialsRequest += value; }
                remove { using (LockWrite()) _userCredentialsRequest -= value; }
            }

            event EventHandler<MouseCursorShapeChangedArgs> IRdpEvents.MouseCursorShapeChanged
            {
                add { using (LockWrite()) _mouseCursorShapeChanged += value; }
                remove { using (LockWrite()) _mouseCursorShapeChanged -= value; }
            }

            event EventHandler<MouseCursorPositionChangedArgs> IRdpEvents.MouseCursorPositionChanged
            {
                add { using (LockWrite()) _mouseCursorPositionChanged += value; }
                remove { using (LockWrite()) _mouseCursorPositionChanged -= value; }
            }

            event EventHandler<MultiTouchEnabledChangedArgs> IRdpEvents.MultiTouchEnabledChanged
            {
                add { using (LockWrite()) _multiTouchEnabledChanged += value; }
                remove { using (LockWrite()) _multiTouchEnabledChanged -= value; }
            }

            event EventHandler<ConnectionHealthStateChangedArgs> IRdpEvents.ConnectionHealthStateChanged
            {
                add { using (LockWrite()) _connectionHealthStateChanged += value; }
                remove { using (LockWrite()) _connectionHealthStateChanged -= value; }
            }

            event EventHandler<ClientAutoReconnectingArgs> IRdpEvents.ClientAutoReconnecting
            {
                add { using (LockWrite()) _clientAutoReconnecting += value; }
                remove { using (LockWrite()) _clientAutoReconnecting -= value; }
            }

            event EventHandler<ClientAutoReconnectCompleteArgs> IRdpEvents.ClientAutoReconnectComplete
            {
                add { using (LockWrite()) _clientAutoReconnectComplete += value; }
                remove { using (LockWrite()) _clientAutoReconnectComplete -= value; }
            }

            event EventHandler<LoginCompletedArgs> IRdpEvents.LoginCompleted
            {
                add { using (LockWrite()) _loginCompleted += value; }
                remove { using (LockWrite()) _loginCompleted -= value; }
            }

            event EventHandler<StatusInfoReceivedArgs> IRdpEvents.StatusInfoReceived
            {
                add { using (LockWrite()) _statusInfoReceived += value; }
                remove { using (LockWrite()) _statusInfoReceived -= value; }
            }

            event EventHandler<FirstGraphicsUpdateArgs> IRdpEvents.FirstGraphicsUpdate
            {
                add { using (LockWrite()) _firstGraphicsUpdate += value; }
                remove { using (LockWrite()) _firstGraphicsUpdate -= value; }
            }

            event EventHandler<RemoteAppWindowCreatedArgs> IRdpEvents.RemoteAppWindowCreated
            {
                add { using (LockWrite()) _remoteAppWindowCreated += value; }
                remove { using (LockWrite()) _remoteAppWindowCreated -= value; }
            }

            event EventHandler<RemoteAppWindowDeletedArgs> IRdpEvents.RemoteAppWindowDeleted
            {
                add { using (LockWrite()) _remoteAppWindowDeleted += value; }
                remove { using (LockWrite()) _remoteAppWindowDeleted -= value; }
            }

            event EventHandler<RemoteAppWindowTitleUpdatedArgs> IRdpEvents.RemoteAppWindowTitleUpdated
            {
                add { using (LockWrite()) _remoteAppWindowTitleUpdated += value; }
                remove { using (LockWrite()) _remoteAppWindowTitleUpdated -= value; }
            }

            event EventHandler<RemoteAppWindowIconUpdatedArgs> IRdpEvents.RemoteAppWindowIconUpdated
            {
                add { using (LockWrite()) _remoteAppWindowIconUpdated += value; }
                remove { using (LockWrite()) _remoteAppWindowIconUpdated -= value; }
            }
            //
            // IRdpEventSource
            //
            public void EmitClientConnected(IRdpConnection sender, ClientConnectedArgs args)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _clientConnected)
                        _clientConnected(sender, args);
                }
            }

            public void EmitClientAsyncDisconnect(IRdpConnection sender, ClientAsyncDisconnectArgs args)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _clientAsyncDisconnect)
                        _clientAsyncDisconnect(sender, args);
                }
            }

            public void EmitClientDisconnected(IRdpConnection sender, ClientDisconnectedArgs args)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _clientDisconnected)
                        _clientDisconnected(sender, args);
                }
            }

            public void EmitUserCredentialsRequest(IRdpConnection sender, UserCredentialsRequestArgs args)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _userCredentialsRequest)
                        _userCredentialsRequest(sender, args);
                }
            }

            public void EmitMouseCursorShapeChanged(IRdpConnection sender, MouseCursorShapeChangedArgs args)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _mouseCursorShapeChanged)
                        _mouseCursorShapeChanged(sender, args);
                }
            }

            public void EmitMouseCursorPositionChanged(IRdpConnection sender, MouseCursorPositionChangedArgs args)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _mouseCursorPositionChanged)
                        _mouseCursorPositionChanged(sender, args);
                }
            }

            public void EmitMultiTouchEnabledChanged(IRdpConnection sender, MultiTouchEnabledChangedArgs args)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _multiTouchEnabledChanged)
                        _multiTouchEnabledChanged(sender, args);
                }
            }

            public void EmitConnectionHealthStateChanged(IRdpConnection sender, ConnectionHealthStateChangedArgs args)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _connectionHealthStateChanged)
                        _connectionHealthStateChanged(sender, args);
                }
            }

            public void EmitClientAutoReconnecting(IRdpConnection sender, ClientAutoReconnectingArgs args)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _clientAutoReconnecting)
                        _clientAutoReconnecting(sender, args);
                }
            }

            public void EmitClientAutoReconnectComplete(IRdpConnection sender, ClientAutoReconnectCompleteArgs args)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _clientAutoReconnectComplete)
                        _clientAutoReconnectComplete(sender, args);
                }
            }

            public void EmitLoginCompleted(IRdpConnection sender, LoginCompletedArgs args)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _loginCompleted)
                        _loginCompleted(sender, args);
                }
            }

            public void EmitStatusInfoReceived(IRdpConnection sender, StatusInfoReceivedArgs args)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _statusInfoReceived)
                        _statusInfoReceived(sender, args);
                }
            }

            public void EmitFirstGraphicsUpdate(IRdpConnection sender, FirstGraphicsUpdateArgs args)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _firstGraphicsUpdate)
                        _firstGraphicsUpdate(sender, args);
                }
            }

            public void EmitRemoteAppWindowCreated(IRdpConnection sender, RemoteAppWindowCreatedArgs args)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _remoteAppWindowCreated)
                        _remoteAppWindowCreated(sender, args);
                }
            }

            public void EmitRemoteAppWindowDeleted(IRdpConnection sender, RemoteAppWindowDeletedArgs args)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _remoteAppWindowDeleted)
                        _remoteAppWindowDeleted(sender, args);
                }
            }

            public void EmitRemoteAppWindowTitleUpdated(IRdpConnection sender, RemoteAppWindowTitleUpdatedArgs args)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _remoteAppWindowTitleUpdated)
                        _remoteAppWindowTitleUpdated(sender, args);
                }
            }

            public void EmitRemoteAppWindowIconUpdated(IRdpConnection sender, RemoteAppWindowIconUpdatedArgs args)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _remoteAppWindowIconUpdated)
                        _remoteAppWindowIconUpdated(sender, args);
                }
            }
        }

        private sealed class Connection : MutableObject, IRdpConnection
        {
            private readonly IRenderingPanel _renderingPanel;
            private readonly Events _events;

            public Connection(IRenderingPanel renderingPanel)
            {
                _renderingPanel = renderingPanel;
                _events = new Events();
            }

            IRdpEvents IRdpConnection.Events
            {
                get { return _events; }
            }

            void IRdpConnection.SetCredentials(CredentialsModel credentials, bool fUsingSavedCreds)
            {
            }

            void IRdpConnection.Connect(CredentialsModel credentials, bool fUsingSavedCreds)
            {
                Task.Factory.StartNew(async delegate
                {
                    await Task.Delay(250);

                    if (fUsingSavedCreds)
                    {
                        _events.EmitClientAsyncDisconnect(this,
                            new ClientAsyncDisconnectArgs(
                                new RdpDisconnectReason(RdpDisconnectCode.FreshCredsRequired, 0, 0)));
                    }
                    else
                    {
                        _events.EmitClientDisconnected(this, new ClientDisconnectedArgs(new RdpDisconnectReason(RdpDisconnectCode.CertExpired, 0, 0)));
                        //_events.EmitClientConnected(this, new ClientConnectedArgs());
                    }
                }, TaskCreationOptions.LongRunning);
            }

            void IRdpConnection.Disconnect()
            {
                Task.Factory.StartNew(async delegate
                {
                    await Task.Delay(100);
                    _events.EmitClientAsyncDisconnect(this,
                        new ClientAsyncDisconnectArgs(
                            new RdpDisconnectReason(RdpDisconnectCode.UserInitiated, 0, 0)));
                }, TaskCreationOptions.LongRunning);
            }

            void IRdpConnection.Suspend()
            {
                throw new NotImplementedException();
            }

            void IRdpConnection.Resume()
            {
                throw new NotImplementedException();
            }

            void IRdpConnection.TerminateInstance()
            {
                throw new NotImplementedException();
            }

            void IRdpConnection.Cleanup()
            {
                throw new NotImplementedException();
            }

            void IRdpConnection.HandleAsyncDisconnectResult(RdpDisconnectReason disconnectReason, bool reconnectToServer)
            {
                throw new NotImplementedException();
            }

            IRdpScreenSnapshot IRdpConnection.GetSnapshot()
            {
                throw new NotImplementedException();
            }

            IRdpCertificate IRdpConnection.GetServerCertificate()
            {
                throw new NotImplementedException();
            }

            void IRdpConnection.SendMouseEvent(MouseEventType type, float xPos, float yPos)
            {
            }

            void IRdpConnection.SendKeyEvent(int keyValue, bool scanCode, bool extended, bool keyUp)
            {
            }

            void IRdpConnection.SendTouchEvent(TouchEventType type, uint contactId, Point position, ulong frameTime)
            {
            }

            void IRdpConnection.SetLeftHandedMouseMode(bool value)
            {
                throw new NotImplementedException();
            }
        }
    }
}
