namespace RdClient.Shared.Test.Model
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Models;
    using System;

    [TestClass]
    public sealed class TrustedCertificateTests
    {
        [TestMethod]
        public void NewTrustedCertificate_DefaultConstructor_NoData()
        {
            TrustedCertificate cert = new TrustedCertificate();
            Assert.IsNull(cert.Hash);
            Assert.IsNull(cert.SerialNumber);
        }

        [TestMethod]
        public void NewTrustedCertificate_CertificateConstructor_DataCopied()
        {
            IRdpCertificate rdpc = new Mock.TestRdpCertificate(new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 },
                new DateTimeOffset(), new DateTimeOffset());

            TrustedCertificate cert = new TrustedCertificate(rdpc);
            Assert.IsNotNull(cert.Hash);
            Assert.AreEqual(rdpc.GetHashValue(), cert.Hash);
            Assert.IsNotNull(cert.SerialNumber);
            Assert.AreEqual(rdpc.SerialNumber, cert.SerialNumber);
        }

        [TestMethod]
        public void NewTrustedCertificate_MatchesOriginalCertificate()
        {
            IRdpCertificate rdpc = new Mock.TestRdpCertificate(new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 },
                new DateTimeOffset(), new DateTimeOffset());

            TrustedCertificate cert = new TrustedCertificate(rdpc);
            Assert.IsTrue(cert.IsMatchingCertificate(rdpc));
        }

        [TestMethod]
        public void NewTrustedCertificate_MatchesCertificateWithSameHashAndSerialNumber()
        {
            IRdpCertificate
                rdpc1 = new Mock.TestRdpCertificate(new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 },
                new DateTimeOffset(), new DateTimeOffset()),
                rdpc2 = new Mock.TestRdpCertificate(new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 },
                new DateTimeOffset(), new DateTimeOffset());

            TrustedCertificate cert = new TrustedCertificate(rdpc1);
            Assert.IsTrue(cert.IsMatchingCertificate(rdpc2));
        }

        [TestMethod]
        public void NewTrustedCertificate_DontMatchDifferentSerialNumber()
        {
            IRdpCertificate
                rdpc1 = new Mock.TestRdpCertificate(new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 },
                new DateTimeOffset(), new DateTimeOffset()),
                rdpc2 = new Mock.TestRdpCertificate(new byte[] { 1, 1, 3 }, new byte[] { 4, 5, 6 },
                new DateTimeOffset(), new DateTimeOffset());

            TrustedCertificate cert = new TrustedCertificate(rdpc1);
            Assert.IsFalse(cert.IsMatchingCertificate(rdpc2));
        }

        [TestMethod]
        public void NewTrustedCertificate_DontMatchDifferentHash()
        {
            IRdpCertificate
                rdpc1 = new Mock.TestRdpCertificate(new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 },
                new DateTimeOffset(), new DateTimeOffset()),
                rdpc2 = new Mock.TestRdpCertificate(new byte[] { 1, 2, 3 }, new byte[] { 1, 5, 6 },
                new DateTimeOffset(), new DateTimeOffset());

            TrustedCertificate cert = new TrustedCertificate(rdpc1);
            Assert.IsFalse(cert.IsMatchingCertificate(rdpc2));
        }
    }
}
