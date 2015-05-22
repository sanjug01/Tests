using System;
using System.Threading;
using Windows.System.Threading;

namespace RdClient.Shared.Helpers
{
    public sealed class WinrtThreadPoolTimerFactory : ITimerFactory
    {
        public ITimer CreateTimer()
        {
            return new WinrtThreadPoolTimer();
        }
    }

    public class WinrtThreadPoolTimer : ITimer
    {
        private ThreadPoolTimer _timer;
        private readonly ReaderWriterLockSlim _monitor;

        public WinrtThreadPoolTimer()
        {
            _monitor = new ReaderWriterLockSlim();
        }

        public void Start(Action callback, TimeSpan period, bool recurring)
        {
            using (ReadWriteMonitor.Write(_monitor))
            {
                if (_timer != null)
                {
                    throw new InvalidOperationException("Cannot start timer as it is already running");
                }
                {
                    if (recurring)
                    {
                        _timer = ThreadPoolTimer.CreatePeriodicTimer((timer) => callback(), period);
                    }
                    else
                    {
                        _timer = ThreadPoolTimer.CreateTimer((timer) => callback(), period);
                    }
                }
            }
        }

        public void Stop()
        {
            using (ReadWriteMonitor.Write(_monitor))
            {
                if (_timer != null)
                {
                    _timer.Cancel();
                    _timer = null;
                }
            }
        }
    }
}
