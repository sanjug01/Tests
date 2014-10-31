using System.Diagnostics.Contracts;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using RdClient.Shared.CxWrappers.Utils;

namespace RdClient.Shared.CxWrappers
{
    
    public class RdpConnection : IRdpConnection, IRdpProperties
    {
        private RdClientCx.RdpConnection _rdpConnection;
        private IRdpEventProxy _eventProxy;

        public IRdpEvents Events { get { return _eventProxy; } }

        public RdpConnection(RdClientCx.RdpConnection rdpConnectionCx)
        {
            Contract.Requires(rdpConnectionCx != null);
            _rdpConnection = rdpConnectionCx;
            _eventProxy = new RdpEventProxy();

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

        ~RdpConnection()
        {
            int xRes;

            // remove from connection store
            RdClientCx.RdpConnectionStore rdpConnectionStore;
            xRes = RdClientCx.RdpConnectionStore.GetConnectionStore(out rdpConnectionStore);
            RdTrace.IfFailXResultThrow(xRes, "Unable to retrieve the connection store.");
            rdpConnectionStore.RemoveConnection(_rdpConnection);
            rdpConnectionStore = null;


            _rdpConnection.OnClientConnected -= OnClientConnectedHandler;
            _rdpConnection.OnClientAsyncDisconnect -= OnClientAsyncDisconnectHandler;
            _rdpConnection.OnClientDisconnected -= OnClientDisconnectedHandler;
            _rdpConnection.OnUserCredentialsRequest -= OnUserCredentialsRequestHandler;
            _rdpConnection.OnMouseCursorShapeChanged -= OnMouseCursorShapeChanged;
            _rdpConnection.OnMouseCursorPositionChanged -= OnMouseCursorPositionChanged;
            _rdpConnection.OnMultiTouchEnabledChanged -= OnMultiTouchEnabledChanged;
            _rdpConnection.OnConnectionHealthStateChanged -= OnConnectionHealthStateChangedHandler;
            _rdpConnection.OnClientAutoReconnecting -= OnClientAutoReconnectingHandler;
            _rdpConnection.OnClientAutoReconnectComplete -= OnClientAutoReconnectCompleteHandler;
            _rdpConnection.OnLoginCompleted -= OnLoginCompletedHandler;
            _rdpConnection.OnStatusInfoReceived -= OnStatusInfoReceivedHandler;
            _rdpConnection.OnFirstGraphicsUpdateReceived -= OnFirstGraphicsUpdateHandler;
            _rdpConnection.OnRemoteAppWindowCreated -= OnRemoteAppWindowCreatedHandler;
            _rdpConnection.OnRemoteAppWindowDeleted -= OnRemoteAppWindowDeletedHandler;
            _rdpConnection.OnRemoteAppWindowTitleUpdated -= OnRemoteAppWindowTitleUpdatedHandler;
            _rdpConnection.OnRemoteAppWindowIconUpdated -= OnRemoteAppWindowIconUpdatedHandler;

            //
            // Terminate the connection object.
            //
            xRes = _rdpConnection.TerminateInstance();
            RdTrace.IfFailXResultThrow(xRes, "Unable to terminate RDP connection.");
            _rdpConnection = null;
        }
        

        public void Connect(Credentials credentials, bool fUsingSavedCreds)
        {
            int xRes = _rdpConnection.SetUserCredentials(credentials.username, credentials.domain, credentials.password, fUsingSavedCreds);
            RdTrace.IfFailXResultThrow(xRes, "Failed to set user credentials.");

            xRes = _rdpConnection.Connect();
            RdTrace.IfFailXResultThrow(xRes, "Failed to connect.");
        }

        public void Disconnect()
        {
            int xRes = _rdpConnection.Disconnect();
            RdTrace.IfFailXResultThrow(xRes, "Failed to disconnect.");
        }

        public void Suspend()
        {
            int xRes = _rdpConnection.Suspend();
            RdTrace.IfFailXResultThrow(xRes, "Failed to suspend.");
        }

        public void Resume()
        {
            int xRes = _rdpConnection.Resume();
            RdTrace.IfFailXResultThrow(xRes, "Failed to resume.");
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
            _eventProxy.EmitClientConnected(this, new ClientConnectedArgs());
        }

        void OnClientAsyncDisconnectHandler(RdClientCx.RdpConnection sender, RdClientCx.RdpDisconnectReason disconnectReason)
        {
            _eventProxy.EmitClientAsyncDisconnect(this, new ClientAsyncDisconnectArgs(RdpTypeConverter.ConvertFromCxRdpDisconnectReason(disconnectReason)));
        }

        void OnClientDisconnectedHandler(RdClientCx.RdpConnection sender, RdClientCx.RdpDisconnectReason disconnectReason)
        {
            _eventProxy.EmitClientDisconnected(this, new ClientDisconnectedArgs(RdpTypeConverter.ConvertFromCxRdpDisconnectReason(disconnectReason)));
        }

        void OnUserCredentialsRequestHandler(RdClientCx.RdpConnection sender, int secLayer)
        {
            _eventProxy.EmitUserCredentialsRequest(this, new UserCredentialsRequestArgs(secLayer));
        }

        void OnMouseCursorShapeChanged(RdClientCx.RdpConnection sender, byte[] buffer, int width, int height, int xHotspot, int yHotspot)
        {
            _eventProxy.EmitMouseCursorShapeChanged(this, new MouseCursorShapeChangedArgs(buffer, width, height, xHotspot, yHotspot));            
        }

        void OnMouseCursorPositionChanged(RdClientCx.RdpConnection sender, int xPos, int yPos)
        {
            _eventProxy.EmitMouseCursorPositionChanged(this, new MouseCursorPositionChangedArgs(xPos, yPos));            
        }

        void OnMultiTouchEnabledChanged(RdClientCx.RdpConnection sender, bool multiTouchEnabled)
        {
            _eventProxy.EmitMultiTouchEnabledChanged(this, new MultiTouchEnabledChangedArgs(multiTouchEnabled));            
        }

        void OnConnectionHealthStateChangedHandler(RdClientCx.RdpConnection sender, int connectionState)
        {
            _eventProxy.EmitConnectionHealthStateChanged(this, new ConnectionHealthStateChangedArgs(connectionState));            
        }

        void OnClientAutoReconnectingHandler(RdClientCx.RdpConnection sender, int disconnectReason, int attemptCount, out bool continueReconnecting)
        {
            bool _continueReconnecting = false;
            ClientAutoReconnectingContinueDelegate shouldContinue = (__continueReconnecting) => { _continueReconnecting = __continueReconnecting; };

            _eventProxy.EmitClientAutoReconnecting(this, new ClientAutoReconnectingArgs(disconnectReason, attemptCount, shouldContinue));

            continueReconnecting = _continueReconnecting;
        }

        void OnClientAutoReconnectCompleteHandler(RdClientCx.RdpConnection sender)
        {
            _eventProxy.EmitClientAutoReconnectComplete(this, new ClientAutoReconnectCompleteArgs());
        }

        void OnLoginCompletedHandler(RdClientCx.RdpConnection sender)
        {
            _eventProxy.EmitLoginCompleted(this, new LoginCompletedArgs());
        }

        void OnStatusInfoReceivedHandler(RdClientCx.RdpConnection sender, int statusCode)
        {
            _eventProxy.EmitStatusInfoReceived(this, new StatusInfoReceivedArgs(statusCode));
        }

        void OnFirstGraphicsUpdateHandler(RdClientCx.RdpConnection sender)
        {
            _eventProxy.EmitFirstGraphicsUpdate(this, new FirstGraphicsUpdateArgs());
        }

        void OnRemoteAppWindowCreatedHandler(RdClientCx.RdpConnection sender, uint windowId, string title, byte[] icon, uint iconWidth, uint iconHeight)
        {
            _eventProxy.EmitRemoteAppWindowCreated(this, new RemoteAppWindowCreatedArgs(windowId, title, icon, iconWidth, iconHeight));
        }

        void OnRemoteAppWindowDeletedHandler(RdClientCx.RdpConnection sender, uint windowId)
        {
            _eventProxy.EmitRemoteAppWindowDeleted(this, new RemoteAppWindowDeletedArgs(windowId));
        }

        void OnRemoteAppWindowTitleUpdatedHandler(RdClientCx.RdpConnection sender, uint windowId, string title)
        {
            _eventProxy.EmitRemoteAppWindowTitleUpdated(this, new RemoteAppWindowTitleUpdatedArgs(windowId, title));
        }

        void OnRemoteAppWindowIconUpdatedHandler(RdClientCx.RdpConnection sender, uint windowId, byte[] icon, uint iconWidth, uint iconHeight)
        {
            _eventProxy.EmitRemoteAppWindowIconUpdated(this, new RemoteAppWindowIconUpdatedArgs(windowId, icon, iconWidth, iconHeight));
        }

    }
}
