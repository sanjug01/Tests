namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;

    public interface ICertificateValidation
    {
        IRdpCertificate Certificate { get; }

        void Accept();

        void Reject();
    }
}
