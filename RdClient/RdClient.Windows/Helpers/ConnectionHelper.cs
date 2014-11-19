using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RdClientCx;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace RdClient.Helpers
{
    /// <summary>
    /// prototype class to handle one single sessions's connect/disconnect
    /// This functionality is implemented in PageRemoteSession.xaml.cs for the WP project
    /// </summary>
    public class ConnectionHelper
    {
        public ConnectionHelper()
        {
            this.IsConnected = false;

            // default connection properties
            this.UserPassword = "1234AbCd";
            this.HostName = "aj-vm32.corp.microsoft.com";
            this.UserName = "tstestadmin1";

            this.IsConsoleMode = false;
            this.IsLefthandedMouse = false;
            this.AudioMode = 0;

            this.HasGatheredCreds = true;
            this.IsUsingSavedCreds = true;
        }

        /// <summary>
        /// Main connect method
        /// </summary>
        public void StartConnection()
        {
            // new connection steps
            ResetInternalState();
            AddEventHandlers();
            InitializeUserControls();
            UnhideAllUIElements();
            // .... plus zoom/size/center calculations

            // *** connection part ***/
            InitiateConnection();

            this.IsConnected  = true;
        }

        /// <summary>
        /// main disconnect method
        /// </summary>
        public void TerminateConnection()
        {

            CleanUpConnection();

            TerminateUserControls();

            RemoveEventHandlers();

            
            // TODO : other UI update 

            //
            // Force a garbage collection to ensure that all memory
            // is cleaned up before we return to the connection center
            // and start another connection.
            //
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();


            this.IsConnected = false;
        }

        public bool IsConnected { get; protected set; }

        public SwapChainPanel SessionSwapChainPanel 
        {
            get { return _sessionSwapChainPannel; }
            set { _sessionSwapChainPannel = value; } 
        }

        #region ConnectionProperties
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public bool IsLefthandedMouse { get; set; }
        public bool IsConsoleMode { get; set; }
        public int AudioMode { get; set; }
        public bool HasGatheredCreds { get; set; }
        public bool IsUsingSavedCreds { get; set; }
        #endregion


        #region RequiredMethods
        private bool InitiateConnection()
        {
            int xRes;
            bool isPublishedResource;

            RdTrace.IfCondThrow(null != _spConnection, "Cannot initiate a new connection without first cleaning up the old connection.");

            // TODO: Connection's Settings ------ from _mainViewModel.ActiveDesktop  
            //      should initialize HostName;

            // TODO: Credentials settings  - saved, default os new credentials 
            //      should iniitialize: HasGatheredCreds, IsUsingSavedCreds , UserName, UserPassword

            //
            // Add the connection to the store.
            //
            xRes =  RdClientCx.RdpConnectionStore.GetConnectionStore(
                out _spConnectionStore
                );
            RdTrace.IfFailXResultThrow(xRes, "Unable to retrieve the connection store.");

            // SwapChainPAnnel is mandatory
            if (null == this.SessionSwapChainPanel)
            {
                return false;
            }
            xRes = _spConnectionStore.SetSwapChainPanel(this.SessionSwapChainPanel);
            RdTrace.IfFailXResultThrow(xRes, "Unable to set the swap chain panel.");

            // TODO: published resources not supported
            // String rdpFileContents = (m_activeDesktop.ResourceRdpFile != null) ? m_activeDesktop.ResourceRdpFile : String.Empty;
            // fPublishedResource = m_activeDesktop.IsPublishedResource;
            String rdpFileContents = String.Empty;
            isPublishedResource = false;

            xRes = _spConnectionStore.CreateConnectionWithSettings(rdpFileContents, out _spConnection);
            RdTrace.IfFailXResultThrow(xRes, "Failed to create a desktop connection with the given settings.");

            // TODO: suport for input handler not supported yet *************************************
            /** 
            xRes = KeyboardInputHandler.CreateInstance(m_spConnection, out m_spKeyboardHandler);
            RdTrace.IfFailXResultThrow(xRes, "Failed to create KeyboardInputHandler.");

            xRes = TouchInteractionHandler.CreateInstance(m_spConnection, out m_spInputHandler);
            RdTrace.IfFailXResultThrow(xRes, "Failed to create TouchInteractionHandler.");

            m_spInputHandler.SwapChainPanelPanned += InputHandler_SwapChainPanelPanned;
            m_spInputHandler.InertiaEnabled += InputHandler_InertiaEnabled;
            */

            // event handlers 
            _spConnection.OnClientConnected += OnClientConnectedHandler;
            _spConnection.OnClientAsyncDisconnect += OnClientAsyncDisconnectHandler;
            _spConnection.OnClientDisconnected += OnClientDisconnectedHandler;
            _spConnection.OnUserCredentialsRequest += OnUserCredentialsRequestHandler;
            _spConnection.OnMouseCursorShapeChanged += OnMouseCursorShapeChanged;
            _spConnection.OnMouseCursorPositionChanged += OnMouseCursorPositionChanged;
            _spConnection.OnMultiTouchEnabledChanged += OnMultiTouchEnabledChanged;

            _spConnection.OnConnectionHealthStateChanged += OnConnectionHealthStateChangedHandler;
            _spConnection.OnClientAutoReconnecting += OnClientAutoReconnectingHandler;
            _spConnection.OnClientAutoReconnectComplete += OnClientAutoReconnectCompleteHandler;
            _spConnection.OnLoginCompleted += OnLoginCompletedHandler;
            _spConnection.OnStatusInfoReceived += OnStatusInfoReceivedHandler;
            _spConnection.OnFirstGraphicsUpdateReceived += OnFirstGraphicsUpdateHandler;
            _spConnection.OnRemoteAppWindowCreated += OnRemoteAppWindowCreatedHandler;
            _spConnection.OnRemoteAppWindowDeleted += OnRemoteAppWindowDeletedHandler;
            _spConnection.OnRemoteAppWindowTitleUpdated += OnRemoteAppWindowTitleUpdatedHandler;
            _spConnection.OnRemoteAppWindowIconUpdated += OnRemoteAppWindowIconUpdatedHandler;

            PrepareConnection(isPublishedResource);

            return true;
        }

        private void PrepareConnection(bool isPublishedResource)
        {
            int xRes;
            int widthInMm = 52;
            int heightInMm = 87;

            if (!isPublishedResource)
            {
                xRes = _spConnection.SetStringProperty("Full Address", this.HostName);
                RdTrace.IfFailXResultThrow(xRes, "Failed to set server address property.");

                xRes = _spConnection.SetBoolProperty("Administrative Session", this.IsConsoleMode);
                RdTrace.IfFailXResultThrow(xRes, "Failed to set administrative session property.");

                xRes = _spConnection.SetIntProperty("AudioMode", this.AudioMode);
                RdTrace.IfFailXResultThrow(xRes, "Failed to set audio mode property.");
            }
            else
            {
                // ignore published resources for now
            }

            //
            // Left-handed mouse is not an RDP file setting.
            //
            xRes = _spConnection.SetLeftHandedMouseMode(this.IsLefthandedMouse);
            RdTrace.IfFailXResultThrow(xRes, "Failed to set left-handed mouse property.");

            // TODO: calculate some size
            /****************************************************
            {
                Size physicalScreenSize;

                PhysicalDeviceAttributes.GetScreenSize(out physicalScreenSize);

                xRes = m_spConnection.SetIntProperty(
                    "PhysicalDesktopWidth",
                    (int)PhysicalDeviceAttributes.InchesToMillimeters(physicalScreenSize.Width)
                    );
                RdTrace.IfFailXResultThrow(xRes, "Failed to set physical desktop width property.");

                xRes = m_spConnection.SetIntProperty(
                    "PhysicalDesktopHeight",
                    (int)PhysicalDeviceAttributes.InchesToMillimeters(physicalScreenSize.Height)
                    );
                RdTrace.IfFailXResultThrow(xRes, "Failed to set physical desktop height property.");
            }
             * *******************************************/
            xRes = _spConnection.SetIntProperty("PhysicalDesktopWidth", widthInMm );
            RdTrace.IfFailXResultThrow(xRes, "Failed to set physical desktop width property.");
            xRes = _spConnection.SetIntProperty("PhysicalDesktopHeight", heightInMm);
            RdTrace.IfFailXResultThrow(xRes, "Failed to set physical desktop height property.");

            ConnectToServer(isSetNewCreds: this.HasGatheredCreds, isUsingSavedCreds: this.IsUsingSavedCreds);
        }

        //
        // Pass in true for fSetNewCreds to apply the page's active credentials to the connection.
        // Pass in false for fSetNewCreds to not pass any creds to the connection (e.g. if the
        // connection object already gathered credentials from a cred prompt, and you want to re-use them).
        //
        private void ConnectToServer(bool isSetNewCreds, bool isUsingSavedCreds, bool isResumeRdpHandshake = false)
        {
            int xRes;

            if (isSetNewCreds)
            {
                xRes = _spConnection.SetUserCredentials(
                    this.UserName,
                    "", // Domain name, passed as part of the user name field
                    this.UserPassword,
                    isUsingSavedCreds
                    );
                RdTrace.IfFailXResultThrow(xRes, "Failed to set user credentials.");
            }

            if (isResumeRdpHandshake)
            {
                // The connection is in a continuable state.
                xRes = _spConnection.ResumeRdpHandshake();
                RdTrace.IfFailXResultThrow(xRes, "ResumeRdpHandshake failed.");
            }
            else
            {
                // **** NOT handling Azure *****
                // if (m_activeDesktop.IsPublishedAzureApp) {.......}

                xRes = _spConnection.Connect();
                RdTrace.IfFailXResultThrow(xRes, "Failed to connect.");
            }

            // ShowClientConnectingUI(m_activeDesktop.DisplayName);
            ShowClientConnectingUI("HelloWorld Connection");
        }

        /// <summary>
        ///  NOT implemented - shows the Connecting UI
        /// </summary>
        /// <param name="hostName"></param>
        private void ShowClientConnectingUI(string hostName) { }

        private void CleanUpConnection()
        {
            int xRes;

            // m_strTempAllowedCertThumbprint = null;
            // m_activeDesktop = null;
            this.HostName = String.Empty;
            this.UserName= String.Empty;
            this.UserPassword = String.Empty;

            if (null != _spConnection)
            {
                //
                // Remove connection from the store.
                //
                if (_spConnectionStore == null)
                {
                    xRes = RdpConnectionStore.GetConnectionStore(out _spConnectionStore);
                    RdTrace.IfFailXResultThrow(xRes, "Unable to retrieve the connection store.");
                }

                xRes = _spConnectionStore.RemoveConnection(_spConnection);
                RdTrace.IfFailXResultThrow(xRes, "Unable to remove the connection from the store.");

                _spConnection.OnClientConnected -= OnClientConnectedHandler;
                _spConnection.OnClientAsyncDisconnect -= OnClientAsyncDisconnectHandler;
                _spConnection.OnClientDisconnected -= OnClientDisconnectedHandler;
                _spConnection.OnUserCredentialsRequest -= OnUserCredentialsRequestHandler;
                _spConnection.OnMouseCursorShapeChanged -= OnMouseCursorShapeChanged;
                _spConnection.OnMouseCursorPositionChanged -= OnMouseCursorPositionChanged;
                _spConnection.OnMultiTouchEnabledChanged -= OnMultiTouchEnabledChanged;
                _spConnection.OnConnectionHealthStateChanged -= OnConnectionHealthStateChangedHandler;
                _spConnection.OnClientAutoReconnecting -= OnClientAutoReconnectingHandler;
                _spConnection.OnClientAutoReconnectComplete -= OnClientAutoReconnectCompleteHandler;
                _spConnection.OnLoginCompleted -= OnLoginCompletedHandler;
                _spConnection.OnStatusInfoReceived -= OnStatusInfoReceivedHandler;
                _spConnection.OnFirstGraphicsUpdateReceived -= OnFirstGraphicsUpdateHandler;
                _spConnection.OnRemoteAppWindowCreated -= OnRemoteAppWindowCreatedHandler;
                _spConnection.OnRemoteAppWindowDeleted -= OnRemoteAppWindowDeletedHandler;
                _spConnection.OnRemoteAppWindowTitleUpdated -= OnRemoteAppWindowTitleUpdatedHandler;
                _spConnection.OnRemoteAppWindowIconUpdated -= OnRemoteAppWindowIconUpdatedHandler;

                //
                // Terminate the connection object.
                //
                _spConnection.TerminateInstance();
                _spConnection = null;

                //
                // Flush any pending traces to disk.
                //
                Tracer.Flush();
            }

            /*************** Ignore input handler *************************************
            if (null != m_spInputHandler)
            {
                m_spInputHandler.SwapChainPanelPanned -= InputHandler_SwapChainPanelPanned;
                m_spInputHandler.InertiaEnabled -= InputHandler_InertiaEnabled;
                m_spInputHandler = null;
            }

            m_spKeyboardHandler = null;
             * ***************************************************************************/

            _spConnectionStore = null;
        }

        #endregion

        #region InternalStateMethods
        private void ResetInternalState()
        {
            RdTrace.IfCondThrow(_spConnectionStore != null, "m_spConnectionStore should be null");
            RdTrace.IfCondThrow(_spConnection != null, "m_spConnection should be null");

            // TODO:
            //RdTrace.IfCondThrow(m_activeDesktop != null, "m_activeDesktop should be null");
            //RdTrace.IfCondThrow(m_spKeyboardHandler != null, "m_spKeyboardHandler should be null");
            //RdTrace.IfCondThrow(m_spInputHandler != null, "m_spInputHandler should be null");
            //RdTrace.IfCondThrow(m_snapshotTimer != null, "m_snapshotTimer should be null");
            //RdTrace.IfCondThrow(m_mousePointerDragZoomTimer != null, "m_mousePointerDragZoomTimer should be null");
            //RdTrace.IfCondThrow(m_mousePointerFlushInputTimer != null, "m_mousePointerFlushInputTimer should be null");
            // ..... plus more
        }

        private void AddEventHandlers()  { /** not RdClientCx **/  }
        private void UnhideAllUIElements() { /** not RdClientCx **/  }
        private void InitializeUserControls() { /** not RdClientCx **/  }
        void TerminateUserControls() { /** not RdClientCx **/ }
        void RemoveEventHandlers() { /** not RdClientCx **/ }


        #region ConectionEventHandlers
        // ConectionEventHandlers are NOT implemented
        private void OnClientConnectedHandler(RdpConnection sender) 
        {
            RdTrace.TraceDbg("OnClientConnectedHandler");
        }

        //
        // This event happens before we are properly disconnected.
        // It gives us an opportunity to inspect the reason for the disconnect,
        // communicate with the user, and optionally request an internal reconnect.
        // OnClientDisconnectedHandler will only be called if we are unable or
        // choose not to reconnect internally.
        //
        // TODO: important to handle, otherwise connection fails for any reason
        private void OnClientAsyncDisconnectHandler(RdpConnection sender, RdpDisconnectReason disconnectReason) 
        { 
            // it is important to handle the async disconnects
            RdTrace.TraceDbg("OnClientAsyncDisconnectHandler");

            // TODO : use dispatcher associated to XAML UI
            // Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                int xRes;
                bool fReconnectToServer;

                switch (disconnectReason.code)
                {
                    case RdpDisconnectCode.PreAuthLogonFailed:
                        {
                            // TODO
                            // fReconnectToServer = await DisplayCredentialPrompt(CredentialPromptModes.InvalidCredentials);
                            fReconnectToServer = true;
                        }
                        break;
                    case RdpDisconnectCode.FreshCredsRequired:
                        {
                            // TODO
                            // fReconnectToServer = await DisplayCredentialPrompt(CredentialPromptModes.NeedsFreshCredentials);
                            fReconnectToServer = true;
                        }
                        break;

                    case RdpDisconnectCode.CertValidationFailed:
                        {
                            // fReconnectToServer = await DisplayServerCertificateErrorDialog();
                            fReconnectToServer = true;
                        }
                        break;

                    case RdpDisconnectCode.CredSSPUnsupported:
                        {
                            // fReconnectToServer = await DisplayNoCredSSPDialog();
                            fReconnectToServer = true;
                        }
                        break;

                    default:
                        {
                            //
                            // For all other reasons, we just disconnect.
                            // We'll handle showing any appropriate dialogs to the user in OnClientDisconnectedHandler.
                            //
                            fReconnectToServer = false;
                        }
                        break;
                }

                RdTrace.IfCondThrow(null == _spConnection, "_spConnection is null.");

                xRes = _spConnection.HandleAsyncDisconnectResult(
                    disconnectReason,
                    fReconnectToServer
                    );
                RdTrace.IfFailXResultThrow(xRes, "HandleAsyncDisconnectResult failed.");
            }
            // ); // end RunAsync
        }


        private void OnClientDisconnectedHandler(RdpConnection sender, RdpDisconnectReason disconnectedReason) 
        {
            RdTrace.TraceDbg("OnClientDisconnectedHandler");
        }
        private void OnUserCredentialsRequestHandler(RdpConnection sender, int iSecLayer) 
        {
            RdTrace.TraceDbg("OnUserCredentialsRequestHandler");
        }
        private void OnMouseCursorShapeChanged(RdpConnection spSender, byte[] spbBuffer, int width, int height, int xHotspot, int yHotspot) { }
        private void OnMouseCursorPositionChanged(RdpConnection spSender, int xPos, int yPos) { }
        private void OnMultiTouchEnabledChanged(RdpConnection spSender, bool fMultiTouchEnabled) { }
        private void OnConnectionHealthStateChangedHandler(RdpConnection spSender, int iConnectionState) { }
        private void OnClientAutoReconnectingHandler(RdpConnection spSender, int iDisconnectReason, int iAttemptCount, out bool pfContinueReconnecting) { pfContinueReconnecting = true; }
        private void OnClientAutoReconnectCompleteHandler(RdpConnection spSender) { }
        private void OnLoginCompletedHandler(RdpConnection spSender) 
        {
            RdTrace.TraceDbg("OnLoginCompletedHandler");
        }
        private void OnStatusInfoReceivedHandler(RdpConnection spSender, int iStatusCode) 
        {
            RdTrace.TraceDbg("OnStatusInfoReceivedHandler. Status code" + iStatusCode);
        }
        private void OnFirstGraphicsUpdateHandler(RdpConnection sender) { }
        private void OnRemoteAppWindowCreatedHandler(RdpConnection sender, UInt32 windowId, String title, byte[] pbIcon, UInt32 iconWidth, UInt32 iconHeight) { }
        private void OnRemoteAppWindowDeletedHandler(RdpConnection sender, UInt32 windowId) { }
        private void OnRemoteAppWindowTitleUpdatedHandler(RdpConnection sender, UInt32 windowId, String title) { }
        private void OnRemoteAppWindowIconUpdatedHandler(RdpConnection spSender, UInt32 windowId, byte[] spIcon, UInt32 iconWidth, UInt32 iconHeight) { }
        #endregion

        #endregion

        #region InternalStateMembers
        private RdpConnection _spConnection;
        private RdpConnectionStore _spConnectionStore;
        SwapChainPanel _sessionSwapChainPannel;        

        // TODO:
        // private KeyboardInputHandler m_spKeyboardHandler;
        // private TouchInteractionHandler m_spInputHandler;
        // private SystemIdleDetector m_systemIdleDetector;
        // ..... plus more
        #endregion


    }
}
