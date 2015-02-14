namespace RdClient.Shared.Test.Model
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using System;
    using System.Collections.Generic;

    [TestClass]
    public sealed class SessionModelTests
    {
        SessionModel _sm;
        Mock.RdpConnection _connection;
        Mock.RdpConnectionFactory _connectionFactory;
        Mock.TimerFactory _timerFactory;
        Mock.Timer _timer;
        Mock.Thumbnail _thumbnail;
        Dispatcher _dispatcher;
        bool _connectionMatches;

        private sealed class Dispatcher : IDeferredExecution
        {
            public IList<Action> DeferredActions = new List<Action>();

            public void Dispatch()
            {
                foreach(Action action in this.DeferredActions)
                {
                    action();
                }
                this.DeferredActions.Clear();
            }

            void IDeferredExecution.Defer(Action action)
            {
                Assert.IsNotNull(action);
                this.DeferredActions.Add(action);
            }
        }

        [TestInitialize]
        public void TestSetup()
        {
            _connection = new Mock.RdpConnection(null);
            _connectionFactory = new Mock.RdpConnectionFactory();
            _timerFactory = new Mock.TimerFactory();
            _dispatcher = new Dispatcher();
            _timer = new Mock.Timer();
            _connectionMatches = false;

            DesktopModel desktop = new DesktopModel() { HostName = "narf" };
            CredentialsModel credentials = new CredentialsModel() { Username = "narf", Password = "poit" };

            ConnectionInformation _connectionInformation = new ConnectionInformation() { Desktop = desktop, Credentials = credentials };

            _sm = new SessionModel(_connectionFactory, _dispatcher);

            _connectionFactory.Expect("CreateInstance", new List<object>(), _connection);
            _timerFactory.Expect("CreateTimer", new List<object>(), _timer);
            _connection.Expect("SetStringProperty", new List<object>() { "Full Address", desktop.HostName }, 0);
            _connection.Expect("SetBoolProperty", new List<object>() { "Administrative Session", desktop.IsAdminSession }, 0);
            _connection.Expect("SetIntProperty", new List<object>() { "AudioMode", (int) desktop.AudioMode }, 0);
            _connection.Expect("SetLeftHandedMouseMode", new List<object>() { desktop.IsSwapMouseButtons }, 0);
            _connection.Expect("Connect", new List<object>() { credentials, false/*expect non-saved credentials*/ }, 0);
            
            _sm.ConnectionCreated += (sender, args) => { _connectionMatches = (_connection == (IRdpConnection)args.RdpConnection); };
            _sm.Connect(_connectionInformation, _timerFactory, new GeneralSettings());
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _timerFactory.Dispose();
            _connectionFactory.Dispose();
            _connection.Dispose();
            Assert.AreEqual(0, _dispatcher.DeferredActions.Count);
            _dispatcher = null;
        }

        [TestMethod]
        public void ConnectionCreatedArgs_Constructor()
        {
            ConnectionCreatedArgs cca = new ConnectionCreatedArgs(_connection);
            _dispatcher.Dispatch();
            Assert.AreSame(_connection, cca.RdpConnection);
        }

        [TestMethod]
        public void SessionModel_ShouldConnect()
        {
            _dispatcher.Dispatch();
            Assert.IsTrue(_connectionMatches);            
        }

        [TestMethod]
        public void SessionModel_ShouldDisconnect()
        {
            _connection.Expect("Disconnect", new List<object>() { }, 0);
            _dispatcher.Dispatch();
            _sm.Disconnect();            
        }
    }
}
