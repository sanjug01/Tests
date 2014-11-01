using System.Diagnostics.Contracts;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using RdClient.Shared.CxWrappers.Utils;

namespace RdClient.Shared.CxWrappers
{
    
    public class RdpConnection : IRdpConnection, IRdpProperties
    {
        private RdClientCx.RdpConnection _rdpConnectionCx;
        private RdClientCx.RdpConnectionStore _rdpConnectionStoreCx;

        private IRdpEventProxy _eventProxy;

        public IRdpEvents Events { get { return _eventProxy; } }

        public RdpConnection(RdClientCx.RdpConnection rdpConnectionCx, RdClientCx.RdpConnectionStore rdpConnectionStoreCx)
        {
            Contract.Requires(rdpConnectionCx != null);
            _rdpConnectionCx = rdpConnectionCx;
            _rdpConnectionStoreCx = rdpConnectionStoreCx;
            _eventProxy = new RdpEventProxy();

            _rdpConnectionCx.OnClientConnected += OnClientConnectedHandler;
            _rdpConnectionCx.OnClientAsyncDisconnect += OnClientAsyncDisconnectHandler;
            _rdpConnectionCx.OnClientDisconnected += OnClientDisconnectedHandler;
            _rdpConnectionCx.OnUserCredentialsRequest += OnUserCredentialsRequestHandler;
            _rdpConnectionCx.OnMouseCursorShapeChanged += OnMouseCursorShapeChanged;
            _rdpConnectionCx.OnMouseCursorPositionChanged += OnMouseCursorPositionChanged;
            _rdpConnectionCx.OnMultiTouchEnabledChanged += OnMultiTouchEnabledChanged;

            _rdpConnectionCx.OnConnectionHealthStateChanged += OnConnectionHealthStateChangedHandler;
            _rdpConnectionCx.OnClientAutoReconnecting += OnClientAutoReconnectingHandler;
            _rdpConnectionCx.OnClientAutoReconnectComplete += OnClientAutoReconnectCompleteHandler;
            _rdpConnectionCx.OnLoginCompleted += OnLoginCompletedHandler;
            _rdpConnectionCx.OnStatusInfoReceived += OnStatusInfoReceivedHandler;
            _rdpConnectionCx.OnFirstGraphicsUpdateReceived += OnFirstGraphicsUpdateHandler;
            _rdpConnectionCx.OnRemoteAppWindowCreated += OnRemoteAppWindowCreatedHandler;
            _rdpConnectionCx.OnRemoteAppWindowDeleted += OnRemoteAppWindowDeletedHandler;
            _rdpConnectionCx.OnRemoteAppWindowTitleUpdated += OnRemoteAppWindowTitleUpdatedHandler;
            _rdpConnectionCx.OnRemoteAppWindowIconUpdated += OnRemoteAppWindowIconUpdatedHandler;
        }

        ~RdpConnection()
        {
            int xRes;

            // remove from connection store
            RdClientCx.RdpConnectionStore rdpConnectionStore;
            xRes = RdClientCx.RdpConnectionStore.GetConnectionStore(out rdpConnectionStore);
            RdTrace.IfFailXResultThrow(xRes, "Unable to retrieve the connection store.");
            rdpConnectionStore.RemoveConnection(_rdpConnectionCx);
            rdpConnectionStore = null;


            _rdpConnectionCx.OnClientConnected -= OnClientConnectedHandler;
            _rdpConnectionCx.OnClientAsyncDisconnect -= OnClientAsyncDisconnectHandler;
            _rdpConnectionCx.OnClientDisconnected -= OnClientDisconnectedHandler;
            _rdpConnectionCx.OnUserCredentialsRequest -= OnUserCredentialsRequestHandler;
            _rdpConnectionCx.OnMouseCursorShapeChanged -= OnMouseCursorShapeChanged;
            _rdpConnectionCx.OnMouseCursorPositionChanged -= OnMouseCursorPositionChanged;
            _rdpConnectionCx.OnMultiTouchEnabledChanged -= OnMultiTouchEnabledChanged;
            _rdpConnectionCx.OnConnectionHealthStateChanged -= OnConnectionHealthStateChangedHandler;
            _rdpConnectionCx.OnClientAutoReconnecting -= OnClientAutoReconnectingHandler;
            _rdpConnectionCx.OnClientAutoReconnectComplete -= OnClientAutoReconnectCompleteHandler;
            _rdpConnectionCx.OnLoginCompleted -= OnLoginCompletedHandler;
            _rdpConnectionCx.OnStatusInfoReceived -= OnStatusInfoReceivedHandler;
            _rdpConnectionCx.OnFirstGraphicsUpdateReceived -= OnFirstGraphicsUpdateHandler;
            _rdpConnectionCx.OnRemoteAppWindowCreated -= OnRemoteAppWindowCreatedHandler;
            _rdpConnectionCx.OnRemoteAppWindowDeleted -= OnRemoteAppWindowDeletedHandler;
            _rdpConnectionCx.OnRemoteAppWindowTitleUpdated -= OnRemoteAppWindowTitleUpdatedHandler;
            _rdpConnectionCx.OnRemoteAppWindowIconUpdated -= OnRemoteAppWindowIconUpdatedHandler;

            //
            // Terminate the connection object.
            //
            xRes = _rdpConnectionCx.TerminateInstance();
            RdTrace.IfFailXResultThrow(xRes, "Unable to terminate RDP connection.");
            _rdpConnectionCx = null;
        }
        
        public void Connect(Credentials credentials, bool fUsingSavedCreds)
        {
            int xRes = _rdpConnectionCx.SetUserCredentials(credentials.username, credentials.domain, credentials.password, fUsingSavedCreds);
            RdTrace.IfFailXResultThrow(xRes, "Failed to set user credentials.");

            xRes = _rdpConnectionCx.Connect();
            RdTrace.IfFailXResultThrow(xRes, "Failed to connect.");
        }

        public void Disconnect()
        {
            int xRes = _rdpConnectionCx.Disconnect();
            RdTrace.IfFailXResultThrow(xRes, "Failed to disconnect.");
            
            xRes = _rdpConnectionCx.TerminateInstance();
            RdTrace.IfFailXResultThrow(xRes, "Failed to terminate connection instance.");

            xRes = _rdpConnectionStoreCx.RemoveConnection(_rdpConnectionCx);
            RdTrace.IfFailXResultThrow(xRes, "Failed to disconnect remove connection from store.");

            _rdpConnectionCx = null;
        }

        public void Suspend()
        {
            int xRes = _rdpConnectionCx.Suspend();
            RdTrace.IfFailXResultThrow(xRes, "Failed to suspend.");
        }

        public void Resume()
        {
            int xRes = _rdpConnectionCx.Resume();
            RdTrace.IfFailXResultThrow(xRes, "Failed to resume.");
        }

        public void TerminateInstance()
        {
            _rdpConnectionCx.TerminateInstance();
        }

        public void HandleAsyncDisconnectResult(RdpDisconnectReason disconnectReason, bool reconnectToServer)
        {
            int xRes = _rdpConnectionCx.HandleAsyncDisconnectResult(RdpTypeConverter.ConvertToCxRdpDisconnectReason(disconnectReason), reconnectToServer);
            RdTrace.IfFailXResultThrow(xRes, "Failed async disconnect.");
        }

        public int GetIntProperty(string propertyName)
        {
            int value;
            int xRes = _rdpConnectionCx.GetIntProperty(propertyName, out value);

            RdTrace.IfFailXResultThrow(xRes, "Failed to get int property: " + propertyName);

            return value;
        }

        public void SetIntProperty(string propertyName, int value)
        {
            int xRes = _rdpConnectionCx.SetIntProperty(propertyName, value);
            RdTrace.IfFailXResultThrow(xRes, "Failed to set int property: " + propertyName);
        }

        public string GetStringPropery(string propertyName)
        {
            string value;
            int xRes = _rdpConnectionCx.GetStringProperty(propertyName, out value);

            RdTrace.IfFailXResultThrow(xRes, "Failed to get string property: " + propertyName);

            return value;
        }

        public void SetStringProperty(string propertyName, string value)
        {
            int xRes = _rdpConnectionCx.SetStringProperty(propertyName, value);
            RdTrace.IfFailXResultThrow(xRes, "Failed to set string property: " + propertyName);
        }


        public bool GetBoolProperty(string propertyName)
        {
            bool value;
            int xRes = _rdpConnectionCx.GetBoolProperty(propertyName, out value);

            RdTrace.IfFailXResultThrow(xRes, "Failed to get bool property: " + propertyName);

            return value;
        }

        public void SetBoolProperty(string propertyName, bool value)
        {
            int xRes = _rdpConnectionCx.SetBoolProperty(propertyName, value);
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
