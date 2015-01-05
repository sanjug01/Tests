using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.Models;
using RdClient.Shared.Test.Helpers;
using System.Collections.Generic;

namespace RdClient.Shared.Test.Model
{
    [TestClass]
    public class SnapshotterTests
    {
        private TestData _testData;
        private Mock.RdpConnection _mockConnection;
        private Mock.Thumbnail _mockThumb;
        private Mock.Timer _mockTimer;
        private Mock.TimerFactory _mockTimerFactory;
        private RdpEventSource _eventSource;
        private Snapshotter _snapshotter;
        private GeneralSettings _settings;

        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();
            _eventSource = new RdpEventSource();
            _mockConnection = new Mock.RdpConnection(_eventSource);
            _settings = new GeneralSettings();
            _settings.UseThumbnails = true;
            _mockThumb = new Mock.Thumbnail();
            _mockTimer = new Mock.Timer();
            _mockTimerFactory = new Mock.TimerFactory();
            _mockTimerFactory.Expect("CreateTimer", new List<object>() { }, _mockTimer);
            _snapshotter = new Snapshotter(_mockConnection, _mockThumb, _mockTimerFactory, _settings);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _mockConnection.Dispose();
            _mockThumb.Dispose();
            _mockTimerFactory.Dispose();
        }

        [TestMethod]
        public void TestTimerNotStartedBeforeFirstGraphicsUpdate()
        {
            Assert.AreEqual(0, _mockTimer.CallsToStart);
            Assert.IsFalse(_mockTimer.Running);
        }

        [TestMethod]
        public void TestTimerStartedCorrectlyOnFirstGraphicsUpdate()
        {
            _eventSource.EmitFirstGraphicsUpdate(_mockConnection, new FirstGraphicsUpdateArgs());
            Assert.AreEqual(1, _mockTimer.CallsToStart);
            Assert.IsTrue(_mockTimer.Running);
            Assert.AreEqual(_snapshotter.firstSnapshotTime, _mockTimer.Period);
            Assert.IsFalse(_mockTimer.Recurring);
        }

        [TestMethod]
        public void TestRepeatingTimerCreatedCorrectlyAfterFirstSnapshot()
        {
            _eventSource.EmitFirstGraphicsUpdate(_mockConnection, new FirstGraphicsUpdateArgs());
            Assert.IsNotNull(_mockTimer.Callback);
            _mockTimer.Callback();            
            Assert.AreEqual(2, _mockTimer.CallsToStart);
            Assert.IsTrue(_mockTimer.Running);
            Assert.AreEqual(_snapshotter.snapshotPeriod, _mockTimer.Period);
            Assert.AreEqual(true, _mockTimer.Recurring);
        }

        [TestMethod]
        public void TestSnapshotTakenWhenTimerCallbacksExecuted()
        {
            Mock.RdpScreenSnapshot snapshot = new Mock.RdpScreenSnapshot();
            //first snapshot
            _eventSource.EmitFirstGraphicsUpdate(_mockConnection, new FirstGraphicsUpdateArgs());       
            _mockConnection.Expect("GetSnapshot", new List<object>() { }, snapshot);
            _mockThumb.Expect("Update", new List<object>() { snapshot }, 0);
            _mockTimer.Callback();
            //repeating snapshot
            snapshot = new Mock.RdpScreenSnapshot();
            _mockConnection.Expect("GetSnapshot", new List<object>() { }, snapshot);
            _mockThumb.Expect("Update", new List<object>() { snapshot }, 0);
            _mockTimer.Callback();
        }

        [TestMethod]
        public void TestSnapshotsNotTakenWhenUseThumbnailsSettingIsFalse()
        {
            _settings.UseThumbnails = false;
            //initial snapshot timer set on first graphics update
            _eventSource.EmitFirstGraphicsUpdate(_mockConnection, new FirstGraphicsUpdateArgs());
            _mockTimer.Callback();
            //repeating snapshot set on callback to initial snapshot timer
            _mockTimer.Callback();
        }

        [TestMethod]
        public void TestTimerCancelledOnDisconnectBeforeFirstGraphicsUpdate()
        {
            _eventSource.EmitClientDisconnected(_mockConnection, new ClientDisconnectedArgs(new RdpDisconnectReason(RdpDisconnectCode.UnknownError, 0, 0)));            
            Assert.IsFalse(_mockTimer.Running);
        }

        [TestMethod]
        public void TestTimerCancelledOnDisconnectBeforeFirstSnapshot()
        {
            _eventSource.EmitFirstGraphicsUpdate(_mockConnection, new FirstGraphicsUpdateArgs());
            _eventSource.EmitClientDisconnected(_mockConnection, new ClientDisconnectedArgs(new RdpDisconnectReason(RdpDisconnectCode.UnknownError, 0, 0)));            
            Assert.IsFalse(_mockTimer.Running);
        }

        public void TestTimerCancelledOnDisconnectAfterFirstSnapshot()
        {
            _eventSource.EmitFirstGraphicsUpdate(_mockConnection, new FirstGraphicsUpdateArgs());
            _mockTimer.Callback();
            _eventSource.EmitClientDisconnected(_mockConnection, new ClientDisconnectedArgs(new RdpDisconnectReason(RdpDisconnectCode.UnknownError, 0, 0)));
            Assert.IsFalse(_mockTimer.Running);
        }
    }
}
