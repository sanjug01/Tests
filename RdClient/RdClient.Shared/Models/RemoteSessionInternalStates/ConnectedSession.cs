namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using System;
    using System.Diagnostics.Contracts;

    partial class RemoteSession
    {
        private sealed class ConnectedSession : InternalState
        {
            private static readonly uint ThumbnailHeight = 276;

            private readonly IRdpConnection _connection;
            private readonly IThumbnailEncoder _thumbnailEncoder;
            private readonly Snapshotter _snapshotter;
            private RemoteSession _session;

            public override void Activate(RemoteSession session)
            {
                Contract.Assert(null == _session);

                using (LockWrite())
                {
                    _thumbnailEncoder.ThumbnailUpdated += this.OnThumbnailUpdated;
                    _session = session;
                    _session._state.SetReconnectAttempt(0);
                    _session._state.SetReconnectAttempt(0);
                    _session._syncEvents.ClientAutoReconnecting += this.OnClientAutoReconnecting;
                    _session._syncEvents.ClientDisconnected += this.OnClientDisconnected;
                }
            }

            public override void Deactivate(RemoteSession session)
            {
            }

            public override void Complete(RemoteSession session)
            {
                Contract.Assert(object.ReferenceEquals(_session, session));

                using (LockWrite())
                {
                    _thumbnailEncoder.ThumbnailUpdated -= this.OnThumbnailUpdated;
                    _session._syncEvents.ClientAutoReconnecting -= this.OnClientAutoReconnecting;
                    _session._syncEvents.ClientDisconnected -= this.OnClientDisconnected;
                    _session = null;
                }
            }

            public override void Terminate(RemoteSession session)
            {
                _connection.Disconnect();
            }

            public ConnectedSession(IRdpConnection connection, InternalState otherState)
                : base(SessionState.Connected, otherState)
            {
                _connection = connection;
                _thumbnailEncoder = ThumbnailEncoder.Create(ThumbnailHeight);
                _snapshotter = new Snapshotter(_connection, _thumbnailEncoder, _session._timerFactory, _session._sessionSetup.DataModel.Settings);
            }

            private void OnClientAutoReconnecting(object sender, ClientAutoReconnectingArgs e)
            {
                using (LockUpgradeableRead())
                {
                    e.ContinueDelegate(true);
                    _session.InternalSetState(new ReconnectingSession(_connection, this));
                }
            }

            private void OnClientDisconnected(object sender, ClientDisconnectedArgs e)
            {
                InternalState newState;

                switch(e.DisconnectReason.Code)
                {
                    case RdpDisconnectCode.UserInitiated:
                        newState = new ClosedSession(this);
                        break;

                    default:
                        newState = new FailedSession(e.DisconnectReason, this);
                        break;
                }

                _session.InternalSetState(newState);
            }

            private void OnThumbnailUpdated(object sender, ThumbnailUpdatedEventArgs e)
            {
                _session.InternalDeferUpdateSnapshot(e.EncodedImageBytes);
            }
        }
    }
}
