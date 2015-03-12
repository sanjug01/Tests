namespace RdClient.Shared.Helpers
{
    using RdClient.Shared.CxWrappers;
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;

    /// <summary>
    /// Implementation of IRdpEvents that re-emits events emitted by another instance of IRdpEvents
    /// and protects all access to the event invocation lists with a reader-writer monitor.
    /// </summary>
    /// <remarks>The proxy object is needed because the RdClientCX component emits its events asynchronously
    /// on worker threads, and it does not offer any thread synchronization for subscribing and consuming
    /// its events; therefore, a race is possible when one object subscribes for an event with RdWinCX but the event
    /// is emitted at the moment of subscribing and it may be lost.</remarks>
    public sealed class RdpEventsSyncProxy : IRdpEvents
    {
        private readonly ReaderWriterLockSlim _monitor;
        private readonly IRdpEvents _source;

        private EventHandler<ClientConnectedArgs> _clientConnected;
        private EventHandler<ClientAsyncDisconnectArgs> _clientAsyncDisconnect;
        private EventHandler<ClientDisconnectedArgs> _clientDisconnected;
        private EventHandler<UserCredentialsRequestArgs> _userCredentialsRequest;
        private EventHandler<MouseCursorShapeChangedArgs> _mouseCursorShapeChanged;
        private EventHandler<MouseCursorPositionChangedArgs> _mouseCursorPositionChanged;
        private EventHandler<MultiTouchEnabledChangedArgs> _multiTouchEnabledChanged;
        private EventHandler<ConnectionHealthStateChangedArgs> _connectionHealthStateChanged;
        private EventHandler<ClientAutoReconnectingArgs> _clientAutoReconnecting;
        private EventHandler<ClientAutoReconnectCompleteArgs> _clientAutoReconnectComplete;
        private EventHandler<LoginCompletedArgs> _loginCompleted;
        private EventHandler<StatusInfoReceivedArgs> _statusInfoReceived;
        private EventHandler<FirstGraphicsUpdateArgs> _firstGraphicsUpdate;
        private EventHandler<RemoteAppWindowCreatedArgs> _remoteAppWindowCreated;
        private EventHandler<RemoteAppWindowDeletedArgs> _remoteAppWindowDeleted;
        private EventHandler<RemoteAppWindowTitleUpdatedArgs> _remoteAppWindowTitleUpdated;
        private EventHandler<RemoteAppWindowIconUpdatedArgs> _remoteAppWindowIconUpdated;

        public static IRdpEvents Create(IRdpEvents source, ReaderWriterLockSlim monitor)
        {
            Contract.Requires(null != monitor);
            Contract.Requires(null != source);

            return new RdpEventsSyncProxy(source, monitor);
        }

        private RdpEventsSyncProxy(IRdpEvents source, ReaderWriterLockSlim monitor)
        {
            Contract.Requires(null != monitor);
            Contract.Requires(null != source);
            Contract.Ensures(null != _monitor);
            Contract.Ensures(null != _source);

            _monitor = monitor;
            _source = source;
        }

        event EventHandler<ClientConnectedArgs> IRdpEvents.ClientConnected
        {
            add
            {
                using(ReadWriteMonitor.Write(_monitor))
                {
                    if (null == _clientConnected)
                        _source.ClientConnected += this.EmitClientConnected;
                    _clientConnected += value;
                }
            }

            remove
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    _clientConnected -= value;
                    if (null == _clientConnected)
                        _source.ClientConnected -= this.EmitClientConnected;
                }
            }
        }

        event EventHandler<ClientAsyncDisconnectArgs> IRdpEvents.ClientAsyncDisconnect
        {
            add
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    if (null == _clientAsyncDisconnect)
                        _source.ClientAsyncDisconnect += this.EmitClientAsyncDisconnect;
                    _clientAsyncDisconnect += value;
                }
            }

            remove
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    _clientAsyncDisconnect -= value;
                    if (null == _clientAsyncDisconnect)
                        _source.ClientAsyncDisconnect -= this.EmitClientAsyncDisconnect;
                }
            }
        }

        event EventHandler<ClientDisconnectedArgs> IRdpEvents.ClientDisconnected
        {
            add
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    if (null == _clientDisconnected)
                        _source.ClientDisconnected += this.EmitClientDisconnected;
                    _clientDisconnected += value;
                }
            }

            remove
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    _clientDisconnected -= value;
                    if (null == _clientDisconnected)
                        _source.ClientDisconnected -= this.EmitClientDisconnected;
                }
            }
        }

        event EventHandler<UserCredentialsRequestArgs> IRdpEvents.UserCredentialsRequest
        {
            add
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    if (null == _userCredentialsRequest)
                        _source.UserCredentialsRequest += this.EmitUserCredentialsRequest;
                    _userCredentialsRequest += value;
                }
            }

            remove
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    _userCredentialsRequest -= value;
                    if (null == _userCredentialsRequest)
                        _source.UserCredentialsRequest -= this.EmitUserCredentialsRequest;
                }
            }
        }

        event EventHandler<MouseCursorShapeChangedArgs> IRdpEvents.MouseCursorShapeChanged
        {
            add
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    if (null == _mouseCursorShapeChanged)
                        _source.MouseCursorShapeChanged += this.EmitMouseCursorShapeChanged;
                    _mouseCursorShapeChanged += value;
                }
            }

            remove
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    _mouseCursorShapeChanged -= value;
                    if (null == _mouseCursorShapeChanged)
                        _source.MouseCursorShapeChanged -= this.EmitMouseCursorShapeChanged;
                }
            }
        }

        event EventHandler<MouseCursorPositionChangedArgs> IRdpEvents.MouseCursorPositionChanged
        {
            add
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    if (null == _mouseCursorPositionChanged)
                        _source.MouseCursorPositionChanged += this.EmitMouseCursorPositionChanged;
                    _mouseCursorPositionChanged += value;
                }
            }

            remove
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    _mouseCursorPositionChanged -= value;
                    if (null == _mouseCursorPositionChanged)
                        _source.MouseCursorPositionChanged -= this.EmitMouseCursorPositionChanged;
                }
            }
        }

        event EventHandler<MultiTouchEnabledChangedArgs> IRdpEvents.MultiTouchEnabledChanged
        {
            add
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    if (null == _multiTouchEnabledChanged)
                        _source.MultiTouchEnabledChanged += this.EmitMultiTouchEnabledChanged;
                    _multiTouchEnabledChanged += value;
                }
            }

            remove
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    _multiTouchEnabledChanged -= value;
                    if (null == _multiTouchEnabledChanged)
                        _source.MultiTouchEnabledChanged -= this.EmitMultiTouchEnabledChanged;
                }
            }
        }

        event EventHandler<ConnectionHealthStateChangedArgs> IRdpEvents.ConnectionHealthStateChanged
        {
            add
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    if (null == _connectionHealthStateChanged)
                        _source.ConnectionHealthStateChanged += this.EmitConnectionHealthStateChanged;
                    _connectionHealthStateChanged += value;
                }
            }

            remove
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    _connectionHealthStateChanged -= value;
                    if (null == _connectionHealthStateChanged)
                        _source.ConnectionHealthStateChanged -= this.EmitConnectionHealthStateChanged;
                }
            }
        }

        event EventHandler<ClientAutoReconnectingArgs> IRdpEvents.ClientAutoReconnecting
        {
            add
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    if (null == _clientAutoReconnecting)
                        _source.ClientAutoReconnecting += this.EmitClientAutoReconnecting;
                    _clientAutoReconnecting += value;
                }
            }

            remove
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    _clientAutoReconnecting -= value;
                    if (null == _clientAutoReconnecting)
                        _source.ClientAutoReconnecting -= this.EmitClientAutoReconnecting;
                }
            }
        }

        event EventHandler<ClientAutoReconnectCompleteArgs> IRdpEvents.ClientAutoReconnectComplete
        {
            add
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    if (null == _clientAutoReconnectComplete)
                        _source.ClientAutoReconnectComplete += this.EmitClientAutoReconnectComplete;
                    _clientAutoReconnectComplete += value;
                }
            }

            remove
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    _clientAutoReconnectComplete -= value;
                    if (null == _clientAutoReconnectComplete)
                        _source.ClientAutoReconnectComplete -= this.EmitClientAutoReconnectComplete;
                }
            }
        }

        event EventHandler<LoginCompletedArgs> IRdpEvents.LoginCompleted
        {
            add
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    if (null == _loginCompleted)
                        _source.LoginCompleted += this.EmitLoginCompleted;
                    _loginCompleted += value;
                }
            }

            remove
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    _loginCompleted -= value;
                    if (null == _loginCompleted)
                        _source.LoginCompleted -= this.EmitLoginCompleted;
                }
            }
        }

        event EventHandler<StatusInfoReceivedArgs> IRdpEvents.StatusInfoReceived
        {
            add
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    if (null == _statusInfoReceived)
                        _source.StatusInfoReceived += this.EmitStatusInfoReceived;
                    _statusInfoReceived += value;
                }
            }

            remove
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    _statusInfoReceived -= value;
                    if (null == _statusInfoReceived)
                        _source.StatusInfoReceived -= this.EmitStatusInfoReceived;
                }
            }
        }

        event EventHandler<FirstGraphicsUpdateArgs> IRdpEvents.FirstGraphicsUpdate
        {
            add
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    if (null == _firstGraphicsUpdate)
                        _source.FirstGraphicsUpdate += this.EmitFirstGraphicsUpdate;
                    _firstGraphicsUpdate += value;
                }
            }

            remove
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    _firstGraphicsUpdate -= value;
                    if (null == _firstGraphicsUpdate)
                        _source.FirstGraphicsUpdate -= this.EmitFirstGraphicsUpdate;
                }
            }
        }

        event EventHandler<RemoteAppWindowCreatedArgs> IRdpEvents.RemoteAppWindowCreated
        {
            add
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    if (null == _remoteAppWindowCreated)
                        _source.RemoteAppWindowCreated += this.EmitRemoteAppWindowCreated;
                    _remoteAppWindowCreated += value;
                }
            }

            remove
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    _remoteAppWindowCreated -= value;
                    if (null == _remoteAppWindowCreated)
                        _source.RemoteAppWindowCreated -= this.EmitRemoteAppWindowCreated;
                }
            }
        }

        event EventHandler<RemoteAppWindowDeletedArgs> IRdpEvents.RemoteAppWindowDeleted
        {
            add
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    if (null == _remoteAppWindowDeleted)
                        _source.RemoteAppWindowDeleted += this.EmitRemoteAppWindowDeleted;
                    _remoteAppWindowDeleted += value;
                }
            }

            remove
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    _remoteAppWindowDeleted -= value;
                    if (null == _remoteAppWindowDeleted)
                        _source.RemoteAppWindowDeleted -= this.EmitRemoteAppWindowDeleted;
                }
            }
        }

        event EventHandler<RemoteAppWindowTitleUpdatedArgs> IRdpEvents.RemoteAppWindowTitleUpdated
        {
            add
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    if (null == _remoteAppWindowTitleUpdated)
                        _source.RemoteAppWindowTitleUpdated += this.EmitRemoteAppWindowTitleUpdated;
                    _remoteAppWindowTitleUpdated += value;
                }
            }

            remove
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    _remoteAppWindowTitleUpdated -= value;
                    if (null == _remoteAppWindowTitleUpdated)
                        _source.RemoteAppWindowTitleUpdated -= this.EmitRemoteAppWindowTitleUpdated;
                }
            }
        }

        event EventHandler<RemoteAppWindowIconUpdatedArgs> IRdpEvents.RemoteAppWindowIconUpdated
        {
            add
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    if (null == _remoteAppWindowIconUpdated)
                        _source.RemoteAppWindowIconUpdated += this.EmitRemoteAppWindowIconUpdated;
                    _remoteAppWindowIconUpdated += value;
                }
            }

            remove
            {
                using (ReadWriteMonitor.Write(_monitor))
                {
                    _remoteAppWindowIconUpdated -= value;
                    if (null == _remoteAppWindowIconUpdated)
                        _source.RemoteAppWindowIconUpdated -= this.EmitRemoteAppWindowIconUpdated;
                }
            }
        }

        private void EmitClientConnected(object sender, ClientConnectedArgs e)
        {
            using(ReadWriteMonitor.UpgradeableRead(_monitor))
            {
                if (null != _clientConnected)
                    _clientConnected(sender, e);
            }
        }

        private void EmitClientAsyncDisconnect(object sender, ClientAsyncDisconnectArgs e)
        {
            using (ReadWriteMonitor.UpgradeableRead(_monitor))
            {
                if (null != _clientAsyncDisconnect)
                    _clientAsyncDisconnect(sender, e);
            }
        }

        private void EmitClientDisconnected(object sender, ClientDisconnectedArgs e)
        {
            using (ReadWriteMonitor.UpgradeableRead(_monitor))
            {
                if (null != _clientDisconnected)
                    _clientDisconnected(sender, e);
            }
        }

        private void EmitUserCredentialsRequest(object sender, UserCredentialsRequestArgs e)
        {
            using (ReadWriteMonitor.UpgradeableRead(_monitor))
            {
                if (null != _userCredentialsRequest)
                    _userCredentialsRequest(sender, e);
            }
        }

        private void EmitMouseCursorShapeChanged(object sender, MouseCursorShapeChangedArgs e)
        {
            using (ReadWriteMonitor.UpgradeableRead(_monitor))
            {
                if (null != _mouseCursorShapeChanged)
                    _mouseCursorShapeChanged(sender, e);
            }
        }

        private void EmitMouseCursorPositionChanged(object sender, MouseCursorPositionChangedArgs e)
        {
            using (ReadWriteMonitor.UpgradeableRead(_monitor))
            {
                if (null != _mouseCursorPositionChanged)
                    _mouseCursorPositionChanged(sender, e);
            }
        }

        private void EmitMultiTouchEnabledChanged(object sender, MultiTouchEnabledChangedArgs e)
        {
            using (ReadWriteMonitor.UpgradeableRead(_monitor))
            {
                if (null != _multiTouchEnabledChanged)
                    _multiTouchEnabledChanged(sender, e);
            }
        }

        private void EmitConnectionHealthStateChanged(object sender, ConnectionHealthStateChangedArgs e)
        {
            using (ReadWriteMonitor.UpgradeableRead(_monitor))
            {
                if (null != _connectionHealthStateChanged)
                    _connectionHealthStateChanged(sender, e);
            }
        }

        private void EmitClientAutoReconnecting(object sender, ClientAutoReconnectingArgs e)
        {
            using (ReadWriteMonitor.UpgradeableRead(_monitor))
            {
                if (null != _clientAutoReconnecting)
                    _clientAutoReconnecting(sender, e);
            }
        }

        private void EmitClientAutoReconnectComplete(object sender, ClientAutoReconnectCompleteArgs e)
        {
            using (ReadWriteMonitor.UpgradeableRead(_monitor))
            {
                if (null != _clientAutoReconnectComplete)
                    _clientAutoReconnectComplete(sender, e);
            }
        }

        private void EmitLoginCompleted(object sender, LoginCompletedArgs e)
        {
            using (ReadWriteMonitor.UpgradeableRead(_monitor))
            {
                if (null != _loginCompleted)
                    _loginCompleted(sender, e);
            }
        }

        private void EmitStatusInfoReceived(object sender, StatusInfoReceivedArgs e)
        {
            using (ReadWriteMonitor.UpgradeableRead(_monitor))
            {
                if (null != _statusInfoReceived)
                    _statusInfoReceived(sender, e);
            }
        }

        private void EmitFirstGraphicsUpdate(object sender, FirstGraphicsUpdateArgs e)
        {
            using (ReadWriteMonitor.UpgradeableRead(_monitor))
            {
                if (null != _firstGraphicsUpdate)
                    _firstGraphicsUpdate(sender, e);
            }
        }

        private void EmitRemoteAppWindowCreated(object sender, RemoteAppWindowCreatedArgs e)
        {
            using (ReadWriteMonitor.UpgradeableRead(_monitor))
            {
                if (null != _remoteAppWindowCreated)
                    _remoteAppWindowCreated(sender, e);
            }
        }

        private void EmitRemoteAppWindowDeleted(object sender, RemoteAppWindowDeletedArgs e)
        {
            using (ReadWriteMonitor.UpgradeableRead(_monitor))
            {
                if (null != _remoteAppWindowDeleted)
                    _remoteAppWindowDeleted(sender, e);
            }
        }

        private void EmitRemoteAppWindowTitleUpdated(object sender, RemoteAppWindowTitleUpdatedArgs e)
        {
            if (null != _remoteAppWindowTitleUpdated)
                _remoteAppWindowTitleUpdated(sender, e);
        }

        private void EmitRemoteAppWindowIconUpdated(object sender, RemoteAppWindowIconUpdatedArgs e)
        {
            if (null != _remoteAppWindowIconUpdated)
                _remoteAppWindowIconUpdated(sender, e);
        }
    }
}
