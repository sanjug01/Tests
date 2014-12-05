using RdClient.Shared.CxWrappers;
using RdMock;
using System;


namespace RdClient.Shared.Test.Mock
{
    public class RdpCertificateError : MockBase, IRdpCertificateError
    {
        public RdpCertificateError(int errorCode = 0, CertificateErrors errorFlags = CertificateErrors.UntrustedRoot, ServerCertificateErrorSource errorSource = ServerCertificateErrorSource.None) 
        {
            ErrorCode = errorCode;
            ErrorFlags = errorFlags;
            ErrorSource = errorSource;
        }

        public int ErrorCode { get; set; }
        public CertificateErrors ErrorFlags { get; set; }
        public ServerCertificateErrorSource ErrorSource { get; set; }
    }

    public class RdpCertificate : MockBase, IRdpCertificate
    {      
        public RdpCertificate() { }

        public IRdpCertificateError Error { get; set; }
        public string FriendlyName { get; set; }
        public bool HasPrivateKey { get; set; }
        public bool IsStronglyProtected { get; set; }
        public string Issuer { get; set; }
        public byte[] SerialNumber { get; set; }
        public string Subject { get; set; }
        public DateTimeOffset ValidFrom { get; set; }
        public DateTimeOffset ValidTo { get; set; }

        public byte[] Bytes { private get; set; }
        public byte[] GetHashValue()
        {
            Invoke(new object[] { });
            return this.Bytes;
        }
        public byte[] GetHashValue(string hashAlgorithmName)
        {
            Invoke(new object[] { hashAlgorithmName });
            return this.Bytes;
        }
    }
}
