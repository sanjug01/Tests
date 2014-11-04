using System;
namespace RdClient.Shared.CxWrappers
{
    public class ClientConnectedArgs : EventArgs
    {
    }

    public class ClientAsyncDisconnectArgs : EventArgs
    {
        public RdpDisconnectReason DisconnectReason { get; private set; }
        public ClientAsyncDisconnectArgs(RdpDisconnectReason rdpDisconnectReason)
        {
            DisconnectReason = rdpDisconnectReason;
        }
    }

    public class ClientDisconnectedArgs : EventArgs
    {
        public RdpDisconnectReason DisconnectReason { get; private set; }
        public ClientDisconnectedArgs(RdpDisconnectReason rdpDisconnectReason)
        {
            DisconnectReason = rdpDisconnectReason;
        }
    }

    public class UserCredentialsRequestArgs : EventArgs
    {
        public int SecLayer { get; private set; }
        public UserCredentialsRequestArgs(int secLayer)
        {
            SecLayer = secLayer;
        }
    }

    public class MouseCursorShapeChangedArgs : EventArgs
    {
        public byte[] Buffer { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int XHotspot { get; private set; }
        public int YHotspot { get; private set; }
        public MouseCursorShapeChangedArgs(byte[] buffer, int width, int height, int xHotspot, int yHotspot)
        {
            Buffer = buffer;
            Width = width;
            Height = height;
            XHotspot = xHotspot;
            YHotspot = yHotspot;
        }
    }
    public class MouseCursorPositionChangedArgs : EventArgs
    {
        public byte[] Buffer { get; private set; }
        public int XPos { get; private set; }
        public int YPos { get; private set; }

        public MouseCursorPositionChangedArgs(int xPos, int yPos)
        {
            XPos = xPos;
            YPos = yPos;
        }
    }

    public class MultiTouchEnabledChangedArgs : EventArgs
    {
        public bool MultiTouchEnabled { get; private set; }

        public MultiTouchEnabledChangedArgs(bool multiTouchEnabled)
        {
            MultiTouchEnabled = multiTouchEnabled;
        }
    }

    public class ConnectionHealthStateChangedArgs : EventArgs
    {
        public int ConnectionState { get; private set; }

        public ConnectionHealthStateChangedArgs(int connectionState)
        {
            ConnectionState = connectionState;
        }
    }


    public delegate void ClientAutoReconnectingContinueDelegate(bool continueReconnecting);

    public class ClientAutoReconnectingArgs : EventArgs
    {
        public int DisconnectReason { get; private set; }
        public int AttemptCount { get; private set; }
        public ClientAutoReconnectingContinueDelegate ContinueDelegate { get; private set; }

        public ClientAutoReconnectingArgs(int disconnectReason, int attemptCount, ClientAutoReconnectingContinueDelegate continueDelegate)
        {
            DisconnectReason = disconnectReason;
            AttemptCount = attemptCount;
            ContinueDelegate = continueDelegate;
        }
    }

    public class ClientAutoReconnectCompleteArgs : EventArgs
    {
    }

    public class LoginCompletedArgs : EventArgs
    {
    }

    public class StatusInfoReceivedArgs : EventArgs
    {
        public int StatusCode { get; private set; }

        public StatusInfoReceivedArgs(int statusCode)
        {
            StatusCode = statusCode;
        }
    }

    public class FirstGraphicsUpdateArgs : EventArgs
    {
    }

    public class RemoteAppWindowCreatedArgs : EventArgs
    {
        public UInt32 WindowId { get; private set; }
        public String Title { get; private set; }
        public byte[] Icon { get; private set; }
        public UInt32 IconWidth { get; private set; }
        public UInt32 IconHeight { get; private set; }

        public RemoteAppWindowCreatedArgs(UInt32 windowId, String title, byte[] icon, UInt32 iconWidth, UInt32 iconHeight)
        {
            WindowId = windowId;
            Title = title;
            Icon = icon;
            IconWidth = iconWidth;
            IconHeight = iconHeight;
        }
    }

    public class RemoteAppWindowDeletedArgs : EventArgs
    {
        public UInt32 WindowId { get; private set; }

        public RemoteAppWindowDeletedArgs(UInt32 windowId)
        {
            WindowId = windowId;
        }
    }

    public class RemoteAppWindowTitleUpdatedArgs : EventArgs
    {
        public UInt32 WindowId { get; private set; }
        public String Title { get; private set; }

        public RemoteAppWindowTitleUpdatedArgs(UInt32 windowId, String title)
        {
            WindowId = windowId;
            Title = title;
        }
    }

    public class RemoteAppWindowIconUpdatedArgs : EventArgs
    {
        public UInt32 WindowId { get; private set; }
        public byte[] Icon { get; private set; }
        public UInt32 IconWidth { get; private set; }
        public UInt32 IconHeight { get; private set; }

        public RemoteAppWindowIconUpdatedArgs(UInt32 windowId, byte[] icon, UInt32 iconWidth, UInt32 iconHeight)
        {
            WindowId = windowId;
            Icon = icon;
            IconWidth = iconWidth;
            IconHeight = iconHeight;
        }
    }

    public interface IRdpEvents
    {
        event EventHandler<ClientConnectedArgs> ClientConnected;
        event EventHandler<ClientAsyncDisconnectArgs> ClientAsyncDisconnect;
        event EventHandler<ClientDisconnectedArgs> ClientDisconnected;
        event EventHandler<UserCredentialsRequestArgs> UserCredentialsRequest;
        event EventHandler<MouseCursorShapeChangedArgs> MouseCursorShapeChanged;
        event EventHandler<MouseCursorPositionChangedArgs> MouseCursorPositionChanged;
        event EventHandler<MultiTouchEnabledChangedArgs> MultiTouchEnabledChanged;
        event EventHandler<ConnectionHealthStateChangedArgs> ConnectionHealthStateChanged;
        event EventHandler<ClientAutoReconnectingArgs> ClientAutoReconnecting;
        event EventHandler<ClientAutoReconnectCompleteArgs> ClientAutoReconnectComplete;
        event EventHandler<LoginCompletedArgs> LoginCompleted;
        event EventHandler<StatusInfoReceivedArgs> StatusInfoReceived;
        event EventHandler<FirstGraphicsUpdateArgs> FirstGraphicsUpdate;
        event EventHandler<RemoteAppWindowCreatedArgs> RemoteAppWindowCreated;
        event EventHandler<RemoteAppWindowDeletedArgs> RemoteAppWindowDeleted;
        event EventHandler<RemoteAppWindowTitleUpdatedArgs> RemoteAppWindowTitleUpdated;
        event EventHandler<RemoteAppWindowIconUpdatedArgs> RemoteAppWindowIconUpdated;
    }

    public interface IRdpEventSource
    {
        void EmitClientConnected(IRdpConnection sender, ClientConnectedArgs args);
        void EmitClientAsyncDisconnect(IRdpConnection sender, ClientAsyncDisconnectArgs args);
        void EmitClientDisconnected(IRdpConnection sender, ClientDisconnectedArgs args);
        void EmitUserCredentialsRequest(IRdpConnection sender, UserCredentialsRequestArgs args);
        void EmitMouseCursorShapeChanged(IRdpConnection sender, MouseCursorShapeChangedArgs args);
        void EmitMouseCursorPositionChanged(IRdpConnection sender, MouseCursorPositionChangedArgs args);
        void EmitMultiTouchEnabledChanged(IRdpConnection sender, MultiTouchEnabledChangedArgs args);
        void EmitConnectionHealthStateChanged(IRdpConnection sender, ConnectionHealthStateChangedArgs args);
        void EmitClientAutoReconnecting(IRdpConnection sender, ClientAutoReconnectingArgs args);
        void EmitClientAutoReconnectComplete(IRdpConnection sender, ClientAutoReconnectCompleteArgs args);
        void EmitLoginCompleted(IRdpConnection sender, LoginCompletedArgs args);
        void EmitStatusInfoReceived(IRdpConnection sender, StatusInfoReceivedArgs args);
        void EmitFirstGraphicsUpdate(IRdpConnection sender, FirstGraphicsUpdateArgs args);
        void EmitRemoteAppWindowCreated(IRdpConnection sender, RemoteAppWindowCreatedArgs args);
        void EmitRemoteAppWindowDeleted(IRdpConnection sender, RemoteAppWindowDeletedArgs args);
        void EmitRemoteAppWindowTitleUpdated(IRdpConnection sender, RemoteAppWindowTitleUpdatedArgs args);
        void EmitRemoteAppWindowIconUpdated(IRdpConnection sender, RemoteAppWindowIconUpdatedArgs args);
    }
}
