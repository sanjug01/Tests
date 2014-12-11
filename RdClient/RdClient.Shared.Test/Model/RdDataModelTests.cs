namespace RdClient.Shared.Test.Model
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Models;
    using System;

    [TestClass]
    public sealed class RdDataModelTests
    {
        private RdDataModel _model;

        private sealed class TestCertificate : IRdpCertificate
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

            public TestCertificate(byte[] serialNumber, byte[] hash, DateTimeOffset validFrom, DateTimeOffset valitTo)
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

        [TestInitialize]
        public void SetUpTest()
        {
            _model = new RdDataModel();
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _model = null;
        }

        [TestMethod]
        public void NewRdDataModel_AllCollectionsPresent()
        {
            Assert.IsNotNull(_model);
            Assert.IsNotNull(_model.LocalWorkspace);
            Assert.IsNotNull(_model.LocalWorkspace.Connections);
            Assert.AreEqual(0, _model.LocalWorkspace.Connections.Count);
            Assert.IsNotNull(_model.OnPremiseWorkspaces);
            Assert.AreEqual(0, _model.OnPremiseWorkspaces.Count);
            Assert.IsNotNull(_model.CloudWorkspaces);
            Assert.AreEqual(0, _model.CloudWorkspaces.Count);
            Assert.IsNotNull(_model.Credentials);
            Assert.AreEqual(0, _model.Credentials.Count);
            Assert.IsNotNull(_model.TrustedCertificates);
            Assert.AreEqual(0, _model.TrustedCertificates.Count);
            Assert.IsNull(_model.Storage);
        }

        [TestMethod]
        public void NewRdDataModel_IsCertTrusted_NoTrust()
        {
            IRdpCertificate cert = new TestCertificate(new byte[] { 0, 1, 2, 3, 4, 5 }, new byte[] { 1, 2, 3, 4, 5, 6 },
                new DateTimeOffset(), new DateTimeOffset());

            Assert.IsFalse(_model.IsCertificateTrusted(cert));
        }

        [TestMethod]
        public void NewRdDataModel_TrustCertificate_TrustEstablished()
        {
            IRdpCertificate cert = new TestCertificate(new byte[] { 0, 1, 2, 3, 4, 5 }, new byte[] { 1, 2, 3, 4, 5, 6 },
                new DateTimeOffset(), new DateTimeOffset());

            _model.TrustCertificate(cert);
            Assert.AreEqual(1, _model.TrustedCertificates.Count);
            Assert.IsTrue(_model.IsCertificateTrusted(cert));
        }

        [TestMethod]
        public void NewRdDataModel_TrustCertificate_DifferentSerialNumberNotTrusted()
        {
            IRdpCertificate cert1 = new TestCertificate(new byte[] { 0, 1, 2, 3, 4, 5 }, new byte[] { 1, 2, 3, 4, 5, 6 },
                new DateTimeOffset(), new DateTimeOffset());
            IRdpCertificate cert2 = new TestCertificate(new byte[] { 5, 0, 1, 2, 3, 4, 5 }, new byte[] { 1, 2, 3, 4, 5, 6 },
                new DateTimeOffset(), new DateTimeOffset());

            _model.TrustCertificate(cert1);
            Assert.IsFalse(_model.IsCertificateTrusted(cert2));
        }

        [TestMethod]
        public void NewRdDataModel_TrustCertificate_DifferentHashNotTrusted()
        {
            IRdpCertificate cert1 = new TestCertificate(new byte[] { 0, 1, 2, 3, 4, 5 }, new byte[] { 1, 2, 3, 4, 5, 6 },
                new DateTimeOffset(), new DateTimeOffset());
            IRdpCertificate cert2 = new TestCertificate(new byte[] { 0, 1, 2, 3, 4, 5 }, new byte[] { 0, 1, 2, 3, 4, 5, 6 },
                new DateTimeOffset(), new DateTimeOffset());

            _model.TrustCertificate(cert1);
            Assert.IsFalse(_model.IsCertificateTrusted(cert2));
        }
    }
}
