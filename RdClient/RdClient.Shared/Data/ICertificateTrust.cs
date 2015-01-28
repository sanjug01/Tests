namespace RdClient.Shared.Data
{
    using RdClient.Shared.CxWrappers;

    public interface ICertificateTrust
    {
        /// <summary>
        /// Establish trust to the specified certificate.
        /// </summary>
        /// <param name="certificate">Certificate for that trust must be established.</param>
        void TrustCertificate(IRdpCertificate certificate);

        /// <summary>
        /// Remove all established certificate trust.
        /// </summary>
        void RemoveAllTrust();

        bool IsCertificateTrusted(IRdpCertificate certificate);
    }
}
