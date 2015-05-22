using System;
using System.Threading;
using Windows.UI.Xaml;

namespace RdClient.Shared.Helpers
{
    public sealed class WinrtDispatcherTimerFactory : ITimerFactory
    {
        public ITimer CreateTimer()
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
            _timer = new DispatcherTimer();
            _timer.Tick += OnTick;
        }

        public void Start(Action callback, TimeSpan period, bool recurring)
        {
            using (ReadWriteMonitor.Write(_monitor))
            {
                _timer.Interval = period;

                _recurring = recurring;
                _callback = callback;


                _timer.Start();
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
                }
            }
        }
    }
}
