namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Common root of "resource" objects - remote desktops and remote applications.
    /// </summary>
    public abstract class RemoteResource : ModelBase
    {
        private readonly Workspace _parentWorkspace;

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
        protected RemoteResource(Workspace parentWorkspace)
        {
            Contract.Requires(null != parentWorkspace);
            Contract.Ensures(null != _parentWorkspace);
            _parentWorkspace = parentWorkspace;
        }

        /// <summary>
        /// Make the workspace available to child classes.
        /// </summary>
        protected Workspace ParentWorkspace
        {
            get { return _parentWorkspace; }
        }
    }
}
