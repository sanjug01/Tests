﻿namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Base class for all specific workspace classes - local workspace, on-premise workspace and Azure workspace.
    /// </summary>
    public abstract class Workspace : ModelBase
    {
        private readonly PersistentData _persistentData;
        private readonly ModelCollection<RemoteResource> _connections;

        protected Workspace(PersistentData persistentData)
        {
            Contract.Requires(null != persistentData);
            Contract.Ensures(null != _persistentData);
            Contract.Ensures(null != _connections);

            _persistentData = persistentData;
            _connections = new ModelCollection<RemoteResource>();
        }

        public ModelCollection<RemoteResource> Connections
        {
            get { return _connections; }
        }

        public virtual void TrustCertificate(IRdpCertificate certificate)
        {
            //
            // Delegate adding the certificate to the white list to the root data model object.
            //
            _persistentData.TrustCertificate(certificate);
        }

        public virtual bool IsCertificateTrusted(IRdpCertificate certificate)
        {
            //
            // Delegate the check to the root data model object.
            //
            return _persistentData.IsCertificateTrusted(certificate);
        }
    }
}
