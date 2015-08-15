namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using System.Diagnostics.Contracts;

    partial class RemoteSession
    {
        private sealed class ConnectedSession : InternalState
        {
            private static readonly uint ThumbnailHeight = 296;

            private readonly IRdpConnection _connection;
            private readonly IThumbnailEncoder _thumbnailEncoder;
            private Snapshotter _snapshotter;

            protected override void Activated()
            {
                _thumbnailEncoder.ThumbnailUpdated += this.OnThumbnailUpdated;

                this.Session._state.SetReconnectAttempt(0);
                this.Session._syncEvents.ClientAsyncDisconnect += this.OnClientAsyncDisconnect;
                this.Session._syncEvents.ConnectionHealthStateChanged += this.OnConnectionHealthStateChanged;
                this.Session._syncEvents.ClientAutoReconnecting += this.OnClientAutoReconnecting;
                this.Session._syncEvents.MouseCursorShapeChanged += this.OnMouseCursorShapeChanged;
                this.Session._syncEvents.MultiTouchEnabledChanged += this.OnMultiTouchEnabledChanged;
                this.Session._syncEvents.ClientDisconnected += this.OnClientDisconnected;

                _snapshotter = new Snapshotter(_connection,
                    this.Session._syncEvents,
                    _thumbnailEncoder,
                    this.Session._timerFactory,
                    this.Session._sessionSetup.DataModel.Settings);
                _snapshotter.Activate();

                if(!this.Session._hasConnected)
                {
                    this.SessionDuration.Start();
                    this.Session._hasConnected = true;
                }
            }

            private void OnMultiTouchEnabledChanged(object sender, MultiTouchEnabledChangedArgs e)
            {
                this.Session.DeferEmitMultiTouchEnabledChanged(e);
            }

            private void OnMouseCursorShapeChanged(object sender, MouseCursorShapeChangedArgs e)
            {
                this.Session.DeferEmitMouseCursorShapeChanged(e);
            }

            protected override void Completed()
            {
                if (null != _snapshotter)
                {
                    _thumbnailEncoder.ThumbnailUpdated -= this.OnThumbnailUpdated;
                    //
                    // Deactivate the snapshotter so it unsubscribes from all connection events.
                    //
                    _snapshotter.Deactivate();
                    _snapshotter = null;
                }
                //
                // Stop tracking the session events;
                //
                this.Session._syncEvents.ClientDisconnected -= this.OnClientDisconnected;
                this.Session._syncEvents.ClientAutoReconnecting -= this.OnClientAutoReconnecting;
                this.Session._syncEvents.ConnectionHealthStateChanged -= this.OnConnectionHealthStateChanged;
                this.Session._syncEvents.MouseCursorShapeChanged -= this.OnMouseCursorShapeChanged;
                this.Session._syncEvents.MultiTouchEnabledChanged -= this.OnMultiTouchEnabledChanged;
                this.Session._syncEvents.ClientAsyncDisconnect -= this.OnClientAsyncDisconnect;
            }

            protected override void Terminate()
            {
                _thumbnailEncoder.ThumbnailUpdated -= this.OnThumbnailUpdated;
                //
                // Deactivate the snapshotter so it unsubscribes from all connection events.
                //
                _snapshotter.Deactivate();
                _snapshotter = null;

                _connection.Disconnect();
            }

            public ConnectedSession(IRdpConnection connection, InternalState otherState)
                : base(SessionState.Connected, otherState)
            {
                _connection = connection;
                _thumbnailEncoder = ThumbnailEncoder.Create(ThumbnailHeight);
            }

            private void OnConnectionHealthStateChanged(object sender, ConnectionHealthStateChangedArgs e)
            {
                Contract.Assert(object.ReferenceEquals(_connection, sender));
                using (LockWrite())
                {
                    if ((int)RdClientCx.ConnectionHealthState.Warn == e.ConnectionState)
                    {
                        //
                        // similar to Autoreconnecting event
                        //
                        this.Session.InternalSetState(new ReconnectingSession(_connection, this));
                    }
                }
            }

            private void OnClientAsyncDisconnect(object sender, ClientAsyncDisconnectArgs e)
            {
                Contract.Assert(object.ReferenceEquals(_connection, sender));
                switch(e.DisconnectReason.Code)
                {
                    case RdpDisconnectCode.UserInitiated:
                        //
                        // Do not reconnect, let the connection to terminate and call OnClientDisconnected
                        //
                        _connection.HandleAsyncDisconnectResult(e.DisconnectReason, false);
                        break;

                    case RdpDisconnectCode.ReplacedByOtherConnection:
                        //
                        // TODO: ask if the other connection can connect, perhaps
                        //
                        _connection.HandleAsyncDisconnectResult(e.DisconnectReason, false);
                        break;

                    default:
                        //
                        // Let the connection handle the reconnect; auto reconnect events may be emitted separately
                        //
                        _connection.HandleAsyncDisconnectResult(e.DisconnectReason, false);
                        break;
                }
            }

            private void OnClientAutoReconnecting(object sender, ClientAutoReconnectingArgs e)
            {
                e.ContinueDelegate(true);
                ChangeState(new ReconnectingSession(_connection, this));
            }

            private void OnClientDisconnected(object sender, ClientDisconnectedArgs e)
            {
                InternalState newState;

                switch (e.DisconnectReason.Code)
                {
                    case RdpDisconnectCode.UserInitiated:
                        newState = new ClosedSession(_connection, this);
                        break;

                    default:
                        newState = new FailedSession(_connection, e.DisconnectReason, this);
                        break;
                }

                ChangeState(newState);
            }

            private void OnThumbnailUpdated(object sender, ThumbnailUpdatedEventArgs e)
            {
                this.Session.InternalDeferUpdateSnapshot(e.EncodedImageBytes);
            }
        }
    }
}
