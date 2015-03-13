namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.ViewModels;
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;

    public sealed partial class RemoteSession : MutableObject, IRemoteSession
    {
        private readonly RemoteSessionSetup _sessionSetup;
        private readonly RemoteSessionState _state;
        private readonly IDeferredExecution _deferredExecution;
        private readonly IRdpConnectionSource _connectionSource;
        private readonly ICertificateTrust _certificateTrust;
        private readonly ReaderWriterLockSlim _sessionMonitor;
        private readonly ITimerFactory _timerFactory;

        private EventHandler<CredentialsNeededEventArgs> _credentialsNeeded;
        private EventHandler<BadCertificateEventArgs> _badCertificate;
        private EventHandler<SessionFailureEventArgs> _failed;
        private EventHandler<SessionInterruptedEventArgs> _interrupted;
        private EventHandler _closed;

        private IRemoteSessionView _sessionView;
        private IRenderingPanel _renderingPanel;
        private IRdpConnection _connection;
        private IRdpEvents _syncEvents;
        //
        // Internal state of the session directly reported by the RDP connection component.
        // Access to this state must be protected with a lock.
        //
        private InternalState _internalState;

        private abstract class InternalState : DisposableObject
        {
            private readonly SessionState _sessionState;
            private readonly ReaderWriterLockSlim _monitor;

            public SessionState State { get { return _sessionState; } }

            /// <summary>
            /// Activate the session state - emit all events to synchronize the UI with the session.
            /// </summary>
            /// <param name="session">Session for that the state is restoring</param>
            public abstract void Activate(RemoteSession session);

            public virtual void Terminate(RemoteSession session) { }

            public virtual void Deactivate(RemoteSession session) { }

            public virtual void Complete(RemoteSession session) { }

            protected InternalState(SessionState sessionState, ReaderWriterLockSlim monitor)
            {
                _sessionState = sessionState;
                _monitor = monitor;
            }

            protected InternalState(SessionState sessionState, InternalState state)
            {
                _sessionState = sessionState;
                _monitor = state._monitor;
            }

            public IDisposable LockRead()
            {
                return ReadWriteMonitor.Read(_monitor);
            }

            public IDisposable LockUpgradeableRead()
            {
                return ReadWriteMonitor.UpgradeableRead(_monitor);
            }

            public IDisposable LockWrite()
            {
                return ReadWriteMonitor.Write(_monitor);
            }
        }

        public RemoteSession(RemoteSessionSetup sessionSetup, IDeferredExecution deferredExecution, IRdpConnectionSource connectionSource, ITimerFactory timerFactory)
        {
            Contract.Requires(null != sessionSetup);
            Contract.Requires(null != deferredExecution);
            Contract.Requires(null != connectionSource);
            Contract.Requires(null != timerFactory);
            Contract.Ensures(null != _sessionSetup);
            Contract.Ensures(null != _deferredExecution);
            Contract.Ensures(null != _connectionSource);
            Contract.Ensures(null != _certificateTrust);
            Contract.Ensures(null != _state);
            Contract.Ensures(null != timerFactory);

            _sessionSetup = sessionSetup;
            _deferredExecution = deferredExecution;
            _connectionSource = connectionSource;
            _state = new RemoteSessionState(deferredExecution);
            _certificateTrust = new CertificateTrust();
            _sessionMonitor = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            _timerFactory = timerFactory;
            //
            // _internalState must never be null, so the initial state is assigned to a state object
            // that does not do anything.
            //
            _internalState = new InactiveSession(_sessionMonitor);
        }

        protected override void DisposeManagedState()
        {
            _sessionMonitor.Dispose();
        }

        string IRemoteSession.HostName
        {
            get { return _sessionSetup.HostName; }
        }

        IRemoteSessionState IRemoteSession.State
        {
            get
            {
                Contract.Ensures(null != Contract.Result<IRemoteSessionState>());
                return _state;
            }
        }

        ICertificateTrust IRemoteSession.CertificateTrust
        {
            get
            {
                Contract.Ensures(null != Contract.Result<ICertificateTrust>());
                return _certificateTrust;
            }
        }

        event EventHandler<CredentialsNeededEventArgs> IRemoteSession.CredentialsNeeded
        {
            add { using(LockWrite()) _credentialsNeeded += value; }
            remove { using (LockWrite()) _credentialsNeeded -= value; }
        }

        event EventHandler<BadCertificateEventArgs> IRemoteSession.BadCertificate
        {
            add { using (LockWrite()) _badCertificate += value; }
            remove { using (LockWrite()) _badCertificate -= value; }
        }

        event EventHandler<SessionFailureEventArgs> IRemoteSession.Failed
        {
            add { using (LockWrite()) _failed += value; }
            remove { using (LockWrite()) _failed -= value; }
        }

        event EventHandler<SessionInterruptedEventArgs> IRemoteSession.Interrupted
        {
            add { using (LockWrite()) _interrupted += value; }
            remove { using (LockWrite()) _interrupted -= value; }
        }

        event EventHandler IRemoteSession.Closed
        {
            add { using (LockWrite()) _closed += value; }
            remove { using (LockWrite()) _closed -= value; }
        }

        IRemoteSessionControl IRemoteSession.Activate(IRemoteSessionView sessionView)
        {
            Contract.Assert(null == _sessionView);
            Contract.Requires(null != sessionView);
            Contract.Ensures(null != _sessionView);

            //
            // Obtain a rendering panel from the session view and set up an RDP connection using the panel.
            //
            _sessionView = sessionView;
            _renderingPanel = _sessionView.ActivateNewRenderingPanel();

            using (ReadWriteMonitor.UpgradeableRead(_sessionMonitor))
            {
                if (null == _connection)
                {
                    InternalSetState(new NewSession(_sessionSetup, _sessionMonitor));
                }
                else
                {
                    //
                    // Re-activate the connection.
                    // When the session's internal state changes, the session creates an object that encapsulates the state;
                    // the object stores all information needed to report the state to event subscribers (view models/UI) upon re-activation.
                    //
                }
            }

            return new RemoteSessionControl(_connection);
        }

        IRenderingPanel IRemoteSession.Deactivate()
        {
            Contract.Assert(null != _renderingPanel);

            IRenderingPanel renderingPanel = _renderingPanel;
            _renderingPanel = null;

            _internalState.Deactivate(this);

            return renderingPanel;
        }

        void IRemoteSession.Suspend()
        {
            Contract.Assert(null != _renderingPanel);
            Contract.Assert(null != _sessionView);

            throw new NotImplementedException();
        }

        void IRemoteSession.Disconnect()
        {
            using (LockWrite())
            {
                _internalState.Terminate(this);
            }
        }

        private IRdpConnection InternalCreateConnection(IRenderingPanel renderingPanel)
        {
            Contract.Assert(null == _connection);
            Contract.Assert(null == _syncEvents);
            Contract.Assert(object.ReferenceEquals(_renderingPanel, renderingPanel));

            _connection = _connectionSource.CreateConnection(_sessionSetup.Connection, _renderingPanel);
            _syncEvents = RdpEventsSyncProxy.Create(_connection.Events, _sessionMonitor);

            return _connection;
        }

        private void DeferEmitCredentialsNeeded(IEditCredentialsTask task)
        {
            //
            // Simply defer emitting the event to the dispatcher.
            //
            _deferredExecution.Defer(() => EmitCredentialsNeeded(task));
        }

        private void EmitCredentialsNeeded(IEditCredentialsTask task)
        {
            Contract.Requires(null != task);

            EventHandler<CredentialsNeededEventArgs> credentialsNeeded;

            using (LockUpgradeableRead())
                credentialsNeeded = _credentialsNeeded;

            if (null != credentialsNeeded)
                credentialsNeeded(this, new CredentialsNeededEventArgs(task));
        }

        private void EmitFailed(RdpDisconnectCode disconnectCode)
        {
            EventHandler<SessionFailureEventArgs> failed;

            using (LockUpgradeableRead())
                failed = _failed;

            if (null != failed)
                failed(this, new SessionFailureEventArgs(disconnectCode));
        }

        private void DeferEmitFailed(RdpDisconnectCode disconnectCode)
        {
            _deferredExecution.Defer(() => EmitFailed(disconnectCode));
        }

        private void EmitInterrupted(Action cancelDelegate)
        {
            Contract.Assert(null != cancelDelegate);

            EventHandler<SessionInterruptedEventArgs> interrupted;

            using (LockUpgradeableRead())
                interrupted = _interrupted;

            if (null != interrupted)
                interrupted(this, new SessionInterruptedEventArgs(cancelDelegate));
        }

        private void DeferEmitInterrupted(Action cancelDelegate)
        {
            _deferredExecution.Defer(() => EmitInterrupted(cancelDelegate));
        }

        private void EmitClosed()
        {
            EventHandler closed;

            using(LockUpgradeableRead())
                closed = _closed;

            if (null != closed)
                closed(this, EventArgs.Empty);
        }

        private void DeferEmitClosed()
        {
            _deferredExecution.Defer(() => EmitClosed());
        }

        private void EmitBadCertificate(BadCertificateEventArgs e)
        {
            Contract.Assert(null != e);
            Contract.Assert(!e.ValidationObtained);

            EventHandler<BadCertificateEventArgs> badCertificate;

            using (LockUpgradeableRead())
                badCertificate = _badCertificate;

            if (null != badCertificate)
            {
                badCertificate(this, e);

                if(!e.ValidationObtained)
                {
                    //
                    // Kill the connection
                    //
                    Contract.Assert(null != _connection);
                    _connection.HandleAsyncDisconnectResult(e.DisconnectReason, false);
                }
            }
        }

        private void DeferEmitBadCertificate(BadCertificateEventArgs e)
        {
            _deferredExecution.Defer(() => EmitBadCertificate(e));
        }

        private void InternalSetState(InternalState newState)
        {
            Contract.Assert(null != newState);
            //
            // Lock the session monitor so the RDP connection cannot emit any events while the session
            // is transitioning between the states.
            //
            using (ReadWriteMonitor.Write(_sessionMonitor))
            {
                _internalState.Complete(this);
                _internalState = newState;
                _state.SetState(newState.State);
                _internalState.Activate(this);
            }
        }

        private void InternalStartSession(RemoteSessionSetup sessionSetup)
        {
            //
            // Ask the connection source to create a new session.
            // The connection source comes all the way from XAML of the main page.
            //
            using (ReadWriteMonitor.Write(_sessionMonitor))
            {
                InternalSetState(new ConnectingSession(_renderingPanel, _sessionMonitor));
            }
        }

        private void InternalDeferUpdateSnapshot(byte[] encodedSnapshot)
        {
            _deferredExecution.Defer(() =>
            {
                //
                // Update the data model object for that the session was created.
                //
                _sessionSetup.Connection.EncodedThumbnail = encodedSnapshot;
            });
        }
    }
}
