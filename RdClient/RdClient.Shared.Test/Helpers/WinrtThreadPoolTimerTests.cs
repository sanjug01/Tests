namespace RdClient.Shared.Test.Helpers
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Test.UAP;
    using System;

    [TestClass]
    public class WinrtThreadPoolTimerTests
    {
        private ITimerFactory _timerFactory;
        private ITimer _timer;

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
        public void CallStopOnNotRunningTimerDoesntThrow()
        {
            _timer.Stop();
        }

        [TestMethod]
        public void CallStartOnRunningTimerThrows()
        {
            TimeSpan longTimespan = new TimeSpan(1, 0, 0, 0);
            _timer.Start(() => Assert.Fail("Callback Shouldn't be called"), longTimespan, false);
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                _timer.Start(() => Assert.Fail("Callback Shouldn't be called"), longTimespan, false);
            });
        }

        [TestMethod]
        public void StartRecurringTimerCallsCallbackMultipleTimes()
        {
            int timerCallbacks = 0;
            TimeSpan shortTimespan = TimeSpan.FromMilliseconds(1.0d);
            _timer.Start(() => System.Threading.Interlocked.Increment(ref timerCallbacks), shortTimespan, true);
            UAPSleep.Sleep(50);
            _timer.Stop();
            Assert.IsTrue(timerCallbacks > 1);
        }

        [TestMethod]
        public void StartNonRecurringTimerCallsCallbackSingleTime()
        {
            int timerCallbacks = 0;
            TimeSpan shortTimespan = TimeSpan.FromMilliseconds(1.0d);
            _timer.Start(() => System.Threading.Interlocked.Increment(ref timerCallbacks), shortTimespan, false);
            UAPSleep.Sleep(50);
            _timer.Stop();
            Assert.AreEqual(1, timerCallbacks);
        }

        private ITimerFactory GetFactory()
        {
            return new WinrtThreadPoolTimerFactory();
        }
    }
}
