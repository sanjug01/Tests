namespace RdClient.Shared.Test.Model
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Models;

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
            Assert.IsNotNull(_model.Credentials);
            Assert.AreEqual(0, _model.Credentials.Count);
            Assert.IsNotNull(_model.TrustedCertificates);
            Assert.AreEqual(0, _model.TrustedCertificates.Count);
            Assert.IsNull(_model.Storage);
        }
    }
}
