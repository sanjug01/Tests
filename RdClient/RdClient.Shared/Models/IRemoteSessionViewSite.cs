using RdClient.Shared.Helpers;
using RdClient.Shared.Models.PanKnobModel;

namespace RdClient.Shared.Models
{
    public interface IRemoteSessionViewSite
    {
        void SetRemoteSessionView(IRemoteSessionView sessionView);
        ITimerFactory TimerFactory { get; }
        ITimerFactory DispatcherTimerFactory { get; }

        IPanKnobSite PanKnobSite { get; }
    }
}
