using RdClient.Shared.Models;
using System;

namespace RdClient.Shared.CxWrappers
{
    public enum ServerCertificateErrorSource
    {
        None = 0,
        Ssl = 1,
        CredSSP = 2,
    }

    [Flags]
    public enum CertificateErrors
    {
        Expired = 1,
        NameMismatch = 2,
        UntrustedRoot = 4,
        Revoked = 8,
        RevocationUnknown = 16,
        CertOrChainInvalid = 32,
        MismatchedCert = 64,
        WrongEKU = 128,
        Critical = 134217728,
    }

    public interface IRdpCertificateError
    {
        int ErrorCode { get; }

        CertificateErrors ErrorFlags { get; }

        ServerCertificateErrorSource ErrorSource { get; }

    }
}
