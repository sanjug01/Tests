using RdClient.Shared.CxWrappers;
using RdMock;
using System;

namespace RdClient.Shared.Test.Mock
{
    public class RdpEvents : MockBase, IRdpEvents
    {
#pragma warning disable 67 // warning CS0067: the event <...> is never used

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
    }
}
