namespace RdClient.Shared.Test.Mock
{
    using RdClient.Shared.CxWrappers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    sealed class TestRdpCertificate : IRdpCertificate
    {
        private readonly string
            _friendlyName,
            _issuer,
            _subject;
        private readonly byte[]
            _serialNumber,
            _hash;
        private readonly DateTimeOffset
            _validFrom,
            _validTo;

        public TestRdpCertificate(byte[] serialNumber, byte[] hash, DateTimeOffset validFrom, DateTimeOffset valitTo)
        {
            _friendlyName = "test certificate";
            _issuer = "Horns and Hooves Inc.";
            _subject = "On mice";
            _serialNumber = serialNumber;
            _hash = hash;
            _validFrom = validFrom;
            _validTo = valitTo;
        }

        string IRdpCertificate.FriendlyName { get { return _friendlyName; } }
        bool IRdpCertificate.HasPrivateKey { get { return false; } }
        bool IRdpCertificate.IsStronglyProtected { get { return false; } }
        string IRdpCertificate.Issuer { get { return _issuer; } }
        byte[] IRdpCertificate.SerialNumber { get { return _serialNumber; } }
        string IRdpCertificate.Subject { get { return _subject; } }
        DateTimeOffset IRdpCertificate.ValidFrom { get { return _validFrom; } }
        DateTimeOffset IRdpCertificate.ValidTo { get { return _validTo; } }

        byte[] IRdpCertificate.GetHashValue()
        {
            return _hash;
        }

        byte[] IRdpCertificate.GetHashValue(string hashAlgorithmName)
        {
            return _hash;
        }

        IRdpCertificateError IRdpCertificate.Error { get { return null; } }
    }
}
