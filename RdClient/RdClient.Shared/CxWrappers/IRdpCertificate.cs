using RdClient.Shared.Models;
using System;

namespace RdClient.Shared.CxWrappers
{
    public interface IRdpCertificate
    {
        string FriendlyName { get; }
        bool HasPrivateKey { get; }
        bool IsStronglyProtected { get; }
        string Issuer { get; }
        byte[] SerialNumber { get; }
        string Subject { get; }

        DateTimeOffset ValidFrom { get; }
        DateTimeOffset ValidTo { get; }

        byte[] GetHashValue();
        byte[] GetHashValue(string hashAlgorithmName);

        IRdpCertificateError Error { get; }
    }
}
