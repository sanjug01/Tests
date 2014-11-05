using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using RdClient.Shared.ViewModels;
using System.Collections.Generic;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class SessionViewModelTests
    {
        class TestSessionViewModel : SessionViewModel
        {
            public IRdpConnection RdpConnection { get {return _rdpConnection; } set { _rdpConnection = value; } }
        }

        private TestSessionViewModel _sessionViewModel;

        [TestInitialize]
        public void TestSetUp()
        {
            _sessionViewModel = new TestSessionViewModel();
        }

        public void TestTearDown()
        {
            _sessionViewModel = null;
        }

        [TestMethod]
        public void ConnectionCreatedArgs_Constructor()
        {
            IRdpConnection connection = new Mock.RdpConnection(null);
            ConnectionCreatedArgs cca = new ConnectionCreatedArgs(connection);

            Assert.AreEqual(connection, cca.RdpConnection);
        }

        [TestMethod]
        public void Connect()
        {
            using (Mock.RdpConnection connection = new Mock.RdpConnection(null))
            using (Mock.RdpConnectionFactory factory = new Mock.RdpConnectionFactory())
            {
                bool connectionMatches = false;
                Desktop desktop = new Desktop() { hostName = "narf" };
                Credentials credentials = new Credentials() { username = "narf", domain = "zod", password = "poit", haveBeenPersisted = true };
                ConnectionInformation connectionInformation = new ConnectionInformation() { Desktop = desktop, Credentials = credentials };

                _sessionViewModel.RdpConnectionFactory = factory;
                _sessionViewModel.ConnectionCreated += (sender, args) => { connectionMatches = (connection == (IRdpConnection)args.RdpConnection); };

                factory.Expect("CreateInstance", new System.Collections.Generic.List<object>(), connection);

                connection.Expect("SetStringProperty", new List<object>() { "Full Address", desktop.hostName }, 0);
                connection.Expect("Connect", new List<object>() { credentials, true }, 0);

                _sessionViewModel.ConnectCommand.Execute(connectionInformation);

                Assert.IsTrue(connectionMatches);
            }
        }

        [TestMethod]
        public void Disconnect()
        {
            using (Mock.RdpConnection connection = new Mock.RdpConnection(null))
            using (Mock.NavigationService navigationService = new Mock.NavigationService())
            {
                _sessionViewModel.NavigationService = navigationService;
                _sessionViewModel.RdpConnection = connection;

                object disconnectParam = new object();

                connection.Expect("Disconnect", new List<object>() { }, 0);
                navigationService.Expect("NavigateToView", new List<object>() { "view1", null }, 0);

                _sessionViewModel.DisconnectCommand.Execute(disconnectParam);
            }
        }

        [TestMethod]
        public void ClientAsyncDisconnectHandler_PreAuthLogonFailed()
        {
            RdpDisconnectReason reason = new RdpDisconnectReason(RdpDisconnectCode.PreAuthLogonFailed, 0, 0);
            ClientAsyncDisconnectArgs args = new ClientAsyncDisconnectArgs(reason);

            using (Mock.RdpConnection connection = new Mock.RdpConnection(null))
            {
                _sessionViewModel.RdpConnection = connection;

                connection.Expect("HandleAsyncDisconnectResult", new List<object>() { reason, false }, 0);

                _sessionViewModel.ClientAsyncDisconnectHandler(null, args);
            }
        }

        [TestMethod]
        public void ClientAsyncDisconnectHandler_FreshCredsRequired()
        {
            RdpDisconnectReason reason = new RdpDisconnectReason(RdpDisconnectCode.FreshCredsRequired, 0, 0);
            ClientAsyncDisconnectArgs args = new ClientAsyncDisconnectArgs(reason);

            using (Mock.RdpConnection connection = new Mock.RdpConnection(null))
            {
                _sessionViewModel.RdpConnection = connection;

                connection.Expect("HandleAsyncDisconnectResult", new List<object>() { reason, false }, 0);

                _sessionViewModel.ClientAsyncDisconnectHandler(null, args);
            }
        }

        [TestMethod]
        public void ClientAsyncDisconnectHandler_CertValidationFailed()
        {
            RdpDisconnectReason reason = new RdpDisconnectReason(RdpDisconnectCode.CertValidationFailed, 0, 0);
            ClientAsyncDisconnectArgs args = new ClientAsyncDisconnectArgs(reason);

            using (Mock.RdpConnection connection = new Mock.RdpConnection(null))
            {
                _sessionViewModel.RdpConnection = connection;

                connection.Expect("HandleAsyncDisconnectResult", new List<object>() { reason, true }, 0);

                _sessionViewModel.ClientAsyncDisconnectHandler(null, args);
            }
        }

        [TestMethod]
        public void ClientAsyncDisconnectHandler_CredSSPUnsupported()
        {
            RdpDisconnectReason reason = new RdpDisconnectReason(RdpDisconnectCode.CredSSPUnsupported, 0, 0);
            ClientAsyncDisconnectArgs args = new ClientAsyncDisconnectArgs(reason);

            using (Mock.RdpConnection connection = new Mock.RdpConnection(null))
            {
                _sessionViewModel.RdpConnection = connection;

                connection.Expect("HandleAsyncDisconnectResult", new List<object>() { reason, false }, 0);

                _sessionViewModel.ClientAsyncDisconnectHandler(null, args);
            }
        }
    }
}
