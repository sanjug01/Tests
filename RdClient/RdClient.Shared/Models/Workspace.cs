﻿namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;

    /// <summary>
    /// Base class for all specific workspace classes - local workspace, on-premise workspace and Azure workspace.
    /// </summary>
    [DataContract(IsReference=true)]
    public abstract class Workspace : ModelBase
    {
        private RdDataModel _dataModel;
        private readonly ModelCollection<RemoteConnection> _connections;

        protected Workspace(RdDataModel dataModel)
        {
            Contract.Requires(null != dataModel);
            Contract.Ensures(null != _dataModel);
            Contract.Ensures(null != _connections);

            _dataModel = dataModel;
            _connections = new ModelCollection<RemoteConnection>();
        }

        protected Workspace()
        {
            Contract.Ensures(null != _connections);

            _connections = new ModelCollection<RemoteConnection>();
        }

        public void AttachToDataModel(RdDataModel dataModel)
        {
            Contract.Assert(null == _dataModel);
            Contract.Requires(null != dataModel);
            Contract.Ensures(null != _dataModel);
            _dataModel = dataModel;
        }

        public ModelCollection<RemoteConnection> Connections
        {
            get { return _connections; }
        }

        public virtual void TrustCertificate(IRdpCertificate certificate)
        {
            //
            // Delegate adding the certificate to the white list to the root data model object.
            //
            _dataModel.TrustCertificate(certificate);
        }

        public virtual bool IsCertificateTrusted(IRdpCertificate certificate)
        {
            //
            // Delegate the check to the root data model object.
            //
            return _dataModel.IsCertificateTrusted(certificate);
        }
    }
}
