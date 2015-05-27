using RdClient.Shared.Navigation.Extensions;
using System;

namespace RdClient.Shared.Helpers
{
    public class RdDispatcherTimer : ITimer
    {
        private readonly ITimer _timer;
        private readonly IExecutionDeferrer _deferrer;
        private Action _callback;

        public RdDispatcherTimer(ITimer timer, IExecutionDeferrer deferrer)
        {
            _timer = timer;
            _deferrer = deferrer;
        }

        public void Start(Action callback, TimeSpan period, bool recurring)
        {
            _timer.Stop();
            _callback = callback;
            _timer.Start(InternalCallback, period, recurring);
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void InternalCallback()
        {
            _deferrer.DeferToUI(_callback);
        }
    }
}
