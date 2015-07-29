using RdClient.Shared.Navigation.Extensions;
using System;

namespace RdClient.Shared.Helpers
{
    public class RdDispatcherTimer : ITimer
    {
        private readonly ITimer _timer;
        private readonly ISynchronizedDeferrer _dispatcher;
        private Action _callback;

        public RdDispatcherTimer(ITimer timer, ISynchronizedDeferrer dispatcher)
        {
            _timer = timer;
            _dispatcher = dispatcher;
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
            _dispatcher.DeferToUI(_callback);
        }
    }
}
