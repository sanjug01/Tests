using System;
namespace RdClient.Shared.CxWrappers
{
    public interface IRdpEventHandlers
    {
        void OnClientConnectedHandler(IRdpConnection sender);
        void OnClientAsyncDisconnectHandler(IRdpConnection sender, RdpDisconnectReason disconnectReason);
        void OnClientDisconnectedHandler(IRdpConnection sender, RdpDisconnectReason disconnectReason);
        void OnUserCredentialsRequestHandler(IRdpConnection sender, int secLayer);
        void OnMouseCursorShapeChanged(IRdpConnection sender, byte[] buffer, int width, int height, int xHotspot, int yHotspot);
        void OnMouseCursorPositionChanged(IRdpConnection sender, int xPos, int yPos);
        void OnMultiTouchEnabledChanged(IRdpConnection sender, bool multiTouchEnabled);
        void OnConnectionHealthStateChangedHandler(IRdpConnection sender, int connectionState);
        void OnClientAutoReconnectingHandler(IRdpConnection sender, int disconnectReason, int attemptCount, out bool continueReconnecting);
        void OnClientAutoReconnectCompleteHandler(IRdpConnection sender);
        void OnLoginCompletedHandler(IRdpConnection sender);
        void OnStatusInfoReceivedHandler(IRdpConnection sender, int statusCode);
        void OnFirstGraphicsUpdateHandler(IRdpConnection sender);
        void OnRemoteAppWindowCreatedHandler(IRdpConnection sender, UInt32 windowId, String title, byte[] icon, UInt32 iconWidth, UInt32 iconHeight);
        void OnRemoteAppWindowDeletedHandler(IRdpConnection sender, UInt32 windowId);
        void OnRemoteAppWindowTitleUpdatedHandler(IRdpConnection sender, UInt32 windowId, String title);
        void OnRemoteAppWindowIconUpdatedHandler(IRdpConnection sender, UInt32 windowId, byte[] icon, UInt32 iconWidth, UInt32 iconHeight);
    }
}
