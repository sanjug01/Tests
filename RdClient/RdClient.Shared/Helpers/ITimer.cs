using System;

namespace RdClient.Shared.Helpers
{
    public interface ITimerFactory
    {
        ITimer CreateTimer();
        ITimer CreateDispatcherTimer();
    }

    public interface ITimer
    {
        void Start(Action callback, TimeSpan period, bool recurring);

        void Stop();
    }
}
