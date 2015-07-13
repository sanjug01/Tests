using System;

namespace RdClient.Shared.Helpers
{
    /*
        When calling a method in quick succession, the debouncer makes sure that the method is called only once.

        Debouncer(Action action, ITimer timer, TimeSpan period)
        action - the action which should be invoked
        timer - a timer constructed by the client
        period - the minimum period between invocations of the action

        If Call() is invoked before period expires, the timer is reset for a new duration of period. 
        If Call() is invoked n times with less than 'period' time between invocations, only the last Call() actually invokes _action()        
    */
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
