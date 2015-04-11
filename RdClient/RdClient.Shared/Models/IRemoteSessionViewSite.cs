using RdClient.Shared.Helpers;

namespace RdClient.Shared.Models
{
    public interface IRemoteSessionViewSite
    {
        void SetRemoteSessionView(IRemoteSessionView sessionView);
        ITimerFactory TimerFactory { get; }
    }
}
