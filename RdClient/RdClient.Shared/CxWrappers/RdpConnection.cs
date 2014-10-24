using System.Diagnostics.Contracts;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using RdClient.Shared.CxWrappers.Utils;

namespace RdClient.Shared.CxWrappers
{
    
    public class RdpConnection : IRdpConnection, IRdpProperties
    {
        private RdClientCx.RdpConnection _rdpConnection;
        private IRdpEventHandlers _eventHandlers;

        public RdpConnection(RdClientCx.RdpConnection rdpConnectionCx, IRdpEventHandlers eventHandlers)
        {
            Contract.Requires(rdpConnectionCx != null);
            _rdpConnection = rdpConnectionCx;
            _eventHandlers = eventHandlers;

            _rdpConnection.OnClientConnected += OnClientConnectedHandler;
            _rdpConnection.OnClientAsyncDisconnect += OnClientAsyncDisconnectHandler;
            _rdpConnection.OnClientDisconnected += OnClientDisconnectedHandler;
            _rdpConnection.OnUserCredentialsRequest += OnUserCredentialsRequestHandler;
            _rdpConnection.OnMouseCursorShapeChanged += OnMouseCursorShapeChanged;
            _rdpConnection.OnMouseCursorPositionChanged += OnMouseCursorPositionChanged;
            _rdpConnection.OnMultiTouchEnabledChanged += OnMultiTouchEnabledChanged;

            _rdpConnection.OnConnectionHealthStateChanged += OnConnectionHealthStateChangedHandler;
            _rdpConnection.OnClientAutoReconnecting += OnClientAutoReconnectingHandler;
            _rdpConnection.OnClientAutoReconnectComplete += OnClientAutoReconnectCompleteHandler;
            _rdpConnection.OnLoginCompleted += OnLoginCompletedHandler;
            _rdpConnection.OnStatusInfoReceived += OnStatusInfoReceivedHandler;
            _rdpConnection.OnFirstGraphicsUpdateReceived += OnFirstGraphicsUpdateHandler;
            _rdpConnection.OnRemoteAppWindowCreated += OnRemoteAppWindowCreatedHandler;
            _rdpConnection.OnRemoteAppWindowDeleted += OnRemoteAppWindowDeletedHandler;
            _rdpConnection.OnRemoteAppWindowTitleUpdated += OnRemoteAppWindowTitleUpdatedHandler;
            _rdpConnection.OnRemoteAppWindowIconUpdated += OnRemoteAppWindowIconUpdatedHandler;
        }

        public void Connect(Credentials credentials, bool fUsingSavedCreds)
        {
            int xRes = _rdpConnection.SetUserCredentials(credentials.username, credentials.domain, credentials.password, fUsingSavedCreds);
            RdTrace.IfFailXResultThrow(xRes, "Failed to set user credentials.");

            _rdpConnection.Connect();
        }

        public void Disconnect()
        {
            _rdpConnection.Disconnect();
        }

        public void Suspend()
        {
            _rdpConnection.Suspend();
        }

        public void Resume()
        {
            _rdpConnection.Resume();
        }

        public int HandleAsyncDisconnectResult(RdpDisconnectReason disconnectReason, bool reconnectToServer)
        {
            return _rdpConnection.HandleAsyncDisconnectResult(RdpTypeConverter.ConvertToCxRdpDisconnectReason(disconnectReason), reconnectToServer);
        }

        public int GetIntProperty(string propertyName)
        {
            int value;
            int xRes = _rdpConnection.GetIntProperty(propertyName, out value);

            RdTrace.IfFailXResultThrow(xRes, "Failed to get int property: " + propertyName);

            return value;
        }

        public void SetIntProperty(string propertyName, int value)
        {
            int xRes = _rdpConnection.SetIntProperty(propertyName, value);
            RdTrace.IfFailXResultThrow(xRes, "Failed to set int property: " + propertyName);
        }

        public string GetStringPropery(string propertyName)
        {
            string value;
            int xRes = _rdpConnection.GetStringProperty(propertyName, out value);

            RdTrace.IfFailXResultThrow(xRes, "Failed to get string property: " + propertyName);

            return value;
        }

        public void SetStringProperty(string propertyName, string value)
        {
            int xRes = _rdpConnection.SetStringProperty(propertyName, value);
            RdTrace.IfFailXResultThrow(xRes, "Failed to set string property: " + propertyName);
        }


        public bool GetBoolProperty(string propertyName)
        {
            bool value;
            int xRes = _rdpConnection.GetBoolProperty(propertyName, out value);

            RdTrace.IfFailXResultThrow(xRes, "Failed to get bool property: " + propertyName);

            return value;
        }

        public void SetBoolProperty(string propertyName, bool value)
        {
            int xRes = _rdpConnection.SetBoolProperty(propertyName, value);
            RdTrace.IfFailXResultThrow(xRes, "Failed to set bool property: " + propertyName);
        }

        void OnClientConnectedHandler(RdClientCx.RdpConnection sender)
        {
            _eventHandlers.OnClientConnectedHandler(this);
        }

        void OnClientAsyncDisconnectHandler(RdClientCx.RdpConnection sender, RdClientCx.RdpDisconnectReason disconnectReason)
        {
            _eventHandlers.OnClientAsyncDisconnectHandler(this, RdpTypeConverter.ConvertFromCxRdpDisconnectReason(disconnectReason));
        }

        void OnClientDisconnectedHandler(RdClientCx.RdpConnection sender, RdClientCx.RdpDisconnectReason disconnectReason)
        {
            _eventHandlers.OnClientDisconnectedHandler(this, RdpTypeConverter.ConvertFromCxRdpDisconnectReason(disconnectReason));
        }

        void OnUserCredentialsRequestHandler(RdClientCx.RdpConnection sender, int secLayer)
        {
            _eventHandlers.OnUserCredentialsRequestHandler(this, secLayer);
        }

        void OnMouseCursorShapeChanged(RdClientCx.RdpConnection sender, byte[] buffer, int width, int height, int xHotspot, int yHotspot)
        {
            _eventHandlers.OnMouseCursorShapeChanged(this, buffer, width, height, xHotspot, yHotspot);
        }

        void OnMouseCursorPositionChanged(RdClientCx.RdpConnection sender, int xPos, int yPos)
        {
            _eventHandlers.OnMouseCursorPositionChanged(this, xPos, yPos);
        }

        void OnMultiTouchEnabledChanged(RdClientCx.RdpConnection sender, bool multiTouchEnabled)
        {
            _eventHandlers.OnMultiTouchEnabledChanged(this, multiTouchEnabled);
        }

        void OnConnectionHealthStateChangedHandler(RdClientCx.RdpConnection sender, int connectionState)
        {
            _eventHandlers.OnConnectionHealthStateChangedHandler(this, connectionState);
        }

        void OnClientAutoReconnectingHandler(RdClientCx.RdpConnection sender, int disconnectReason, int attemptCount, out bool continueReconnecting)
        {
            _eventHandlers.OnClientAutoReconnectingHandler(this, disconnectReason, attemptCount, out continueReconnecting);
        }

        void OnClientAutoReconnectCompleteHandler(RdClientCx.RdpConnection sender)
        {
            _eventHandlers.OnClientAutoReconnectCompleteHandler(this);
        }

        void OnLoginCompletedHandler(RdClientCx.RdpConnection sender)
        {
            _eventHandlers.OnLoginCompletedHandler(this);
        }

        void OnStatusInfoReceivedHandler(RdClientCx.RdpConnection sender, int statusCode)
        {
            _eventHandlers.OnStatusInfoReceivedHandler(this, statusCode);
        }

        void OnFirstGraphicsUpdateHandler(RdClientCx.RdpConnection sender)
        {
            _eventHandlers.OnFirstGraphicsUpdateHandler(this);
        }

        void OnRemoteAppWindowCreatedHandler(RdClientCx.RdpConnection sender, uint windowId, string title, byte[] icon, uint iconWidth, uint iconHeight)
        {
            _eventHandlers.OnRemoteAppWindowCreatedHandler(this, windowId, title, icon, iconWidth, iconHeight);
        }

        void OnRemoteAppWindowDeletedHandler(RdClientCx.RdpConnection sender, uint windowId)
        {
            _eventHandlers.OnRemoteAppWindowDeletedHandler(this, windowId);
        }

        void OnRemoteAppWindowTitleUpdatedHandler(RdClientCx.RdpConnection sender, uint windowId, string title)
        {
            _eventHandlers.OnRemoteAppWindowTitleUpdatedHandler(this, windowId, title);
        }

        void OnRemoteAppWindowIconUpdatedHandler(RdClientCx.RdpConnection sender, uint windowId, byte[] icon, uint iconWidth, uint iconHeight)
        {
            _eventHandlers.OnRemoteAppWindowIconUpdatedHandler(this, windowId, icon, iconWidth, iconHeight);
        }
    }
}
