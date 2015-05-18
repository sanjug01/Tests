namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using System.Diagnostics.Contracts;

    partial class RemoteSession
    {
        private sealed class ReconnectingSession : InternalState
        {
            private readonly IRdpConnection _connection;
            private RemoteSession _session;
            private bool _cancelled;

            public override void Activate(RemoteSession session)
            {
                Contract.Assert(null == _session);

                using (LockWrite())
                {
                    _session = session;
                    _session._syncEvents.ClientAutoReconnecting += this.OnClientAutoReconnecting;
                    _session._syncEvents.ConnectionHealthStateChanged += this.OnConnectionHealthStateChanged; ;
                    _session._syncEvents.ClientAutoReconnectComplete += this.OnClientAutoReconnectComplete;
                    _session._syncEvents.ClientAsyncDisconnect += this.OnClientAsyncDisconnect;
                    _session._syncEvents.ClientDisconnected += this.OnClientDisconnected;
                    _session.DeferEmitInterrupted(this.Cancel);
                }
            }

            public override void Complete(RemoteSession session)
            {
                Contract.Assert(object.ReferenceEquals(_session, session));

                using (LockWrite())
                {
                    _session._syncEvents.ClientAutoReconnecting -= this.OnClientAutoReconnecting;
                    _session._syncEvents.ClientAutoReconnectComplete -= this.OnClientAutoReconnectComplete;
                    _session._syncEvents.ConnectionHealthStateChanged -= this.OnConnectionHealthStateChanged;
                    _session._syncEvents.ClientAsyncDisconnect -= this.OnClientAsyncDisconnect;
                    _session._syncEvents.ClientDisconnected -= this.OnClientDisconnected;
                    _session = null;
                }
            }

            public override void Terminate(RemoteSession session)
            {
                _cancelled = true;
                _connection.Disconnect();
            }

            public ReconnectingSession(IRdpConnection connection, InternalState otherState)
                : base(SessionState.Interrupted, otherState)
            {
                _connection = connection;
                _cancelled = false;
            }

            private void OnClientAutoReconnecting(object sender, ClientAutoReconnectingArgs e)
            {
                //
                // The event is always delivered on a worker thread, and the caller expects that
                // the ContinueDelegate will be called by the event handler before it returns.
                // Dumb design.
                // Unless we know that the user has cancelled reconnect on the UI thread (Cancel got called
                // and set _cancelled to true), we let the auto-reconnect to proceed but update the reconnect
                // attempt number in the session state.
                //
                using (LockRead())
                {
                    e.ContinueDelegate(!_cancelled);
                    //
                    // Update the attempt number; the update is dispatched to the UI thread.
                    //
                    if(!_cancelled)
                        _session._state.SetReconnectAttempt(e.AttemptCount);
                }
            }

            private void OnConnectionHealthStateChanged(object sender, ConnectionHealthStateChangedArgs e)
            {
                using (LockWrite())
                {
                    if ((int)RdClientCx.ConnectionHealthState.Connected == e.ConnectionState)
                    {
                        // same as reconnecting complete
                        _session.InternalSetState(new ConnectedSession(_connection, this));
                    }
                }
            }

            private void OnClientAutoReconnectComplete(object sender, ClientAutoReconnectCompleteArgs e)
            {
                _session.InternalSetState(new ConnectedSession(_connection, this));
            }

            private void OnClientAsyncDisconnect(object sender, ClientAsyncDisconnectArgs e)
            {
                _connection.HandleAsyncDisconnectResult(e.DisconnectReason, false);
            }

            private void OnClientDisconnected(object sender, ClientDisconnectedArgs e)
            {
                //
                // Set the session state to Failed
                //
                _session.InternalSetState(new FailedSession(_connection, e.DisconnectReason, this));
            }

            private void Cancel()
            {
                using (LockWrite())
                {
                    _cancelled = true;
                    _connection.Disconnect();
                }
            }
        }
    }
}
