using RdClient.Shared.Helpers;
using RdMock;
using System;

namespace RdClient.Shared.Test.Mock
{
    public class TimerFactory : MockBase, ITimerFactory
    {
        public ITimer CreateTimer()
        {
            return (ITimer) Invoke(new object[] { });
        }
    }

    public class Timer : ITimer
    {
        public Action Callback { get; private set; }
        public TimeSpan Period { get; private set; }
        public bool Recurring { get; private set; }
        public bool Running { get; private set; }
        public int CallsToStart { get; private set; }

        public Timer()
        {
            this.Running = false;
            this.CallsToStart = 0;
        }


        public void Start(Action callback, TimeSpan period, bool recurring)
        {
            this.Callback = callback;
            this.Period = period;
            this.Recurring = recurring;
            this.Running = true;
            this.CallsToStart++;
        }

        public void Stop()
        {
            this.Running = false;
        }
    }
}
