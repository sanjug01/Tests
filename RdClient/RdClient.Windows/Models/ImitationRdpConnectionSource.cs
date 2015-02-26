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

        private sealed class Events : MutableObject, IRdpEvents
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
            //
            public void EmitClientConnected( ClientConnectedArgs clientConnected)
            {
                using(LockUpgradeableRead())
                {
                    if (null != _clientConnected)
                        _clientConnected(this, clientConnected);
                }
            }

            public void EmitClientAsyncDisconnect(ClientAsyncDisconnectArgs clientAsyncDisconnect)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _clientAsyncDisconnect)
                        _clientAsyncDisconnect(this, clientAsyncDisconnect);
                }
            }

            public void EmitClientDisconnected(ClientDisconnectedArgs clientDisconnected)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _clientDisconnected)
                        _clientDisconnected(this, clientDisconnected);
                }
            }

            public void EmitUserCredentialsRequest(UserCredentialsRequestArgs userCredentialsRequest)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _userCredentialsRequest)
                        _userCredentialsRequest(this, userCredentialsRequest);
                }
            }
            
            public void EmitMouseCursorShapeChanged(MouseCursorShapeChangedArgs mouseCursorShapeChanged)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _mouseCursorShapeChanged)
                        _mouseCursorShapeChanged(this, mouseCursorShapeChanged);
                }
            }
            
            public void EmitMouseCursorPositionChanged(MouseCursorPositionChangedArgs mouseCursorPositionChanged)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _mouseCursorPositionChanged)
                        _mouseCursorPositionChanged(this, mouseCursorPositionChanged);
                }
            }
            
            public void EmitMultiTouchEnabledChanged(MultiTouchEnabledChangedArgs multiTouchEnabledChanged)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _multiTouchEnabledChanged)
                        _multiTouchEnabledChanged(this, multiTouchEnabledChanged);
                }
            }
            
            public void EmitConnectionHealthStateChanged(ConnectionHealthStateChangedArgs connectionHealthStateChanged)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _connectionHealthStateChanged)
                        _connectionHealthStateChanged(this, connectionHealthStateChanged);
                }
            }
            
            public void EmitClientAutoReconnecting(ClientAutoReconnectingArgs clientAutoReconnecting)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _clientAutoReconnecting)
                        _clientAutoReconnecting(this, clientAutoReconnecting);
                }
            }
            
            public void EmitClientAutoReconnectComplete(ClientAutoReconnectCompleteArgs clientAutoReconnectComplete)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _clientAutoReconnectComplete)
                        _clientAutoReconnectComplete(this, clientAutoReconnectComplete);
                }
            }
            
            public void EmitLoginCompleted(LoginCompletedArgs loginCompleted)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _loginCompleted)
                        _loginCompleted(this, loginCompleted);
                }
            }
            
            public void EmitStatusInfoReceived(StatusInfoReceivedArgs statusInfoReceived)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _statusInfoReceived)
                        _statusInfoReceived(this, statusInfoReceived);
                }
            }
            
            public void EmitFirstGraphicsUpdate(FirstGraphicsUpdateArgs firstGraphicsUpdate)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _firstGraphicsUpdate)
                        _firstGraphicsUpdate(this, firstGraphicsUpdate);
                }
            }
            
            public void EmitRemoteAppWindowCreated(RemoteAppWindowCreatedArgs remoteAppWindowCreated)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _remoteAppWindowCreated)
                        _remoteAppWindowCreated(this, remoteAppWindowCreated);
                }
            }
            
            public void EmitRemoteAppWindowDeleted(RemoteAppWindowDeletedArgs remoteAppWindowDeleted)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _remoteAppWindowDeleted)
                        _remoteAppWindowDeleted(this, remoteAppWindowDeleted);
                }
            }
            
            public void EmitRemoteAppWindowTitleUpdated(RemoteAppWindowTitleUpdatedArgs remoteAppWindowTitleUpdated)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _remoteAppWindowTitleUpdated)
                        _remoteAppWindowTitleUpdated(this, remoteAppWindowTitleUpdated);
                }
            }
            
            public void EmitRemoteAppWindowIconUpdated(RemoteAppWindowIconUpdatedArgs remoteAppWindowIconUpdated)
            {
                using (LockUpgradeableRead())
                {
                    if (null != _remoteAppWindowIconUpdated)
                        _remoteAppWindowIconUpdated(this, remoteAppWindowIconUpdated);
                }
            }
            //
            //
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
                    //_events.EmitClientConnected(new ClientConnectedArgs());
                    _events.EmitClientAsyncDisconnect(new ClientAsyncDisconnectArgs(
                        new RdpDisconnectReason(RdpDisconnectCode.PasswordMustChange, 0, 0)));
                }, TaskCreationOptions.LongRunning);
            }

            void IRdpConnection.Disconnect()
            {
                Task.Factory.StartNew(async delegate
                {
                    await Task.Delay(100);
                    _events.EmitClientAsyncDisconnect(new ClientAsyncDisconnectArgs(
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
