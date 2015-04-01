using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.Helpers;
using RdClient.Shared.Test.UAP;
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
        public void TestCallStartOnRunningTimerThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<InvalidOperationException>(() =>
            {

                TimeSpan longTimespan = new TimeSpan(1, 0, 0, 0);
                _timer.Start(() => Assert.Fail("Callback Shouldn't be called"), longTimespan, false);
                _timer.Start(() => Assert.Fail("Callback Shouldn't be called"), longTimespan, false);
            }));
        }

        [TestMethod]
        public void TestStartRecurringTimerCallsCallbackMultipleTimes()
        {
            int timerCallbacks = 0;
            TimeSpan shortTimespan = TimeSpan.FromMilliseconds(1.0d);
            _timer.Start(() => System.Threading.Interlocked.Increment(ref timerCallbacks), shortTimespan, true);
            UAPSleep.Sleep(10);
            _timer.Stop();
            Assert.IsTrue(timerCallbacks > 1);                        
        }

        [TestMethod]
        public void TestStartNonRecurringTimerCallsCallbackSingleTime()
        {
            int timerCallbacks = 0;
            TimeSpan shortTimespan = TimeSpan.FromMilliseconds(1.0d);
            _timer.Start(() => System.Threading.Interlocked.Increment(ref timerCallbacks), shortTimespan, false);
            UAPSleep.Sleep(10);
            _timer.Stop();
            Assert.AreEqual(1, timerCallbacks);
        }

    }
}
