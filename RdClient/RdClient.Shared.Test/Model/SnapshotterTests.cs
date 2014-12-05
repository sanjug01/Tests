using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RdClient.Shared.Models;
using RdClient.Shared.Test.Mock;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Test.Helpers;

namespace RdClient.Shared.Test.Model
{
    [TestClass]
    public class SnapshotterTests
    {
        private TestData _testData;
        private Mock.RdpEvents _mockEvents;
        private Mock.RdpConnection _mockConnection;
        private Mock.Thumbnail _mockThumb;
        private Mock.TimerFactory _mockTimerFactory;
        private Snapshotter _snapshotter;

        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();
            _mockEvents = new Mock.RdpEvents();
            _mockConnection = new Mock.RdpConnection(_mockEvents);
            _mockThumb = new Mock.Thumbnail();
            _mockTimerFactory = new Mock.TimerFactory();
            _snapshotter = new Snapshotter(_mockConnection, _mockThumb, _mockTimerFactory);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _mockEvents.Dispose();
            _mockConnection.Dispose();
            _mockThumb.Dispose();
            _mockTimerFactory.Dispose();
        }

        [TestMethod]
        public void TestTimerCreatedCorrectlyOnFirstGraphicsUpdate()
        {   
            Assert.AreEqual(0, _mockTimerFactory.CreateTimerCalls);
            _mockEvents.FireFirstGraphicsUpdate(new FirstGraphicsUpdateArgs());
            Assert.AreEqual(1, _mockTimerFactory.CreateTimerCalls);
            Assert.AreEqual(_snapshotter.firstSnapshotTime, _mockTimerFactory.Timer.Period);
            Assert.AreEqual(false, _mockTimerFactory.Timer.Recurring);
        }

        [TestMethod]
        public void TestRepeatingTimerCreatedCorrectlyAfterFirstSnapshot()
        {
            _mockEvents.FireFirstGraphicsUpdate(new FirstGraphicsUpdateArgs());
            Assert.AreEqual(1, _mockTimerFactory.CreateTimerCalls);
            _mockTimerFactory.Timer.Callback();
            Assert.AreEqual(2, _mockTimerFactory.CreateTimerCalls);
            Assert.AreEqual(_snapshotter.snapshotPeriod, _mockTimerFactory.Timer.Period);
            Assert.AreEqual(true, _mockTimerFactory.Timer.Recurring);
        }

        [TestMethod]
        public void TestSnapshotTakenWhenTimerCallbacksExecuted()
        {
            //first snapshot
            _mockEvents.FireFirstGraphicsUpdate(new FirstGraphicsUpdateArgs());
            Mock.RdpScreenSnapshot snapshot = new Mock.RdpScreenSnapshot();
            _mockConnection.Expect("GetSnapshot", new List<object>() { }, snapshot);
            _mockThumb.Expect("Update", new List<object>() { snapshot }, 0);
            _mockTimerFactory.Timer.Callback();
            //repeating snapshot
            snapshot = new Mock.RdpScreenSnapshot();
            _mockConnection.Expect("GetSnapshot", new List<object>() { }, snapshot);
            _mockThumb.Expect("Update", new List<object>() { snapshot }, 0);
            _mockTimerFactory.Timer.Callback();
        }

        [TestMethod]
        public void TestTimerCancelledOnDisconnect()
        {
            _mockEvents.FireFirstGraphicsUpdate(new FirstGraphicsUpdateArgs());
            _mockTimerFactory.Timer.Expect("Cancel", new List<object>() { }, 0);
            _mockEvents.FireClientDisconnected(new ClientDisconnectedArgs(new RdpDisconnectReason(RdpDisconnectCode.UnknownError, 0, 0)));            
        }


    }
}
