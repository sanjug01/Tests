namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Base class for all specific workspace classes - local workspace, on-premise workspace and Azure workspace.
    /// </summary>
    public abstract class Workspace : ModelBase
    {
        private readonly RdDataModel _persistentData;
        private readonly ModelCollection<RemoteConnection> _connections;

        protected Workspace(RdDataModel persistentData)
        {
            Contract.Requires(null != persistentData);
            Contract.Ensures(null != _persistentData);
            Contract.Ensures(null != _connections);

            _persistentData = persistentData;
            _connections = new ModelCollection<RemoteConnection>();
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
