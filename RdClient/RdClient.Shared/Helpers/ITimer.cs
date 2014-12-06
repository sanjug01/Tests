using System;

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
}
