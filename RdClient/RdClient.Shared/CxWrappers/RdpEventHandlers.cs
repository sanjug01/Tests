using RdClient.Navigation;
using RdClient.Shared.ViewModels;
using RdClientCx;
using System;

namespace RdClient.Shared.CxWrappers
{
    public class RdpEventHandlers : IRdpEventHandlers
    {
        private SessionViewModel _sessionViewModel;

        public RdpEventHandlers(SessionViewModel sessionViewModel)
        {
            _sessionViewModel = sessionViewModel;
        }

        public void OnClientConnectedHandler(IRdpConnection sender)
        {
        }

        public void OnClientAsyncDisconnectHandler(IRdpConnection sender, RdpDisconnectReason disconnectReason)
        {
            int xRes;
            bool reconnect;

            switch (disconnectReason.code)
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

            xRes = sender.HandleAsyncDisconnectResult(disconnectReason, reconnect);
            RdTrace.IfFailXResultThrow(xRes, "HandleAsyncDisconnectResult failed.");
        }

        public void OnClientDisconnectedHandler(IRdpConnection sender, RdpDisconnectReason disconnectReason)
        {
        }

        public void OnUserCredentialsRequestHandler(IRdpConnection sender, int secLayer)
        {
        }

        public void OnMouseCursorShapeChanged(IRdpConnection sender, byte[] buffer, int width, int height, int xHotspot, int yHotspot)
        {
        }

        public void OnMouseCursorPositionChanged(IRdpConnection sender, int xPos, int yPos)
        {
        }

        public void OnMultiTouchEnabledChanged(IRdpConnection sender, bool multiTouchEnabled)
        {
        }

        public void OnConnectionHealthStateChangedHandler(IRdpConnection sender, int connectionState)
        {
        }

        public void OnClientAutoReconnectingHandler(IRdpConnection sender, int disconnectReason, int attemptCount, out bool continueReconnecting)
        {
            continueReconnecting = true;
        }

        public void OnClientAutoReconnectCompleteHandler(IRdpConnection sender)
        {
        }

        public void OnLoginCompletedHandler(IRdpConnection sender)
        {
        }

        public void OnStatusInfoReceivedHandler(IRdpConnection sender, int statusCode)
        {
        }

        public void OnFirstGraphicsUpdateHandler(IRdpConnection sender)
        {
        }

        public void OnRemoteAppWindowCreatedHandler(IRdpConnection sender, uint windowId, string title, byte[] icon, uint iconWidth, uint iconHeight)
        {
        }

        public void OnRemoteAppWindowDeletedHandler(IRdpConnection sender, uint windowId)
        {
        }

        public void OnRemoteAppWindowTitleUpdatedHandler(IRdpConnection sender, uint windowId, string title)
        {
        }

        public void OnRemoteAppWindowIconUpdatedHandler(IRdpConnection sender, uint windowId, byte[] icon, uint iconWidth, uint iconHeight)
        {
        }
    }
}
