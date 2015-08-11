namespace RdClient.Shared.Test.Model
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using RdClient.Shared.Telemetry;
    using RdClient.Shared.Test.Data;

    class TestRemoteConnectionModel : RemoteConnectionModel, ICredentialsIdModel
    {
        public Guid CredentialsId { get; set; }

        public bool HasCredentials { get; set; }

        public override IRdpConnection CreateConnection(IRdpConnectionFactory connectionFactory, IRenderingPanel renderingPanel)
        {
            throw new NotImplementedException();
        }

        public override void InitializeSessionTelemetry(ApplicationDataModel dataModel, Telemetry.Events.SessionLaunch sessionLaunch)
        {
            throw new NotImplementedException();
        }

        protected override string GetTelemetrySourceType()
        {
            throw new NotImplementedException();
        }
    }

    [TestClass]
    public sealed class RemoteSessionSetupTests
    {
        private ApplicationDataModel _dataModel;
        private TestRemoteConnectionModel _connection;


        [TestInitialize]
        public void SetUpTest()
        {
            _dataModel = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer(),
                DataScrambler = new Mock.DummyDataScrambler()
            };
            _dataModel.Compose();

            _connection = new TestRemoteConnectionModel();
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _dataModel = null;
            _connection = null;
        }

        [TestMethod]
        public void RemoteSessionSetup_New_Initialized()
        {
            RemoteSessionSetup rss = new RemoteSessionSetup(_dataModel, _connection);
            Assert.IsNotNull(rss.DataModel);
        }

        [TestMethod]
        public void RemoteSessionSetup_CopyTo_Copied()
        {
            RemoteSessionSetup rss = new RemoteSessionSetup(_dataModel, _connection);
            RemoteSessionSetup rssCopy = new RemoteSessionSetup(rss);

            Assert.AreEqual(rss.DataModel, rssCopy.DataModel);
            Assert.AreEqual(rss.Connection, rssCopy.Connection);
            Assert.AreEqual(rss.HostName, "");
        }
    }
}
