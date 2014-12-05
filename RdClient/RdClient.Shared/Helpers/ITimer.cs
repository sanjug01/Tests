using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace RdClient.Shared.Helpers
{
    public interface ITimerFactory
    {
        ITimer CreateTimer(Action callback, TimeSpan period, bool recurring);
    }

    public interface ITimer
    {
        void Cancel();
    }

    public interface ITimer2
    {
        void Start(Action<ITimer2> callback, TimeSpan period, bool repeating = true);
        void Stop();
        TimeSpan Period { get; }
        bool Running { get; }
    }
}
