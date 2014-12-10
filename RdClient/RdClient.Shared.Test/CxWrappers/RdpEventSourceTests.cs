using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Errors;

namespace RdClient.Shared.Test.CxWrappers
{
    [TestClass]
    public class RdpEventSourceTests
    {
        private RdpEventSource _eventSource;
        private IRdpConnection _mockRdpConnection;

        [TestInitialize]
        public void TestSetUp()
        {
            _eventSource = new RdpEventSource();
            _mockRdpConnection = new Mock.RdpConnection(_eventSource);
        }
        
        [TestCleanup]
        public void TestTearDown()
        {
            _eventSource = null;
        }

        [TestMethod]
        public void EmitClientConnected_Emits()
        {
            int called = 0;

            _eventSource.EmitClientConnected(_mockRdpConnection, new ClientConnectedArgs());

            _eventSource.ClientConnected += (src, args) => { called++; };

            _eventSource.EmitClientConnected(_mockRdpConnection, new ClientConnectedArgs());

            Assert.AreEqual(1, called);
        }

        [TestMethod]
        public void EmitClientAsyncDisconnect_Emits()
        {
            int called = 0;

            _eventSource.EmitClientAsyncDisconnect(_mockRdpConnection, new ClientAsyncDisconnectArgs(new RdpDisconnectReason(RdpDisconnectCode.UserInitiated, 23, 42)));

            _eventSource.ClientAsyncDisconnect += (src, args) => { called++; };

            _eventSource.EmitClientAsyncDisconnect(_mockRdpConnection, new ClientAsyncDisconnectArgs(new RdpDisconnectReason(RdpDisconnectCode.UserInitiated, 23, 42)));

            Assert.AreEqual(1, called);
        }

        [TestMethod]
        public void EmitClientDisconnected_Emits()
        {
            int called = 0;
            
            _eventSource.EmitClientDisconnected(_mockRdpConnection, new ClientDisconnectedArgs(new RdpDisconnectReason(RdpDisconnectCode.UserInitiated, 23, 42)));

            _eventSource.ClientDisconnected += (src, args) => { called++; };

            _eventSource.EmitClientDisconnected(_mockRdpConnection, new ClientDisconnectedArgs(new RdpDisconnectReason(RdpDisconnectCode.UserInitiated, 23, 42)));
            
            Assert.AreEqual(1, called);
        }

        [TestMethod]
        public void UserCredentialsRequest_Emits()
        {
            int called = 0;

            _eventSource.EmitUserCredentialsRequest(_mockRdpConnection, new UserCredentialsRequestArgs(23));

            _eventSource.UserCredentialsRequest += (src, args) => { called++; };

            _eventSource.EmitUserCredentialsRequest(_mockRdpConnection, new UserCredentialsRequestArgs(23));

            Assert.AreEqual(1, called);
        }

        [TestMethod]
        public void MouseCursorShapeChanged_Emits()
        {
            int called = 0;

            _eventSource.EmitMouseCursorShapeChanged(_mockRdpConnection, new MouseCursorShapeChangedArgs(new byte[] { 23 }, 23, 42, 5, 6));

            _eventSource.MouseCursorShapeChanged += (src, args) => { called++; };

            _eventSource.EmitMouseCursorShapeChanged(_mockRdpConnection, new MouseCursorShapeChangedArgs(new byte[] { 23 }, 23, 42, 5, 6));

            Assert.AreEqual(1, called);
        }

        [TestMethod]
        public void MouseCursorPositionChanged_Emits()
        {
            int called = 0;

            _eventSource.EmitMouseCursorPositionChanged(_mockRdpConnection, new MouseCursorPositionChangedArgs(23, 42));

            _eventSource.MouseCursorPositionChanged += (src, args) => { called++; };

            _eventSource.EmitMouseCursorPositionChanged(_mockRdpConnection, new MouseCursorPositionChangedArgs(23, 42));

            Assert.AreEqual(1, called);
        }

        [TestMethod]
        public void MultiTouchEnabledChanged_Emits()
        {
            int called = 0;

            _eventSource.EmitMultiTouchEnabledChanged(_mockRdpConnection, new MultiTouchEnabledChangedArgs(true));

            _eventSource.MultiTouchEnabledChanged += (src, args) => { called++; };

            _eventSource.EmitMultiTouchEnabledChanged(_mockRdpConnection, new MultiTouchEnabledChangedArgs(true));

            Assert.AreEqual(1, called);
        }

        [TestMethod]
        public void ConnectionHealthStateChanged_Emits()
        {
            int called = 0;

            _eventSource.EmitConnectionHealthStateChanged(_mockRdpConnection, new ConnectionHealthStateChangedArgs(23));

            _eventSource.ConnectionHealthStateChanged += (src, args) => { called++; };

            _eventSource.EmitConnectionHealthStateChanged(_mockRdpConnection, new ConnectionHealthStateChangedArgs(23));

            Assert.AreEqual(1, called);
        }

        [TestMethod]
        public void ClientAutoReconnecting_Emits()
        {
            int called = 0;
            AutoReconnectError error = new AutoReconnectError(23);

            _eventSource.EmitClientAutoReconnecting(_mockRdpConnection, new ClientAutoReconnectingArgs(error, 5, (b) => { }));

            _eventSource.ClientAutoReconnecting += (src, args) => { called++; };

            _eventSource.EmitClientAutoReconnecting(_mockRdpConnection, new ClientAutoReconnectingArgs(error, 5, (b) => { }));

            Assert.AreEqual(1, called);
        }


        [TestMethod]
        public void ClientAutoReconnectComplete_Emits()
        {
            int called = 0;

            _eventSource.EmitClientAutoReconnectComplete(_mockRdpConnection, new ClientAutoReconnectCompleteArgs());

            _eventSource.ClientAutoReconnectComplete += (src, args) => { called++; };

            _eventSource.EmitClientAutoReconnectComplete(_mockRdpConnection, new ClientAutoReconnectCompleteArgs());

            Assert.AreEqual(1, called);
        }

        [TestMethod]
        public void LoginCompleted()
        {
            int called = 0;

            _eventSource.EmitLoginCompleted(_mockRdpConnection, new LoginCompletedArgs());

            _eventSource.LoginCompleted += (src, args) => { called++; };

            _eventSource.EmitLoginCompleted(_mockRdpConnection, new LoginCompletedArgs());

            Assert.AreEqual(1, called);
        }

        [TestMethod]
        public void StatusInfoReceived()
        {
            int called = 0;

            _eventSource.EmitStatusInfoReceived(_mockRdpConnection, new StatusInfoReceivedArgs(23));

            _eventSource.StatusInfoReceived += (src, args) => { called++; };

            _eventSource.EmitStatusInfoReceived(_mockRdpConnection, new StatusInfoReceivedArgs(23));

            Assert.AreEqual(1, called);
        }

        [TestMethod]
        public void FirstGraphicsUpdate_Emits()
        {
            int called = 0;

            _eventSource.EmitFirstGraphicsUpdate(_mockRdpConnection, new FirstGraphicsUpdateArgs());

            _eventSource.FirstGraphicsUpdate += (src, args) => { called++; };

            _eventSource.EmitFirstGraphicsUpdate(_mockRdpConnection, new FirstGraphicsUpdateArgs());

            Assert.AreEqual(1, called);
        }

        [TestMethod]
        public void RemoteAppWindowCreated_Emits()
        {
            int called = 0;

            _eventSource.EmitRemoteAppWindowCreated(_mockRdpConnection, new RemoteAppWindowCreatedArgs(23, "narf", new byte[] {23}, 5, 6));

            _eventSource.RemoteAppWindowCreated += (src, args) => { called++; };

            _eventSource.EmitRemoteAppWindowCreated(_mockRdpConnection, new RemoteAppWindowCreatedArgs(23, "narf", new byte[] {23}, 5, 6));

            Assert.AreEqual(1, called);
        }

        [TestMethod]
        public void RemoteAppWindowDeleted_Emits()
        {
            int called = 0;

            _eventSource.EmitRemoteAppWindowDeleted(_mockRdpConnection, new RemoteAppWindowDeletedArgs(23));

            _eventSource.RemoteAppWindowDeleted += (src, args) => { called++; };

            _eventSource.EmitRemoteAppWindowDeleted(_mockRdpConnection, new RemoteAppWindowDeletedArgs(23));

            Assert.AreEqual(1, called);
        }

        [TestMethod]
        public void RemoteAppWindowTitleUpdated_Emits()
        {
            int called = 0;

            _eventSource.EmitRemoteAppWindowTitleUpdated(_mockRdpConnection, new RemoteAppWindowTitleUpdatedArgs(23, "narf"));

            _eventSource.RemoteAppWindowTitleUpdated += (src, args) => { called++; };

            _eventSource.EmitRemoteAppWindowTitleUpdated(_mockRdpConnection, new RemoteAppWindowTitleUpdatedArgs(23, "narf"));

            Assert.AreEqual(1, called);  
        }

        [TestMethod]
        public void RemoteAppWindowIconUpdated_Emits()
        {
            int called = 0;

            _eventSource.EmitRemoteAppWindowIconUpdated(_mockRdpConnection, new RemoteAppWindowIconUpdatedArgs(23, new byte[] {23}, 5, 6));

            _eventSource.RemoteAppWindowIconUpdated += (src, args) => { called++; };

            _eventSource.EmitRemoteAppWindowIconUpdated(_mockRdpConnection, new RemoteAppWindowIconUpdatedArgs(23, new byte[] {23}, 5, 6));

            Assert.AreEqual(1, called);
        }
    }
}
