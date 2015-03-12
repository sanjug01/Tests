using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.Models;
using System.Diagnostics.Contracts;
using Windows.Foundation;
using Windows.Security.Cryptography.Certificates;
using RdClient.Shared.Helpers;

namespace RdClient.Shared.CxWrappers
{
    
    public class RdpConnection : IRdpConnection, IRdpProperties
    {
        private RdInstrumenter _instrument = new RdInstrumenter();

        private readonly RdClientCx.RdpConnectionStore _rdpConnectionStoreCx;
        private RdClientCx.RdpConnection _rdpConnectionCx;

        private IRdpEventSource _eventProxy;

        public IRdpEvents Events { get { return _eventProxy as IRdpEvents; } }

        public RdpConnection(RdClientCx.RdpConnection rdpConnectionCx, RdClientCx.RdpConnectionStore rdpConnectionStoreCx, RdpEventSource eventProxy)
        {
            Contract.Requires(rdpConnectionCx != null);

            _rdpConnectionStoreCx = rdpConnectionStoreCx;

            _rdpConnectionCx = rdpConnectionCx;
            _eventProxy = eventProxy;

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

        public RdpConnection(RemoteApplicationModel remoteApp, RdClientCx.RdpConnectionStore rdpConnectionStoreCx, RdpEventSource eventProxy)
        {
            rdpConnectionStoreCx.LaunchRemoteApp(remoteApp.RdpFile, out _rdpConnectionCx);
            _rdpConnectionStoreCx = rdpConnectionStoreCx;
            _eventProxy = eventProxy;

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


        // TODO
        // TODO
        // TODO
        // TODO this has to go away from the destructor.
        // also, there is a race condition which makes disconnect events to be called multiple times
        ~RdpConnection()
        {
            if(_rdpConnectionCx != null)
            {
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

                // remove from connection store
                // RdClientCx.RdpConnectionStore rdpConnectionStore;
                // int xRes;

                // xRes = RdClientCx.RdpConnectionStore.GetConnectionStore(out rdpConnectionStore);
                // RdTrace.IfFailXResultThrow(xRes, "Unable to retrieve the connection store.");
                _rdpConnectionStoreCx.RemoveConnection(_rdpConnectionCx);

                TerminateInstance();

                _rdpConnectionCx = null;   
            }         
        }

        public void SetCredentials(CredentialsModel credentials, bool fUsingSavedCreds)
        {
            _instrument.Instrument("SetCredentials");
            int xRes = _rdpConnectionCx.SetUserCredentials(
                credentials.Username,
                string.Empty, // Empty domain strings; the application doesn't have UI where the domain may be entered
                credentials.Password,
                fUsingSavedCreds);
            RdTrace.IfFailXResultThrow(xRes, "Failed to set user credentials.");
        }
        
        public void Connect(CredentialsModel credentials, bool fUsingSavedCreds)
        {
            _instrument.Instrument("Connect");
            this.SetCredentials(credentials, fUsingSavedCreds);
            int xRes = _rdpConnectionCx.Connect();
            RdTrace.IfFailXResultThrow(xRes, "Failed to connect.");
        }

        public void Cleanup()
        {
            _instrument.Instrument("Cleanup");

            if (null != _rdpConnectionCx)
            {
                int xRes = _rdpConnectionStoreCx.RemoveConnection(_rdpConnectionCx);
                RdTrace.IfFailXResultThrow(xRes, "Failed to disconnect remove connection from store.");

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

                _rdpConnectionCx = null;
            }
        }

        public void Disconnect()
        {
            _instrument.Instrument("Disconnect");

            int xRes = _rdpConnectionCx.Disconnect();
            RdTrace.IfFailXResultThrow(xRes, "Failed to disconnect.");
        }

        public void Suspend()
        {
            _instrument.Instrument("Suspend");

            int xRes = _rdpConnectionCx.Suspend();
            RdTrace.IfFailXResultThrow(xRes, "Failed to suspend.");
        }

        public void Resume()
        {
            _instrument.Instrument("Resume");

            int xRes = _rdpConnectionCx.Resume();
            RdTrace.IfFailXResultThrow(xRes, "Failed to resume.");
        }

        public void TerminateInstance()
        {
            _instrument.Instrument("TerminateInstance");

            _rdpConnectionCx.TerminateInstance();
        }

        public void HandleAsyncDisconnectResult(RdpDisconnectReason disconnectReason, bool reconnectToServer)
        {
            _instrument.Instrument("HandleAsyncDisconnectResult");

            // TODO fix crash when this is called after Disconnect() has set _rdpConnectionCx to null
            int xRes = _rdpConnectionCx.HandleAsyncDisconnectResult(RdpTypeConverter.ConvertToCx(disconnectReason), reconnectToServer);
            RdTrace.IfFailXResultThrow(xRes, "Failed async disconnect.");
        }

        public int GetIntProperty(string propertyName)
        {
            _instrument.Instrument("GetIntProperty");

            int value;
            int xRes = _rdpConnectionCx.GetIntProperty(propertyName, out value);

            RdTrace.IfFailXResultThrow(xRes, "Failed to get int property: " + propertyName);

            return value;
        }

        public void SetIntProperty(string propertyName, int value)
        {
            _instrument.Instrument("SetIntProperty");

            int xRes = _rdpConnectionCx.SetIntProperty(propertyName, value);
            RdTrace.IfFailXResultThrow(xRes, "Failed to set int property: " + propertyName);
        }

        public string GetStringPropery(string propertyName)
        {
            _instrument.Instrument("GetStringPropery");

            string value;
            int xRes = _rdpConnectionCx.GetStringProperty(propertyName, out value);

            RdTrace.IfFailXResultThrow(xRes, "Failed to get string property: " + propertyName);

            return value;
        }

        public void SetStringProperty(string propertyName, string value)
        {
            _instrument.Instrument("SetStringProperty");

            int xRes = _rdpConnectionCx.SetStringProperty(propertyName, value);
            RdTrace.IfFailXResultThrow(xRes, "Failed to set string property: " + propertyName);
        }


        public bool GetBoolProperty(string propertyName)
        {
            _instrument.Instrument("GetBoolProperty");

            bool value;
            int xRes = _rdpConnectionCx.GetBoolProperty(propertyName, out value);

            RdTrace.IfFailXResultThrow(xRes, "Failed to get bool property: " + propertyName);

            return value;
        }

        public void SetBoolProperty(string propertyName, bool value)
        {
            _instrument.Instrument("SetBoolProperty");

            int xRes = _rdpConnectionCx.SetBoolProperty(propertyName, value);
            RdTrace.IfFailXResultThrow(xRes, "Failed to set bool property: " + propertyName);
        }

        public void SetLeftHandedMouseMode(bool value)
        {
            int xRes = _rdpConnectionCx.SetLeftHandedMouseMode(value);
            RdTrace.IfFailXResultThrow(xRes, "Failed to set LeftHandedMouseMode: " + value);
        }

        public IRdpScreenSnapshot GetSnapshot()
        {
            _instrument.Instrument("GetSnapshot");

            int width, height;
            byte[] bytes;
            int xRes = _rdpConnectionCx.GetSnapshot(out width, out height, out bytes);            
            if (xRes == 0)
            {                
                return new RdpScreenSnapshot(width, height, bytes);                
            }
            else
            {
                //snapshot can fail in some scenarios and is not a critical error, so we don't throw.
                RdTrace.TraceWrn("GetSnapshot failed with xRes = " + xRes);
                return null; 
            }            
        }

        public void SendMouseEvent(MouseEventType type, float xPos, float yPos)
        {
            //_instrument.Instrument("SendMouseEvent");
            float _yPos = yPos;
            float _xPos = xPos;

            // This is needed because RdClientCx silently multiplies these values by 2
            // the reason RdClientCx does this is to achieve a natural scrolling experience
            // on small screen touch devices. However the universal App targets not only
            // devices with bigger screens but also receives input events from the hardware
            // mouse.
            if(type == MouseEventType.MouseWheel || type == MouseEventType.MouseHWheel)
            {
                _xPos = xPos / 2.0f;
                _yPos = yPos / 2.0f;
            }

            int xRes = _rdpConnectionCx.SendMouseEvent(RdpTypeConverter.ConvertToCx(type), _xPos, _yPos);
            RdTrace.IfFailXResultThrow(xRes, "Failed to send mouse event.");
        }

        public void SendTouchEvent(TouchEventType type, uint contactId, Point position, ulong frameTime)
        {
            //_instrument.Instrument("SendTouchEvent");
            int xRes = _rdpConnectionCx.SendTouchEvent(RdpTypeConverter.ConvertToCx(type), contactId, (float)position.X, (float)position.Y, (uint)frameTime);
            RdTrace.IfFailXResultThrow(xRes, "Failed to send touch event.");
        }

        public void SendKeyEvent(int keyValue, bool scanCode, bool extended, bool keyUp)
        {
            _instrument.Instrument("SendKeyEvent");

            int xRes = _rdpConnectionCx.SendKeyEvent(keyValue, scanCode, extended, keyUp);
            RdTrace.IfFailXResultThrow(xRes, "Failed to send key event.");
        }


        void OnClientConnectedHandler(RdClientCx.RdpConnection sender)
        {
            _instrument.Instrument("OnClientConnectedHandler");

            _eventProxy.EmitClientConnected(this, new ClientConnectedArgs());
        }

        void OnClientAsyncDisconnectHandler(RdClientCx.RdpConnection sender, RdClientCx.RdpDisconnectReason disconnectReason)
        {
            _instrument.Instrument("OnClientAsyncDisconnectHandler");

            _eventProxy.EmitClientAsyncDisconnect(this, new ClientAsyncDisconnectArgs(RdpTypeConverter.ConvertFromCx(disconnectReason)));
        }

        void OnClientDisconnectedHandler(RdClientCx.RdpConnection sender, RdClientCx.RdpDisconnectReason disconnectReason)
        {
            _instrument.Instrument("OnClientDisconnectedHandler");

            _eventProxy.EmitClientDisconnected(this, new ClientDisconnectedArgs(RdpTypeConverter.ConvertFromCx(disconnectReason)));
        }

        void OnUserCredentialsRequestHandler(RdClientCx.RdpConnection sender, int secLayer)
        {
            _instrument.Instrument("OnUserCredentialsRequestHandler");

            _eventProxy.EmitUserCredentialsRequest(this, new UserCredentialsRequestArgs(secLayer));
        }

        void OnMouseCursorShapeChanged(RdClientCx.RdpConnection sender, byte[] buffer, int width, int height, int xHotspot, int yHotspot)
        {
            _instrument.Instrument("OnMouseCursorShapeChanged");

            _eventProxy.EmitMouseCursorShapeChanged(this, new MouseCursorShapeChangedArgs(buffer, width, height, xHotspot, yHotspot));            
        }

        void OnMouseCursorPositionChanged(RdClientCx.RdpConnection sender, int xPos, int yPos)
        {
            _instrument.Instrument("OnMouseCursorPositionChanged");

            _eventProxy.EmitMouseCursorPositionChanged(this, new MouseCursorPositionChangedArgs(xPos, yPos));            
        }

        void OnMultiTouchEnabledChanged(RdClientCx.RdpConnection sender, bool multiTouchEnabled)
        {
            _instrument.Instrument("OnMultiTouchEnabledChanged");

            _eventProxy.EmitMultiTouchEnabledChanged(this, new MultiTouchEnabledChangedArgs(multiTouchEnabled));            
        }

        void OnConnectionHealthStateChangedHandler(RdClientCx.RdpConnection sender, int connectionState)
        {
            _instrument.Instrument("OnConnectionHealthStateChangedHandler");

            _eventProxy.EmitConnectionHealthStateChanged(this, new ConnectionHealthStateChangedArgs(connectionState));            
        }

        void OnClientAutoReconnectingHandler(RdClientCx.RdpConnection sender, int disconnectReason, int attemptCount, out bool continueReconnecting)
        {
            _instrument.Instrument("OnClientAutoReconnectingHandler");

            bool _continueReconnecting = false;
            ClientAutoReconnectingContinueDelegate shouldContinue = (__continueReconnecting) => { _continueReconnecting = __continueReconnecting; };

            _eventProxy.EmitClientAutoReconnecting(this, new ClientAutoReconnectingArgs(new AutoReconnectError(disconnectReason), attemptCount, shouldContinue));

            continueReconnecting = _continueReconnecting;
        }

        void OnClientAutoReconnectCompleteHandler(RdClientCx.RdpConnection sender)
        {
            _instrument.Instrument("OnClientAutoReconnectCompleteHandler");

            _eventProxy.EmitClientAutoReconnectComplete(this, new ClientAutoReconnectCompleteArgs());
        }

        void OnLoginCompletedHandler(RdClientCx.RdpConnection sender)
        {
            _instrument.Instrument("OnLoginCompletedHandler");

            _eventProxy.EmitLoginCompleted(this, new LoginCompletedArgs());
        }

        void OnStatusInfoReceivedHandler(RdClientCx.RdpConnection sender, int statusCode)
        {
            _instrument.Instrument("OnStatusInfoReceivedHandler");

            _eventProxy.EmitStatusInfoReceived(this, new StatusInfoReceivedArgs(statusCode));
        }

        void OnFirstGraphicsUpdateHandler(RdClientCx.RdpConnection sender)
        {
            _instrument.Instrument("OnFirstGraphicsUpdateHandler");

            _eventProxy.EmitFirstGraphicsUpdate(this, new FirstGraphicsUpdateArgs());
        }

        void OnRemoteAppWindowCreatedHandler(RdClientCx.RdpConnection sender, uint windowId, string title, byte[] icon, uint iconWidth, uint iconHeight)
        {
            _instrument.Instrument("OnRemoteAppWindowCreatedHandler");

            _eventProxy.EmitRemoteAppWindowCreated(this, new RemoteAppWindowCreatedArgs(windowId, title, icon, iconWidth, iconHeight));
        }

        void OnRemoteAppWindowDeletedHandler(RdClientCx.RdpConnection sender, uint windowId)
        {
            _instrument.Instrument("OnRemoteAppWindowDeletedHandler");

            _eventProxy.EmitRemoteAppWindowDeleted(this, new RemoteAppWindowDeletedArgs(windowId));
        }

        void OnRemoteAppWindowTitleUpdatedHandler(RdClientCx.RdpConnection sender, uint windowId, string title)
        {
            _instrument.Instrument("OnRemoteAppWindowTitleUpdatedHandler");

            _eventProxy.EmitRemoteAppWindowTitleUpdated(this, new RemoteAppWindowTitleUpdatedArgs(windowId, title));
        }

        void OnRemoteAppWindowIconUpdatedHandler(RdClientCx.RdpConnection sender, uint windowId, byte[] icon, uint iconWidth, uint iconHeight)
        {
            _instrument.Instrument("OnRemoteAppWindowIconUpdatedHandler");

            _eventProxy.EmitRemoteAppWindowIconUpdated(this, new RemoteAppWindowIconUpdatedArgs(windowId, icon, iconWidth, iconHeight));
        }

        public IRdpCertificate GetServerCertificate()
        {
            _instrument.Instrument("GetServerCertificate");

            RdpCertificate rdpCertificate = null;

            RdClientCx.ServerCertificateError certErrors;
            Certificate cert = null;
            _rdpConnectionCx.GetServerCertificateDetails(out cert);
            if (null != cert)
            {
                _rdpConnectionCx.GetServerCertificateValidationErrors(out certErrors);
                rdpCertificate = new RdpCertificate(cert, certErrors);
            }

            return rdpCertificate;
        }


    }
}
