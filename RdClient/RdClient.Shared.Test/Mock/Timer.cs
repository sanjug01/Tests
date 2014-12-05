using RdClient.Shared.Helpers;
using RdMock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Test.Mock
{
    public class TimerFactory : MockBase, ITimerFactory
    {
        public Mock.Timer Timer { get; private set; }
        private int _createTimerCalls = 0;

        public int CreateTimerCalls
        {
            get
            {
                return _createTimerCalls;
            }
        }

        public ITimer CreateTimer(Action callback, TimeSpan period, bool recurring)
        {
            _createTimerCalls++;
            if (this.Timer != null)
            {
                this.Timer.Dispose();
            }
            this.Timer = new Mock.Timer() { Callback = callback, Period = period, Recurring = recurring };
            return this.Timer;
        }

        public override void Dispose()
        {
            if (this.Timer != null)
            {
                this.Timer.Dispose();
            }
            base.Dispose();
        }
    }

    public class Timer : MockBase, ITimer
    {
        public Action Callback { get; set; }
        public TimeSpan Period { get; set; }
        public bool Recurring { get; set; }

        public void Cancel()
        {
            Invoke(new object[] { });
        }
    }
}
