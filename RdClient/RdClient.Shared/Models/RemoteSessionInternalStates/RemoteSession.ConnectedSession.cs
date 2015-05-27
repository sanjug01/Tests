namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Telemetry;
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;

    partial class RemoteSession
    {
        private sealed class ConnectedSession : InternalState
        {
            private static readonly uint ThumbnailHeight = 276;

            private readonly IRdpConnection _connection;
            private readonly IThumbnailEncoder _thumbnailEncoder;
            private ITelemetryStopwatch _totalTime;
            private Snapshotter _snapshotter;
            private RemoteSession _session;

            public override void Activate(RemoteSession session)
            {
                Contract.Assert(null == _session);

                using (LockWrite())
                {
                    _thumbnailEncoder.ThumbnailUpdated += this.OnThumbnailUpdated;
                    _session = session;
                    _session._state.SetReconnectAttempt(0);
                    _session._syncEvents.ClientAsyncDisconnect += this.OnClientAsyncDisconnect;
                    _session._syncEvents.ConnectionHealthStateChanged += this.OnConnectionHealthStateChanged;
                    _session._syncEvents.ClientAutoReconnecting += this.OnClientAutoReconnecting;
                    _session._syncEvents.MouseCursorShapeChanged += this.OnMouseCursorShapeChanged;
                    _session._syncEvents.MultiTouchEnabledChanged += this.OnMultiTouchEnabledChanged;
                    _session._syncEvents.ClientDisconnected += this.OnClientDisconnected;

                    _snapshotter = new Snapshotter(_connection,
                        _session._syncEvents,
                        _thumbnailEncoder,
                        _session._timerFactory,
                        _session._sessionSetup.DataModel.Settings);
                    _snapshotter.Activate();

                    Contract.Assert(null == _totalTime);
                    _totalTime = this.TelemetryClient.StartStopwatch();
                }
            }

            private void OnMultiTouchEnabledChanged(object sender, MultiTouchEnabledChangedArgs e)
            {
                _session.DeferEmitMultiTouchEnabledChanged(e);
            }

            private void OnMouseCursorShapeChanged(object sender, MouseCursorShapeChangedArgs e)
            {
                _session.DeferEmitMouseCursorShapeChanged(e);
            }

            public override void Deactivate(RemoteSession session)
            {
            }

            public override void Complete(RemoteSession session)
            {
                Contract.Assert(object.ReferenceEquals(_session, session));

                using (LockWrite())
                {
                    Contract.Assert(null == _totalTime);

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
                    _session._syncEvents.ClientDisconnected -= this.OnClientDisconnected;
                    _session._syncEvents.ClientAutoReconnecting -= this.OnClientAutoReconnecting;
                    _session._syncEvents.ConnectionHealthStateChanged -= this.OnConnectionHealthStateChanged;
                    _session._syncEvents.MouseCursorShapeChanged -= this.OnMouseCursorShapeChanged;
                    _session._syncEvents.MultiTouchEnabledChanged -= this.OnMultiTouchEnabledChanged;
                    _session._syncEvents.ClientAsyncDisconnect -= this.OnClientAsyncDisconnect;
                    _session = null;
                }
            }

            public override void Terminate(RemoteSession session)
            {
                using (LockWrite())
                {
                    _thumbnailEncoder.ThumbnailUpdated -= this.OnThumbnailUpdated;
                    //
                    // Deactivate the snapshotter so it unsubscribes from all connection events.
                    //
                    _snapshotter.Deactivate();
                    _snapshotter = null;
                }

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
                        // similar to Autoreconnecting event
                        _session.InternalSetState(new ReconnectingSession(_connection, this));
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
                        // Try to reconnect, go through the reconnect UI.
                        //
                        _connection.HandleAsyncDisconnectResult(e.DisconnectReason, true);
                        break;
                }
            }

            private void OnClientAutoReconnecting(object sender, ClientAutoReconnectingArgs e)
            {
                e.ContinueDelegate(true);
                _session.InternalSetState(new ReconnectingSession(_connection, this));
            }

            private void OnClientDisconnected(object sender, ClientDisconnectedArgs e)
            {
                Contract.Assert(null != _totalTime);

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

                _totalTime.Stop("TotalConnectedTime");
                _totalTime = null;
                _session.InternalSetState(newState);
            }

            private void OnThumbnailUpdated(object sender, ThumbnailUpdatedEventArgs e)
            {
                _session.InternalDeferUpdateSnapshot(e.EncodedImageBytes);
            }
        }
    }
}
