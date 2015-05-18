namespace RdClient.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using Windows.Foundation;

    abstract class ImitationRdpConnectionSource : IRdpConnectionSource
    {
        protected ImitationRdpConnectionSource()
        {
        }

        protected abstract IRdpConnectionFactory CreateConnectionFactory(IRenderingPanel renderingPanel);

        IRdpConnection IRdpConnectionSource.CreateConnection(RemoteConnectionModel model, IRenderingPanel renderingPanel)
        {
            //
            // Create a temporary connection factory only to give it to the model object that creates
            // actual RDP connections.
            //
            return model.CreateConnection(CreateConnectionFactory(renderingPanel), renderingPanel);
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
            private EventHandler<CheckGatewayCertificateTrustArgs> _checkGatewayCertificateTrustRequest;
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
            event EventHandler<CheckGatewayCertificateTrustArgs> IRdpEvents.CheckGatewayCertificateTrust
            {
                add { using (LockWrite()) _checkGatewayCertificateTrustRequest += value; }
                remove { using (LockWrite()) _checkGatewayCertificateTrustRequest -= value; }
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

            void IRdpEventSource.EmitClientConnected(IRdpConnection sender, ClientConnectedArgs args)
            {
                throw new NotImplementedException();
            }

            void IRdpEventSource.EmitClientAsyncDisconnect(IRdpConnection sender, ClientAsyncDisconnectArgs args)
            {
                throw new NotImplementedException();
            }

            void IRdpEventSource.EmitClientDisconnected(IRdpConnection sender, ClientDisconnectedArgs args)
            {
                throw new NotImplementedException();
            }

            void IRdpEventSource.EmitUserCredentialsRequest(IRdpConnection sender, UserCredentialsRequestArgs args)
            {
                throw new NotImplementedException();
            }

            void IRdpEventSource.EmitMouseCursorShapeChanged(IRdpConnection sender, MouseCursorShapeChangedArgs args)
            {
                throw new NotImplementedException();
            }

            void IRdpEventSource.EmitMouseCursorPositionChanged(IRdpConnection sender, MouseCursorPositionChangedArgs args)
            {
                throw new NotImplementedException();
            }

            void IRdpEventSource.EmitMultiTouchEnabledChanged(IRdpConnection sender, MultiTouchEnabledChangedArgs args)
            {
                throw new NotImplementedException();
            }

            void IRdpEventSource.EmitConnectionHealthStateChanged(IRdpConnection sender, ConnectionHealthStateChangedArgs args)
            {
                throw new NotImplementedException();
            }

            void IRdpEventSource.EmitClientAutoReconnecting(IRdpConnection sender, ClientAutoReconnectingArgs args)
            {
                throw new NotImplementedException();
            }

            void IRdpEventSource.EmitClientAutoReconnectComplete(IRdpConnection sender, ClientAutoReconnectCompleteArgs args)
            {
                throw new NotImplementedException();
            }

            void IRdpEventSource.EmitLoginCompleted(IRdpConnection sender, LoginCompletedArgs args)
            {
                throw new NotImplementedException();
            }

            void IRdpEventSource.EmitStatusInfoReceived(IRdpConnection sender, StatusInfoReceivedArgs args)
            {
                throw new NotImplementedException();
            }

            void IRdpEventSource.EmitFirstGraphicsUpdate(IRdpConnection sender, FirstGraphicsUpdateArgs args)
            {
                throw new NotImplementedException();
            }

            void IRdpEventSource.EmitRemoteAppWindowCreated(IRdpConnection sender, RemoteAppWindowCreatedArgs args)
            {
                throw new NotImplementedException();
            }

            void IRdpEventSource.EmitRemoteAppWindowDeleted(IRdpConnection sender, RemoteAppWindowDeletedArgs args)
            {
                throw new NotImplementedException();
            }

            void IRdpEventSource.EmitRemoteAppWindowTitleUpdated(IRdpConnection sender, RemoteAppWindowTitleUpdatedArgs args)
            {
                throw new NotImplementedException();
            }

            void IRdpEventSource.EmitRemoteAppWindowIconUpdated(IRdpConnection sender, RemoteAppWindowIconUpdatedArgs args)
            {
                throw new NotImplementedException();
            }

            void IRdpEventSource.EmitCheckGatewayCertificateTrust(IRdpConnection sender, CheckGatewayCertificateTrustArgs args)
            {
                throw new NotImplementedException();
            }
        }

        protected abstract class Connection : MutableObject, IRdpConnection, IRdpProperties
        {
            private readonly IRenderingPanel _renderingPanel;
            private readonly Events _events;

            private RdpDisconnectReason _currentDisconnectReason;

            protected void EmitAsyncDisconnect(RdpDisconnectReason reason)
            {
                Contract.Assert(null == _currentDisconnectReason);

                _currentDisconnectReason = reason;
                _events.EmitClientAsyncDisconnect(this, new ClientAsyncDisconnectArgs(reason));
            }


            protected void EmitConnected()
            {
                _events.EmitClientConnected(this, new ClientConnectedArgs());
            }

            protected void EmitDisconnected(RdpDisconnectReason reason)
            {
                _events.EmitClientDisconnected(this, new ClientDisconnectedArgs(reason));
            }

            protected virtual void SetCredentials(CredentialsModel credentials, bool fUsingSavedCreds)
            {
                throw new NotImplementedException();
            }
            protected virtual void SetGateway(GatewayModel gateway, CredentialsModel gatewayCredentials)
            {
                throw new NotImplementedException();
            }

            protected virtual void Connect()
            {
                throw new NotImplementedException();
            }

            protected virtual void Disconnect()
            {
                throw new NotImplementedException();
            }

            protected virtual void HandleAsyncDisconnectResult(RdpDisconnectReason disconnectReason, bool reconnectToServer)
            {
                throw new NotImplementedException();
            }

            protected virtual IRdpCertificate GetServerCertificate()
            {
                throw new NotImplementedException();
            }

            protected virtual IRdpCertificate GetGatewayCertificate()
            {
                throw new NotImplementedException();
            }

            protected Connection(IRenderingPanel renderingPanel)
            {
                Contract.Assert(null != renderingPanel);

                _renderingPanel = renderingPanel;
                _events = new Events();
            }

            IRdpEvents IRdpConnection.Events
            {
                get { return _events; }
            }

            void IRdpConnection.SetCredentials(CredentialsModel credentials, bool fUsingSavedCreds)
            {
                this.SetCredentials(credentials, fUsingSavedCreds);
            }
            void IRdpConnection.SetGateway(GatewayModel gateway, CredentialsModel gateayCredentials)
            {
                this.SetGateway(gateway, gateayCredentials);
            }

            void IRdpConnection.Connect()
            {
                this.Connect();
            }

            void IRdpConnection.Disconnect()
            {
                this.Disconnect();
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
            }

            void IRdpConnection.HandleAsyncDisconnectResult(RdpDisconnectReason disconnectReason, bool reconnectToServer)
            {
                Contract.Assert(null != disconnectReason);
                Contract.Assert(object.ReferenceEquals(disconnectReason, _currentDisconnectReason));

                _currentDisconnectReason = null;
                this.HandleAsyncDisconnectResult(disconnectReason, reconnectToServer);
            }

            IRdpScreenSnapshot IRdpConnection.GetSnapshot()
            {
                throw new NotImplementedException();
            }

            IRdpCertificate IRdpConnection.GetServerCertificate()
            {
                return this.GetServerCertificate();
            }

            void IRdpConnection.SendMouseEvent(MouseEventType type, float xPos, float yPos)
            {
            }

            void IRdpConnection.SendKeyEvent(int keyValue, bool scanCode, bool extended, bool keyUp)
            {
                Debug.WriteLine("SendKeyEvent|value={0}, scan code={1}, extended={2}, key up={3}",
                    keyValue, scanCode, extended, keyUp);
            }

            void IRdpConnection.SendTouchEvent(TouchEventType type, uint contactId, Point position, ulong frameTime)
            {
            }

            void IRdpConnection.SetLeftHandedMouseMode(bool value)
            {
                throw new NotImplementedException();
            }

            int IRdpProperties.GetIntProperty(string propertyName)
            {
                throw new NotImplementedException();
            }

            void IRdpProperties.SetIntProperty(string propertyName, int value)
            {
                throw new NotImplementedException();
            }

            string IRdpProperties.GetStringPropery(string propertyName)
            {
                throw new NotImplementedException();
            }

            void IRdpProperties.SetStringProperty(string propertyName, string value)
            {
                throw new NotImplementedException();
            }

            bool IRdpProperties.GetBoolProperty(string propertyName)
            {
                throw new NotImplementedException();
            }

            void IRdpProperties.SetBoolProperty(string propertyName, bool value)
            {
                throw new NotImplementedException();
            }

            IRdpCertificate IRdpConnection.GetGatewayCertificate()
            {
                throw new NotImplementedException();
            }
        }
    }
}
