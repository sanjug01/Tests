using RdClient.Shared.Helpers;
using RdClient.Shared.Models.PanKnobModel;
using RdClient.Shared.Navigation.Extensions;

namespace RdClient.Shared.Models
{
    public interface IRemoteSessionViewSite
    {
        void SetRemoteSessionView(IRemoteSessionView sessionView);
        ITimerFactory TimerFactory { get; }
        IDeferredExecution Dispatcher { get; }

        IPanKnobSite PanKnobSite { get; }
    }
}
