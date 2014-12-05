using System;
using Windows.System.Threading;

namespace RdClient.Shared.Helpers
{
    public class WinrtThreadPoolTimerFactory : ITimerFactory
    {
        public ITimer CreateTimer(Action callback, TimeSpan period, bool recurring)
        {            
            if (recurring)
            {
                return new WinrtThreadPoolTimer(ThreadPoolTimer.CreatePeriodicTimer((timer) => callback(), period));
            }
            else
            {
                return new WinrtThreadPoolTimer(ThreadPoolTimer.CreateTimer((timer) => callback(), period));
            }
        }
    }

    public class WinrtThreadPoolTimer : ITimer
    {
        private ThreadPoolTimer _timer;

        public WinrtThreadPoolTimer(ThreadPoolTimer timer)
        {
            _timer = timer;
        }

        public void Cancel()
        {
            _timer.Cancel();
        }
    }
}
