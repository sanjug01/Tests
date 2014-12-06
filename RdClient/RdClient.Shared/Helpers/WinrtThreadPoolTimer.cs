using System;
using Windows.System.Threading;

namespace RdClient.Shared.Helpers
{
    public sealed class WinrtThreadPoolTimerFactory : ITimerFactory
    {
        public ITimer CreateTimer(Action callback, TimeSpan period, bool recurring)
        {
            return new WinrtThreadPoolTimer(callback, period, recurring);
        }
    }

    public class WinrtThreadPoolTimer : ITimer
    {
        private readonly ThreadPoolTimer _timer;

        public WinrtThreadPoolTimer(Action callback, TimeSpan period, bool recurring)
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

        public void Cancel()
        {
            _timer.Cancel();
        }
    }
}
