﻿namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Data;
    using System;
    using System.Diagnostics.Contracts;

    public sealed class RemoteSession : IRemoteSession
    {
        private readonly RemoteSessionSetup _sessionSetup;
        private readonly ICertificateTrust _certificateTrust;

        private IRemoteSessionView _sessionView;
        private IRenderingPanel _renderingPanel;
        private IRdpConnection _connection;

        public RemoteSession(RemoteSessionSetup sessionSetup)
        {
            Contract.Requires(null != sessionSetup);
            Contract.Ensures(null != _sessionSetup);
            Contract.Ensures(null != _certificateTrust);

            _sessionSetup = sessionSetup;
            _certificateTrust = new CertificateTrust();
        }

        ICertificateTrust IRemoteSession.CertificateTrust
        {
            get { return _certificateTrust; }
        }

        IRemoteSessionControl IRemoteSession.Activate(IRemoteSessionView sessionView)
        {
            Contract.Assert(null == _sessionView);
            Contract.Requires(null != sessionView);
            Contract.Ensures(null != _sessionView);

            //
            // Obtain a rendering panel from the session view and set up an RDP connection using the panel.
            //
            _renderingPanel = _sessionView.ActivateNewRenderingPanel();
            _sessionView = sessionView;
            _connection = CreateConnection(_renderingPanel);

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
    }
}
