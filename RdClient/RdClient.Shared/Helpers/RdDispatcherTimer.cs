using System;

namespace RdClient.Shared.Helpers
{
    public class RdDispatcherTimer : ITimer
    {
        private readonly ITimer _timer;
        private readonly IDeferredExecution _dispatcher;
        private Action _callback;

        public RdDispatcherTimer(ITimer timer, IDeferredExecution dispatcher)
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
            _dispatcher.Defer(_callback);
        }
    }
}
