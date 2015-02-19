namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.ViewModels;
    using System;
    using System.Diagnostics.Contracts;

    public sealed class RemoteSession : MutableObject, IRemoteSession
    {
        private readonly RemoteSessionSetup _sessionSetup;
        private readonly IDeferredExecution _deferredExecution;
        private readonly IRdpConnectionSource _connectionSource;
        private readonly ICertificateTrust _certificateTrust;

        private EventHandler<CredentialsNeededEventArgs> _credentialsNeeded;

        private IRemoteSessionView _sessionView;
        private IRenderingPanel _renderingPanel;
        private IRdpConnection _connection;

        public RemoteSession(RemoteSessionSetup sessionSetup, IDeferredExecution deferredExecution, IRdpConnectionSource connectionSource)
        {
            Contract.Requires(null != sessionSetup);
            Contract.Requires(null != deferredExecution);
            Contract.Requires(null != connectionSource);
            Contract.Ensures(null != _sessionSetup);
            Contract.Ensures(null != _deferredExecution);
            Contract.Ensures(null != _connectionSource);
            Contract.Ensures(null != _certificateTrust);

            _sessionSetup = sessionSetup;
            _deferredExecution = deferredExecution;
            _connectionSource = connectionSource;
            _certificateTrust = new CertificateTrust();
        }

        ICertificateTrust IRemoteSession.CertificateTrust
        {
            get { return _certificateTrust; }
        }

        event EventHandler<CredentialsNeededEventArgs> IRemoteSession.CredentialsNeeded
        {
            add
            {
                using(LockWrite())
                    _credentialsNeeded += value;
            }

            remove
            {
                using (LockWrite())
                    _credentialsNeeded -= value;
            }
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

            if (null == _connection)
            {
                //
                // Ask the connection source to create a new session.
                // The connection source comes all the way from XAML of the main page.
                //
                _connection = _connectionSource.CreateConnection(_renderingPanel);
                //
                // TODO: activate the connection.
                //
            }
            else
            {
                //
                // TODO: re-activate the connection.
                //
            }

            return new RemoteSessionControl(_connection);
        }

        void IRemoteSession.Suspend()
        {
            Contract.Assert(null != _renderingPanel);
            Contract.Assert(null != _sessionView);
            //
            // TODO: Find out whether suspension is an asynchronous process
            //       and we must wait for some events from the connection.
            //
            _connection.Suspend();
            //
            // Recycle the rendering panel;
            //
            _sessionView.RecycleRenderingPanel(_renderingPanel);
            _renderingPanel = null;
            _sessionView = null;
        }

        void IRemoteSession.Disconnect()
        {
            //
            // TODO: Wait for the connection to disconnect and recycle the rendering panel after that.
            //
            _connection.Disconnect();
            //
            // Recycle the rendering panel;
            //
            _sessionView.RecycleRenderingPanel(_renderingPanel);
            _renderingPanel = null;
            _sessionView = null;
        }

        private IRdpConnection CreateConnection(IRenderingPanel renderingPanel)
        {
            Contract.Requires(null != renderingPanel);
            //
            // TODO: because of the limitations of the CX component, the connection factory
            //       must be a singleton; the rendering panel also must be a singleton, along
            //       with the session view/session view model.
            //       The singleton session view provides the singleton swap chain panel (rendering panel).
            //       The singleton is established in XAML of the session view. The singleton then is used
            //       (through different interfaces) to produce the rendering panel passed to this method,
            //       and to create an RDP session.
            //
            throw new NotImplementedException();
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
            using (LockUpgradeableRead())
            {
                if (null != _credentialsNeeded)
                    _credentialsNeeded(this, new CredentialsNeededEventArgs(task));
            }
        }
    }
}
