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

        event EventHandler<ClientConnectedArgs> IRdpEvents.ClientConnected
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event EventHandler<ClientAsyncDisconnectArgs> IRdpEvents.ClientAsyncDisconnect
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event EventHandler<ClientDisconnectedArgs> IRdpEvents.ClientDisconnected
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event EventHandler<UserCredentialsRequestArgs> IRdpEvents.UserCredentialsRequest
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event EventHandler<MouseCursorShapeChangedArgs> IRdpEvents.MouseCursorShapeChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event EventHandler<MouseCursorPositionChangedArgs> IRdpEvents.MouseCursorPositionChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event EventHandler<MultiTouchEnabledChangedArgs> IRdpEvents.MultiTouchEnabledChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event EventHandler<ConnectionHealthStateChangedArgs> IRdpEvents.ConnectionHealthStateChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event EventHandler<ClientAutoReconnectingArgs> IRdpEvents.ClientAutoReconnecting
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event EventHandler<ClientAutoReconnectCompleteArgs> IRdpEvents.ClientAutoReconnectComplete
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event EventHandler<LoginCompletedArgs> IRdpEvents.LoginCompleted
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event EventHandler<StatusInfoReceivedArgs> IRdpEvents.StatusInfoReceived
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event EventHandler<FirstGraphicsUpdateArgs> IRdpEvents.FirstGraphicsUpdate
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event EventHandler<RemoteAppWindowCreatedArgs> IRdpEvents.RemoteAppWindowCreated
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event EventHandler<RemoteAppWindowDeletedArgs> IRdpEvents.RemoteAppWindowDeleted
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event EventHandler<RemoteAppWindowTitleUpdatedArgs> IRdpEvents.RemoteAppWindowTitleUpdated
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event EventHandler<RemoteAppWindowIconUpdatedArgs> IRdpEvents.RemoteAppWindowIconUpdated
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event EventHandler<CheckGatewayCertificateTrustArgs> IRdpEvents.CheckGatewayCertificateTrust
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }
#pragma warning restore 67 // warning CS0067: the event <...> is never used
    }
}
