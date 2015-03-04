using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.CxWrappers.Utils;
using RdClient.Shared.Models;
using System;
using System.Diagnostics.Contracts;
using Windows.Security.Cryptography.Certificates;

namespace RdClient.Shared.CxWrappers
{


    /// <summary>
    /// wrapper class for both the server certificate with certificate error property
    /// </summary>
    public class RdpCertificate : IRdpCertificate
    {
        private Certificate _serverCertificate;

        public IRdpCertificateError Error {get; private set; }
        public RdpCertificate(Certificate certificate, RdClientCx.ServerCertificateError serverCertificateError)
        {
            Contract.Requires(certificate != null);

            _serverCertificate = certificate;
            Error = new RdpCertificateError(serverCertificateError);
        }

        public string FriendlyName 
        { 
            get {return _serverCertificate.FriendlyName; }
        }
        public bool HasPrivateKey 
        { 
            get {return _serverCertificate.HasPrivateKey; }
        }
        public bool IsStronglyProtected 
        { 
            get {return _serverCertificate.IsStronglyProtected; }
        }
        public string Issuer 
        { 
            get {return _serverCertificate.Issuer; }
        }
        public byte[] SerialNumber 
        { 
            get {return _serverCertificate.SerialNumber; }
        }
        public string Subject 
        { 
            get {return _serverCertificate.Subject; } 
        }
        public DateTimeOffset ValidFrom 
        { 
            get {return _serverCertificate.ValidFrom; }
        }
        public DateTimeOffset ValidTo 
        {
            get { return _serverCertificate.ValidTo; }
        }

        public byte[] GetHashValue()
        { 
            return _serverCertificate.GetHashValue();  
        }
        public byte[] GetHashValue(string hashAlgorithmName)
        { 
            return _serverCertificate.GetHashValue(hashAlgorithmName);  
        }
    }
}
