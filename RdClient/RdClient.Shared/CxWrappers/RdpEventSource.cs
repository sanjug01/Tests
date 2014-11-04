using System;

namespace RdClient.Shared.CxWrappers
{
    public class RdpEventSource : IRdpEventSource, IRdpEvents
    {
        public event EventHandler<ClientConnectedArgs> ClientConnected;

        public event EventHandler<ClientAsyncDisconnectArgs> ClientAsyncDisconnect;

        public event EventHandler<ClientDisconnectedArgs> ClientDisconnected;

        public event EventHandler<UserCredentialsRequestArgs> UserCredentialsRequest;

        public event EventHandler<MouseCursorShapeChangedArgs> MouseCursorShapeChanged;

        public event EventHandler<MouseCursorPositionChangedArgs> MouseCursorPositionChanged;

        public event EventHandler<MultiTouchEnabledChangedArgs> MultiTouchEnabledChanged;

        public event EventHandler<ConnectionHealthStateChangedArgs> ConnectionHealthStateChanged;

        public event EventHandler<ClientAutoReconnectingArgs> ClientAutoReconnecting;

        public event EventHandler<ClientAutoReconnectCompleteArgs> ClientAutoReconnectComplete;

        public event EventHandler<LoginCompletedArgs> LoginCompleted;

        public event EventHandler<StatusInfoReceivedArgs> StatusInfoReceived;

        public event EventHandler<FirstGraphicsUpdateArgs> FirstGraphicsUpdate;

        public event EventHandler<RemoteAppWindowCreatedArgs> RemoteAppWindowCreated;

        public event EventHandler<RemoteAppWindowDeletedArgs> RemoteAppWindowDeleted;

        public event EventHandler<RemoteAppWindowTitleUpdatedArgs> RemoteAppWindowTitleUpdated;

        public event EventHandler<RemoteAppWindowIconUpdatedArgs> RemoteAppWindowIconUpdated;

        public void EmitClientConnected(IRdpConnection sender, ClientConnectedArgs args)
        {
            if (ClientConnected != null)
            {
                ClientConnected(sender, args);
            }
        }

        public void EmitClientAsyncDisconnect(IRdpConnection sender, ClientAsyncDisconnectArgs args)
        {
            if(ClientAsyncDisconnect != null)
            {
                ClientAsyncDisconnect(sender, args);
            }
        }

        public void EmitClientDisconnected(IRdpConnection sender, ClientDisconnectedArgs args)
        {
            if(ClientDisconnected != null)
            {
                ClientDisconnected(sender, args);
            }
        }

        public void EmitUserCredentialsRequest(IRdpConnection sender, UserCredentialsRequestArgs args)
        {
            if(UserCredentialsRequest != null)
            {
                UserCredentialsRequest(sender, args);
            }
        }

        public void EmitMouseCursorShapeChanged(IRdpConnection sender, MouseCursorShapeChangedArgs args)
        {
            if(MouseCursorShapeChanged != null)
            {
                MouseCursorShapeChanged(sender, args);
            }
        }

        public void EmitMouseCursorPositionChanged(IRdpConnection sender, MouseCursorPositionChangedArgs args)
        {
            if(MouseCursorPositionChanged != null)
            {
                MouseCursorPositionChanged(sender, args);            
            }
        }

        public void EmitMultiTouchEnabledChanged(IRdpConnection sender, MultiTouchEnabledChangedArgs args)
        {
            if(MultiTouchEnabledChanged != null)
            {
                MultiTouchEnabledChanged(sender, args);
            }
        }

        public void EmitConnectionHealthStateChanged(IRdpConnection sender, ConnectionHealthStateChangedArgs args)
        {
            if(ConnectionHealthStateChanged != null)
            {
                ConnectionHealthStateChanged(sender, args);
            }
        }

        public void EmitClientAutoReconnecting(IRdpConnection sender, ClientAutoReconnectingArgs args)
        {
            if(ClientAutoReconnecting != null)
            {
                ClientAutoReconnecting(sender, args);
            }
        }

        public void EmitClientAutoReconnectComplete(IRdpConnection sender, ClientAutoReconnectCompleteArgs args)
        {
            if (ClientAutoReconnectComplete != null)
            {
                ClientAutoReconnectComplete(sender, args);
            }
        }

        public void EmitLoginCompleted(IRdpConnection sender, LoginCompletedArgs args)
        {
            if(LoginCompleted != null)
            {
                LoginCompleted(sender, args);
            }
        }

        public void EmitStatusInfoReceived(IRdpConnection sender, StatusInfoReceivedArgs args)
        {
            if(StatusInfoReceived != null)
            {
                StatusInfoReceived(sender, args);
            }
        }

        public void EmitFirstGraphicsUpdate(IRdpConnection sender, FirstGraphicsUpdateArgs args)
        {
            if(FirstGraphicsUpdate != null)
            {
                FirstGraphicsUpdate(sender, args);
            }
        }

        public void EmitRemoteAppWindowCreated(IRdpConnection sender, RemoteAppWindowCreatedArgs args)
        {
            if(RemoteAppWindowCreated != null)
            {
                RemoteAppWindowCreated(sender, args);            
            }
        }

        public void EmitRemoteAppWindowDeleted(IRdpConnection sender, RemoteAppWindowDeletedArgs args)
        {
            if(RemoteAppWindowDeleted != null)
            {
                RemoteAppWindowDeleted(sender, args);
            }
        }

        public void EmitRemoteAppWindowTitleUpdated(IRdpConnection sender, RemoteAppWindowTitleUpdatedArgs args)
        {
            if (RemoteAppWindowTitleUpdated != null)
            {
                RemoteAppWindowTitleUpdated(sender, args);            
            }
        }

        public void EmitRemoteAppWindowIconUpdated(IRdpConnection sender, RemoteAppWindowIconUpdatedArgs args)
        {
            if (RemoteAppWindowIconUpdated != null)
            {
                RemoteAppWindowIconUpdated(sender, args);
            }
        }
    }
}
