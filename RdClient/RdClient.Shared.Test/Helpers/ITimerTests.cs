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

        public void TestSetup()
        {
            _timerFactory = GetFactory();
            _timer = _timerFactory.CreateTimer();
        }

        public void TestCleanup()
        {
            _timer.Stop();
        }

        public void TestCallStopOnNotRunningTimerDoesntThrow()
        {
            _timer.Stop();
        }

        public void TestCallStartOnRunningTimerThrows()
        {
            TimeSpan longTimespan = new TimeSpan(1, 0, 0, 0);
            _timer.Start(() => Assert.Fail("Callback Shouldn't be called"), longTimespan, false);
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                _timer.Start(() => Assert.Fail("Callback Shouldn't be called"), longTimespan, false);
            });
        }

        public void TestStartRecurringTimerCallsCallbackMultipleTimes()
        {
            int timerCallbacks = 0;
            TimeSpan shortTimespan = TimeSpan.FromMilliseconds(1.0d);
            _timer.Start(() => System.Threading.Interlocked.Increment(ref timerCallbacks), shortTimespan, true);
            UAPSleep.Sleep(50);
            _timer.Stop();
            Assert.IsTrue(timerCallbacks > 1);                        
        }
        
        public void TestStartNonRecurringTimerCallsCallbackSingleTime()
        {
            int timerCallbacks = 0;
            TimeSpan shortTimespan = TimeSpan.FromMilliseconds(1.0d);
            _timer.Start(() => System.Threading.Interlocked.Increment(ref timerCallbacks), shortTimespan, false);
            UAPSleep.Sleep(50);
            _timer.Stop();
            Assert.AreEqual(1, timerCallbacks);
        }

    }
}
