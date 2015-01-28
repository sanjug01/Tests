namespace RdClient.Shared.Test.Data
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Data;
    using RdClient.Shared.Test.Mock;
    using System.Collections.Generic;
    using System.IO;

    [TestClass]
    public sealed class CertificateTrustTests
    {
        [TestMethod]
        public void CertificateTrustTests_BumpUpCodeCoverage()
        {
            IRdpCertificate cert = new TestRdpCertificate();
            Assert.IsNotNull(cert.FriendlyName);
            Assert.IsNotNull(cert.Issuer);
            Assert.IsNotNull(cert.SerialNumber);
            Assert.IsNotNull(cert.Subject);
        }

        [TestMethod]
        public void NewCertificateTrust_IsTrusted_NotTrusted()
        {
            TestRdpCertificate cert = new TestRdpCertificate("Issuer", new byte[] { 0, 1, 2, 3, 4, 5 });
            ICertificateTrust trust = new CertificateTrust();

            Assert.IsFalse(trust.IsCertificateTrusted(cert));
        }

        [TestMethod]
        public void CertificateTrust_TrustCertificate_IsTrusted()
        {
            TestRdpCertificate cert = new TestRdpCertificate("Issuer", new byte[] { 0, 1, 2, 3, 4, 5 });
            ICertificateTrust trust = new CertificateTrust();

            trust.TrustCertificate(cert);
            Assert.IsTrue(trust.IsCertificateTrusted(cert));
        }

        [TestMethod]
        public void CertificateTrust_TrustManyRandom_AllTrusted()
        {
            ICertificateTrust trust = new CertificateTrust();
            IList<IRdpCertificate> certificates = new List<IRdpCertificate>();

            for(int i = 0; i < 250; ++i)
            {
                IRdpCertificate cert = new TestRdpCertificate();
                certificates.Add(cert);
                trust.TrustCertificate(cert);
            }

            foreach(IRdpCertificate cert in certificates)
            {
                Assert.IsTrue(trust.IsCertificateTrusted(cert));
            }
        }

        [TestMethod]
        public void CertificateTrust_TrustManyRandomClear_NothingTrusted()
        {
            ICertificateTrust trust = new CertificateTrust();
            IList<IRdpCertificate> certificates = new List<IRdpCertificate>();

            for (int i = 0; i < 250; ++i)
            {
                IRdpCertificate cert = new TestRdpCertificate();
                certificates.Add(cert);
                trust.TrustCertificate(cert);
            }

            trust.RemoveAllTrust();

            foreach (IRdpCertificate cert in certificates)
            {
                Assert.IsFalse(trust.IsCertificateTrusted(cert));
            }
        }

        [TestMethod]
        public void CertificateTrust_TrustManyRandomSaveLoad_AllLoadedTrusted()
        {
            ICertificateTrust trust = new CertificateTrust();
            IList<IRdpCertificate> certificates = new List<IRdpCertificate>();

            for (int i = 0; i < 250; ++i)
            {
                IRdpCertificate cert = new TestRdpCertificate();
                certificates.Add(cert);
                trust.TrustCertificate(cert);
            }

            MemoryStream stream = new MemoryStream();
            IModelSerializer serializer = new SerializableModelSerializer();

            serializer.WriteModel((CertificateTrust)trust, stream);
            stream.Seek(0, SeekOrigin.Begin);
            ICertificateTrust loadedTrust = serializer.ReadModel<CertificateTrust>(stream);

            foreach (IRdpCertificate cert in certificates)
            {
                Assert.IsTrue(loadedTrust.IsCertificateTrusted(cert));
            }
        }

        [TestMethod]
        public void NewCertificateTrust_RemoveAllSaveLoad_NothingLoadedTrusted()
        {
            ICertificateTrust trust = new CertificateTrust();
            IList<IRdpCertificate> certificates = new List<IRdpCertificate>();

            for (int i = 0; i < 250; ++i)
            {
                IRdpCertificate cert = new TestRdpCertificate();
                certificates.Add(cert);
                trust.TrustCertificate(cert);
            }
            trust.RemoveAllTrust();

            MemoryStream stream = new MemoryStream();
            IModelSerializer serializer = new SerializableModelSerializer();

            serializer.WriteModel((CertificateTrust)trust, stream);
            stream.Seek(0, SeekOrigin.Begin);
            ICertificateTrust loadedTrust = serializer.ReadModel<CertificateTrust>(stream);

            foreach (IRdpCertificate cert in certificates)
            {
                Assert.IsFalse(loadedTrust.IsCertificateTrusted(cert));
            }
        }

        [TestMethod]
        public void CertificateTrust_TrustSameIssuerDifferentSerials_BothTrusted()
        {
            ICertificateTrust trust = new CertificateTrust();

            IRdpCertificate cert1 = new TestRdpCertificate("Issuer", new byte[] { 0, 1, 2, 3, 4, 5 });
            IRdpCertificate cert2 = new TestRdpCertificate("Issuer", new byte[] { 0, 1, 2, 3, 4 });

            trust.TrustCertificate(cert1);
            trust.TrustCertificate(cert2);
            Assert.IsTrue(trust.IsCertificateTrusted(cert1));
            Assert.IsTrue(trust.IsCertificateTrusted(cert2));

            cert1 = new TestRdpCertificate("Issuer", new byte[] { 0, 1, 2, 3, 4, 5 });
            cert2 = new TestRdpCertificate("Issuer", new byte[] { 0, 1, 2, 3, 4, 5, 6 });
            trust.RemoveAllTrust();
            trust.TrustCertificate(cert1);
            trust.TrustCertificate(cert2);
            Assert.IsTrue(trust.IsCertificateTrusted(cert1));
            Assert.IsTrue(trust.IsCertificateTrusted(cert2));

            cert1 = new TestRdpCertificate("Issuer", new byte[] { 0, 1, 3, 3, 4, 5 });
            cert2 = new TestRdpCertificate("Issuer", new byte[] { 0, 1, 2, 3, 4, 5 });
            trust.RemoveAllTrust();
            trust.TrustCertificate(cert1);
            trust.TrustCertificate(cert2);
            Assert.IsTrue(trust.IsCertificateTrusted(cert1));
            Assert.IsTrue(trust.IsCertificateTrusted(cert2));

            cert1 = new TestRdpCertificate("Issuer", new byte[] { 0, 1, 2, 3, 4, 5 });
            cert2 = new TestRdpCertificate("Issuer", new byte[] { 0, 1, 3, 3, 4, 5 });
            trust.RemoveAllTrust();
            trust.TrustCertificate(cert1);
            trust.TrustCertificate(cert2);
            Assert.IsTrue(trust.IsCertificateTrusted(cert1));
            Assert.IsTrue(trust.IsCertificateTrusted(cert2));
        }

        [TestMethod]
        public void CertificateTrust_TrustSameSerialsDifferentIssuers_BothTrusted()
        {
            ICertificateTrust trust = new CertificateTrust();

            IRdpCertificate cert1 = new TestRdpCertificate("Issuer1", new byte[] { 0, 1, 2, 3, 4, 5 });
            IRdpCertificate cert2 = new TestRdpCertificate("Issuer2", new byte[] { 0, 1, 2, 3, 4, 5 });

            trust.TrustCertificate(cert1);
            trust.TrustCertificate(cert2);
            Assert.IsTrue(trust.IsCertificateTrusted(cert1));
            Assert.IsTrue(trust.IsCertificateTrusted(cert2));

            cert1 = new TestRdpCertificate("Issuer2", new byte[] { 0, 1, 2, 3, 4, 5 });
            cert2 = new TestRdpCertificate("Issuer1", new byte[] { 0, 1, 2, 3, 4, 5 });
            trust.RemoveAllTrust();
            trust.TrustCertificate(cert1);
            trust.TrustCertificate(cert2);
            Assert.IsTrue(trust.IsCertificateTrusted(cert1));
            Assert.IsTrue(trust.IsCertificateTrusted(cert2));
        }
    }
}
