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
        public string Subject { get; }

        DateTimeOffset ValidFrom { get; }
        DateTimeOffset ValidTo { get; }

        byte[] GetHashValue();
        byte[] GetHashValue(string hashAlgorithmName);

        IRdpCertificate Error { get; }
        // Windows.Storage.Streams.IBuffer GetCertificateBlob();
    }
}
