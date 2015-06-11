namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Telemetry;
    using System.Diagnostics.Contracts;

    partial class RemoteSession
    {
        private sealed class ReconnectingSession : InternalState
        {
            private readonly IRdpConnection _connection;
            private ITelemetryStopwatch _reconnectTime;
            private bool _cancelled;

            protected override void Activated()
            {
                Contract.Assert(null == _reconnectTime);

                using (LockWrite())
                {
                    this.Session._syncEvents.ClientAutoReconnecting += this.OnClientAutoReconnecting;
                    this.Session._syncEvents.ConnectionHealthStateChanged += this.OnConnectionHealthStateChanged; ;
                    this.Session._syncEvents.ClientAutoReconnectComplete += this.OnClientAutoReconnectComplete;
                    this.Session._syncEvents.ClientAsyncDisconnect += this.OnClientAsyncDisconnect;
                    this.Session._syncEvents.ClientDisconnected += this.OnClientDisconnected;
                    this.Session.DeferEmitInterrupted(this.Cancel);
                }

                _reconnectTime = this.TelemetryClient.StartStopwatch();
            }

            protected override void Completed()
            {
                Contract.Assert(null == _reconnectTime);

                using (LockWrite())
                {
                    this.Session._syncEvents.ClientAutoReconnecting -= this.OnClientAutoReconnecting;
                    this.Session._syncEvents.ClientAutoReconnectComplete -= this.OnClientAutoReconnectComplete;
                    this.Session._syncEvents.ConnectionHealthStateChanged -= this.OnConnectionHealthStateChanged;
                    this.Session._syncEvents.ClientAsyncDisconnect -= this.OnClientAsyncDisconnect;
                    this.Session._syncEvents.ClientDisconnected -= this.OnClientDisconnected;
                }
            }

            protected override void Terminate()
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
                        this.Session._state.SetReconnectAttempt(e.AttemptCount);
                }
            }

            private void OnConnectionHealthStateChanged(object sender, ConnectionHealthStateChangedArgs e)
            {
                using (LockWrite())
                {
                    if ((int)RdClientCx.ConnectionHealthState.Connected == e.ConnectionState)
                    {
                        _reconnectTime.Stop("ReconnectedTime");
                        _reconnectTime = null;
                        // same as reconnecting complete
                        ChangeState(new ConnectedSession(_connection, this));
                    }
                }
            }

            private void OnClientAutoReconnectComplete(object sender, ClientAutoReconnectCompleteArgs e)
            {
                _reconnectTime.Stop("ReconnectedTime");
                _reconnectTime = null;
                ChangeState(new ConnectedSession(_connection, this));
            }

            private void OnClientAsyncDisconnect(object sender, ClientAsyncDisconnectArgs e)
            {
                _connection.HandleAsyncDisconnectResult(e.DisconnectReason, false);
            }

            private void OnClientDisconnected(object sender, ClientDisconnectedArgs e)
            {
                _reconnectTime = null;
                //
                // Set the session state to Failed
                //
                ChangeState(new FailedSession(_connection, e.DisconnectReason, this));
            }

            private void Cancel()
            {
                using (LockWrite())
                {
                    _reconnectTime.Stop("ReconnectCancellationTime");
                    _reconnectTime = null;
                    this.TelemetryClient.Event("ReconnectCancelled");
                    _cancelled = true;
                    _connection.Disconnect();
                }
            }
        }
    }
}
