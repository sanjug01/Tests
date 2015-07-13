using System;

namespace RdClient.Shared.Helpers
{
    public interface ITimerFactory
    {
        ITimer CreateTimer();
    }

    public interface ITimer
    {
        void Start(Action callback, TimeSpan period, bool recurring);
        void Stop();
    }
}
