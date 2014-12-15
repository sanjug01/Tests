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
            Assert.IsNotNull(_model.LocalWorkspace.Credentials);
            Assert.AreEqual(0, _model.LocalWorkspace.Credentials.Count);
            Assert.IsNotNull(_model.TrustedCertificates);
            Assert.AreEqual(0, _model.TrustedCertificates.Count);
            Assert.IsNull(_model.Storage);
        }

        [TestMethod]
        public void NewRdDataModel_IsCertTrusted_NoTrust()
        {
            IRdpCertificate cert = new Mock.TestRdpCertificate(new byte[] { 0, 1, 2, 3, 4, 5 }, new byte[] { 1, 2, 3, 4, 5, 6 },
                new DateTimeOffset(), new DateTimeOffset());

            Assert.IsFalse(_model.IsCertificateTrusted(cert));
        }

        [TestMethod]
        public void NewRdDataModel_TrustCertificate_TrustEstablished()
        {
            IRdpCertificate cert = new Mock.TestRdpCertificate(new byte[] { 0, 1, 2, 3, 4, 5 }, new byte[] { 1, 2, 3, 4, 5, 6 },
                new DateTimeOffset(), new DateTimeOffset());

            _model.TrustCertificate(cert);
            Assert.AreEqual(1, _model.TrustedCertificates.Count);
            Assert.IsTrue(_model.IsCertificateTrusted(cert));
        }

        [TestMethod]
        public void NewRdDataModel_TrustCertificate_DifferentSerialNumberNotTrusted()
        {
            IRdpCertificate cert1 = new Mock.TestRdpCertificate(new byte[] { 0, 1, 2, 3, 4, 5 }, new byte[] { 1, 2, 3, 4, 5, 6 },
                new DateTimeOffset(), new DateTimeOffset());
            IRdpCertificate cert2 = new Mock.TestRdpCertificate(new byte[] { 5, 0, 1, 2, 3, 4, 5 }, new byte[] { 1, 2, 3, 4, 5, 6 },
                new DateTimeOffset(), new DateTimeOffset());

            _model.TrustCertificate(cert1);
            Assert.IsFalse(_model.IsCertificateTrusted(cert2));
        }

        [TestMethod]
        public void NewRdDataModel_TrustCertificate_DifferentHashNotTrusted()
        {
            IRdpCertificate cert1 = new Mock.TestRdpCertificate(new byte[] { 0, 1, 2, 3, 4, 5 }, new byte[] { 1, 2, 3, 4, 5, 6 },
                new DateTimeOffset(), new DateTimeOffset());
            IRdpCertificate cert2 = new Mock.TestRdpCertificate(new byte[] { 0, 1, 2, 3, 4, 5 }, new byte[] { 0, 1, 2, 3, 4, 5, 6 },
                new DateTimeOffset(), new DateTimeOffset());

            _model.TrustCertificate(cert1);
            Assert.IsFalse(_model.IsCertificateTrusted(cert2));
        }
    }
}
