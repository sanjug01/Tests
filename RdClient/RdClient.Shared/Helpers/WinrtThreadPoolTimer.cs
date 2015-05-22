using System;
using System.Threading;
using Windows.System.Threading;
using Windows.UI.Xaml;

namespace RdClient.Shared.Helpers
{
    public sealed class WinrtThreadPoolTimerFactory : ITimerFactory
    {
        public ITimer CreateTimer()
        {
            return new WinrtThreadPoolTimer();
        }

        public ITimer CreateDispatcherTimer()
        {
            return new WinrtDispatcherTimer();
        }
    }

    public class WinrtDispatcherTimer : ITimer
    {
        private DispatcherTimer _timer;
        private bool _recurring;
        private Action _callback;

        private readonly ReaderWriterLockSlim _monitor;

        public WinrtDispatcherTimer()
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
                    _timer = new DispatcherTimer();
                    _timer.Interval = period;

                    _recurring = recurring;
                    _callback = callback;

                    _timer.Tick += OnTick;

                    _timer.Start();
                }
            }
        }

        private void OnTick(object sender, object e)
        {
            _callback();

            if (_recurring == false)
            {
                Stop();
            }
        }

        public void Stop()
        {
            using (ReadWriteMonitor.Write(_monitor))
            {
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer = null;
                }
            }
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
