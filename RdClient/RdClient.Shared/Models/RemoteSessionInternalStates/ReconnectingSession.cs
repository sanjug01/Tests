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
                    _session._syncEvents.ClientAutoReconnectComplete += this.OnClientAutoReconnectComplete;
                    _session._syncEvents.ClientDisconnected += this.OnClientDisconnected;
                    //
                    // TODO:    route the deferral that may be obtained from the "Interrupted" event
                    //          to this object, so if will update _canConnect and close the connection
                    //          if the user so wishes.
                    //
                    _session.DeferEmitInterrupted();
                }
            }

            public override void Complete(RemoteSession session)
            {
                Contract.Assert(object.ReferenceEquals(_session, session));

                using (LockWrite())
                {
                    _session._syncEvents.ClientAutoReconnecting -= this.OnClientAutoReconnecting;
                    _session._syncEvents.ClientAutoReconnectComplete -= this.OnClientAutoReconnectComplete;
                    _session._syncEvents.ClientDisconnected -= this.OnClientDisconnected;
                    _session = null;
                }
            }

            public override void Terminate(RemoteSession session)
            {
                using(LockWrite())
                {
                    _cancelled = true;
                    _connection.Disconnect();
                }
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
                // We let the auto-reconnect to proceed but emit an event on the UI thread that informs user
                // that the session is reconnecting and why.
                //
                using (LockRead())
                    e.ContinueDelegate(!_cancelled);

                _session._state.SetReconnectAttempt(e.AttemptCount);
            }

            private void OnClientAutoReconnectComplete(object sender, ClientAutoReconnectCompleteArgs e)
            {
                _session.InternalSetState(new ConnectedSession(_connection, this));
            }

            private void OnClientDisconnected(object sender, ClientDisconnectedArgs e)
            {
                //
                // TODO: set the session state to "disconnected"
                //
            }
        }
    }
}
