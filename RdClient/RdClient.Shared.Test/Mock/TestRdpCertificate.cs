namespace RdClient.Shared.Test.Mock
{
    using RdClient.Shared.CxWrappers;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
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
        public TestRdpCertificate()
        {
            Random rnd = new Random();

            _issuer = string.Format("Issuer #{0}", rnd.Next());
            _serialNumber = new byte[128];
            rnd.NextBytes(_serialNumber);
            _friendlyName = string.Format("Test certificate #{0}", rnd.Next());
            _subject = string.Format("On mice #{0}", rnd.Next());
            _hash = new byte[64];
            rnd.NextBytes(_hash);
            _validFrom = DateTimeOffset.UtcNow;
            _validTo = DateTimeOffset.UtcNow;
        }

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

        public TestRdpCertificate(string issuer, byte[] serialNumber)
        {
            Contract.Assert(null != issuer);
            Contract.Assert(null != serialNumber);

            _issuer = issuer;
            _serialNumber = serialNumber;
            _friendlyName = "test certificate";
            _subject = "On mice";
            _hash = new byte[64];
            (new Random()).NextBytes(_hash);
            _validFrom = DateTimeOffset.UtcNow;
            _validTo = DateTimeOffset.UtcNow;
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
