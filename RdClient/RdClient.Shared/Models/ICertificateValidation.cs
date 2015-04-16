namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;

    public interface ICertificateValidation : IValidation
    {
        IRdpCertificate Certificate { get; }
    }
}
