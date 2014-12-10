using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.Models;
using System.Collections.Generic;

namespace RdClient.Shared.Test.Model
{
    [TestClass]
    public class SessionModelTests
    {
        private RdDataModel _dataModel;

        [TestInitialize]
        public void SetUpTest()
        {
            _dataModel = new RdDataModel();
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _dataModel = null;
        }

        [TestMethod]
        public void ConnectionCreatedArgs_Constructor()
        {
            IRdpConnection connection = new Mock.RdpConnection(null);
            ConnectionCreatedArgs cca = new ConnectionCreatedArgs(connection);

            Assert.AreEqual(connection, cca.RdpConnection);
        }

        [TestMethod]
        public void SessionModel_ShouldConnect()
        {
            using (Mock.RdpConnection connection = new Mock.RdpConnection(null))
            using (Mock.RdpConnectionFactory factory = new Mock.RdpConnectionFactory())
            {
                bool connectionMatches = false;
                Desktop desktop = new Desktop(_dataModel.LocalWorkspace) { HostName = "narf" };
                Credentials credentials = new Credentials() { Username = "narf", Domain = "zod", Password = "poit", HaveBeenPersisted = true };
                ConnectionInformation connectionInformation = new ConnectionInformation() { Desktop = desktop, Credentials = credentials };

                SessionModel sm = new SessionModel(factory);
                sm.ConnectionCreated += (sender, args) => { connectionMatches = (connection == (IRdpConnection)args.RdpConnection); };

                factory.Expect("CreateInstance", new List<object>(), connection);

                connection.Expect("SetStringProperty", new List<object>() { "Full Address", desktop.HostName }, 0);
                connection.Expect("Connect", new List<object>() { credentials, true }, 0);

                sm.Connect(connectionInformation);

                Assert.IsTrue(connectionMatches);
            }
        }

        [TestMethod]
        public void SessionModel_ShouldDisconnect()
        {
            using (Mock.RdpConnection connection = new Mock.RdpConnection(null))
            using (Mock.RdpConnectionFactory factory = new Mock.RdpConnectionFactory())
            {
                Desktop desktop = new Desktop(_dataModel.LocalWorkspace) { HostName = "narf" };
                Credentials credentials = new Credentials() { Username = "narf", Domain = "zod", Password = "poit", HaveBeenPersisted = true };
                ConnectionInformation connectionInformation = new ConnectionInformation() { Desktop = desktop, Credentials = credentials };

                SessionModel sm = new SessionModel(factory);

                factory.Expect("CreateInstance", new List<object>(), connection);

                connection.Expect("SetStringProperty", new List<object>() { "Full Address", desktop.HostName }, 0);
                connection.Expect("Connect", new List<object>() { credentials, true }, 0);

                sm.Connect(connectionInformation);

                connection.Expect("Disconnect", new List<object>() { }, 0);

                sm.Disconnect();
            }
        }

        [TestMethod]
        public void ClientAsyncDisconnectHandler_PreAuthLogonFailed()
        {
            RdpDisconnectReason reason = new RdpDisconnectReason(RdpDisconnectCode.PreAuthLogonFailed, 0, 0);
            ClientAsyncDisconnectArgs args = new ClientAsyncDisconnectArgs(reason);

            using (Mock.RdpConnection connection = new Mock.RdpConnection(null))
            using (Mock.RdpConnectionFactory factory = new Mock.RdpConnectionFactory())
            {
                Desktop desktop = new Desktop(_dataModel.LocalWorkspace) { HostName = "narf" };
                Credentials credentials = new Credentials() { Username = "narf", Domain = "zod", Password = "poit", HaveBeenPersisted = true };
                ConnectionInformation connectionInformation = new ConnectionInformation() { Desktop = desktop, Credentials = credentials };

                SessionModel sm = new SessionModel(factory);

                factory.Expect("CreateInstance", new List<object>(), connection);

                connection.Expect("SetStringProperty", new List<object>() { "Full Address", desktop.HostName }, 0);
                connection.Expect("Connect", new List<object>() { credentials, true }, 0);

                sm.Connect(connectionInformation);

                connection.Expect("HandleAsyncDisconnectResult", new List<object>() { reason, false }, 0);

                sm.ClientAsyncDisconnectHandler(null, args);
            }
        }

        [TestMethod]
        public void ClientAsyncDisconnectHandler_FreshCredsRequired()
        {
            RdpDisconnectReason reason = new RdpDisconnectReason(RdpDisconnectCode.FreshCredsRequired, 0, 0);
            ClientAsyncDisconnectArgs args = new ClientAsyncDisconnectArgs(reason);

            using (Mock.RdpConnection connection = new Mock.RdpConnection(null))
            using (Mock.RdpConnectionFactory factory = new Mock.RdpConnectionFactory())
            {
                Desktop desktop = new Desktop(_dataModel.LocalWorkspace) { HostName = "narf" };
                Credentials credentials = new Credentials() { Username = "narf", Domain = "zod", Password = "poit", HaveBeenPersisted = true };
                ConnectionInformation connectionInformation = new ConnectionInformation() { Desktop = desktop, Credentials = credentials };

                SessionModel sm = new SessionModel(factory);

                factory.Expect("CreateInstance", new List<object>(), connection);

                connection.Expect("SetStringProperty", new List<object>() { "Full Address", desktop.HostName }, 0);
                connection.Expect("Connect", new List<object>() { credentials, true }, 0);

                sm.Connect(connectionInformation);

                connection.Expect("HandleAsyncDisconnectResult", new List<object>() { reason, false }, 0);

                sm.ClientAsyncDisconnectHandler(null, args);
            }
        }

        [TestMethod]
        public void ClientAsyncDisconnectHandler_CertValidationFailed()
        {
            RdpDisconnectReason reason = new RdpDisconnectReason(RdpDisconnectCode.CertValidationFailed, 0, 0);
            ClientAsyncDisconnectArgs args = new ClientAsyncDisconnectArgs(reason);

            using (Mock.RdpConnection connection = new Mock.RdpConnection(null))
            using (Mock.RdpConnectionFactory factory = new Mock.RdpConnectionFactory())
            {
                Desktop desktop = new Desktop(_dataModel.LocalWorkspace) { HostName = "narf" };
                Credentials credentials = new Credentials() { Username = "narf", Domain = "zod", Password = "poit", HaveBeenPersisted = true };
                ConnectionInformation connectionInformation = new ConnectionInformation() { Desktop = desktop, Credentials = credentials };

                SessionModel sm = new SessionModel(factory);

                factory.Expect("CreateInstance", new List<object>(), connection);

                connection.Expect("SetStringProperty", new List<object>() { "Full Address", desktop.HostName }, 0);
                connection.Expect("Connect", new List<object>() { credentials, true }, 0);

                sm.Connect(connectionInformation);

                connection.Expect("HandleAsyncDisconnectResult", new List<object>() { reason, true }, 0);

                sm.ClientAsyncDisconnectHandler(null, args);
            }
        }

        [TestMethod]
        public void ClientAsyncDisconnectHandler_CredSSPUnsupported()
        {
            RdpDisconnectReason reason = new RdpDisconnectReason(RdpDisconnectCode.CredSSPUnsupported, 0, 0);
            ClientAsyncDisconnectArgs args = new ClientAsyncDisconnectArgs(reason);

            using (Mock.RdpConnection connection = new Mock.RdpConnection(null))
            using (Mock.RdpConnectionFactory factory = new Mock.RdpConnectionFactory())
            {
                Desktop desktop = new Desktop(_dataModel.LocalWorkspace) { HostName = "narf" };
                Credentials credentials = new Credentials() { Username = "narf", Domain = "zod", Password = "poit", HaveBeenPersisted = true };
                ConnectionInformation connectionInformation = new ConnectionInformation() { Desktop = desktop, Credentials = credentials };

                SessionModel sm = new SessionModel(factory);

                factory.Expect("CreateInstance", new List<object>(), connection);

                connection.Expect("SetStringProperty", new List<object>() { "Full Address", desktop.HostName }, 0);
                connection.Expect("Connect", new List<object>() { credentials, true }, 0);

                sm.Connect(connectionInformation);

                connection.Expect("HandleAsyncDisconnectResult", new List<object>() { reason, false }, 0);

                sm.ClientAsyncDisconnectHandler(null, args);
            }
        }
    }
}
