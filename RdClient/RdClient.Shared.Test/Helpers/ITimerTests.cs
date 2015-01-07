﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.Helpers;
using System;
using System.Threading;

namespace RdClient.Shared.Test.Helpers
{
    public abstract class ITimerTests
    {
        private ITimerFactory _timerFactory;
        private ITimer _timer;

        public abstract ITimerFactory GetFactory();

        [TestInitialize]
        public void TestSetup()
        {
            _timerFactory = GetFactory();
            _timer = _timerFactory.CreateTimer();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _timer.Stop();
        }

        [TestMethod]
        public void TestCallStopOnNotRunningTimerDoesntThrow()
        {
            _timer.Stop();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestCallStartOnRunningTimerThrows()
        {
            TimeSpan longTimespan = new TimeSpan(1, 0, 0, 0);
            _timer.Start(() => Assert.Fail("Callback Shouldn't be called"), longTimespan, false);
            _timer.Start(() => Assert.Fail("Callback Shouldn't be called"), longTimespan, false);
        }

        [TestMethod]
        public void TestStartRecurringTimerCallsCallbackMultipleTimes()
        {
            int timerCallbacks = 0;
            TimeSpan shortTimespan = new TimeSpan(0, 0, 0, 0, 1); //1ms timer period
            _timer.Start(() => System.Threading.Interlocked.Increment(ref timerCallbacks), shortTimespan, true);
            Thread.Sleep(10);
            _timer.Stop();
            Assert.IsTrue(timerCallbacks > 1);                        
        }

        [TestMethod]
        public void TestStartNonRecurringTimerCallsCallbackSingleTime()
        {
            int timerCallbacks = 0;
            TimeSpan shortTimespan = new TimeSpan(0, 0, 0, 0, 1); //1ms timer period
            _timer.Start(() => System.Threading.Interlocked.Increment(ref timerCallbacks), shortTimespan, false);
            Thread.Sleep(10);
            _timer.Stop();
            Assert.AreEqual(1, timerCallbacks);
        }

    }
}
