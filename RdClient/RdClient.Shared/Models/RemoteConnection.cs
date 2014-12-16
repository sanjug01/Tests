namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;

    /// <summary>
    /// Common root of "resource" objects - remote desktops and remote applications.
    /// </summary>
    [DataContract(IsReference=true)]
    public abstract class RemoteConnection : ModelBase
    {
        private Workspace _parentWorkspace;

        /// <summary>
        /// Tell the remote resource to always accept a certificate.
        /// </summary>
        /// <param name="certificate">Certificate reported by the RDP layer.</param>
        /// <remarks>Default implementation delegates establishing of trust to the parent workspace.</remarks>
        public virtual void TrustCertificate(IRdpCertificate certificate)
        {
            _parentWorkspace.TrustCertificate(certificate);
        }

        public virtual bool IsCertificateTrusted(IRdpCertificate certificate)
        {
            return _parentWorkspace.IsCertificateTrusted(certificate);
        }

        /// <summary>
        /// A resource is always associated with a workspace.
        /// </summary>
        /// <param name="workspace">Workspace to that the resource belongs.</param>
        protected RemoteConnection(Workspace parentWorkspace)
        {
            Contract.Requires(null != parentWorkspace);
            Contract.Ensures(null != _parentWorkspace);
            _parentWorkspace = parentWorkspace;
        }

        protected RemoteConnection()
        {
        }

        /// <summary>
        /// Reference to the workspace wi that the connection belongs.
        /// </summary>
        public Workspace ParentWorkspace
        {
            get { return _parentWorkspace; }
        }

        /// <summary>
        /// Serialization requires a default constructor that cannot attach the new object to
        /// a parent workspace. In serialization scenarios a workspace may be attached to a loaded object.
        /// </summary>
        /// <param name="parentWorkspace">New parent workspace.</param>
        public void AttachToWorkspace(Workspace parentWorkspace)
        {
            Contract.Requires(null != parentWorkspace);
            Contract.Ensures(null != _parentWorkspace);
            Contract.Assert(null == _parentWorkspace);
            _parentWorkspace = parentWorkspace;
        }
    }
}
