using System;

namespace RdClient.Shared.Helpers
{
    public class Debouncer
    {
        private Action _action;
        private ITimer _timer;
        private TimeSpan _period;

        public Debouncer(Action action, ITimer timer, TimeSpan period)
        {
            _action = action;
            _timer = timer;
            _period = period;
        }

        public void Call()
        {
            _timer.Stop();
            _timer.Start(_action, _period, false);
        }
    }
}
