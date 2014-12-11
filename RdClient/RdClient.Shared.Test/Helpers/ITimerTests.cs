using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.Helpers;
using System;

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

    }
}
