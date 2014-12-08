using RdClient.Shared.CxWrappers;
using RdMock;
using System;


namespace RdClient.Shared.Test.Mock
{
    public class RdpCertificateError : MockBase, IRdpCertificateError
    {
        /// <summary>
        ///     Constructs a RdpCertificateError mock object and assigns initial value to its properties.
        ///     The properties get randomly chosen default values, if not given
        /// </summary>
        /// <param name="errorCode">initial errorCode value</param>
        /// <param name="errorFlags">initial errorFlags value</param>
        /// <param name="errorSource">initial errorSource value</param>
        public RdpCertificateError(int errorCode = 1, CertificateErrors errorFlags = CertificateErrors.UntrustedRoot, ServerCertificateErrorSource errorSource = ServerCertificateErrorSource.None) 
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

        public byte[] GetHashValue()
        {
            return (byte[]) Invoke(new object[] { });
        }
        public byte[] GetHashValue(string hashAlgorithmName)
        {
            return (byte[])Invoke(new object[] { hashAlgorithmName });
        }
    }
}
