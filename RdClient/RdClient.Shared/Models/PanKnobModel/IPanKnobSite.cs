using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Pointer;

namespace RdClient.Shared.Models.PanKnobModel
{
    public interface IPanKnobSite : IPointerEventConsumer
    {
        ITimerFactory TimerFactory { get; }
        IViewport Viewport { set; }

        void SetPanKnob(IPanKnob panKnob);

        void OnConsumptionModeChanged(object sender, ConsumptionModeType consumptionMode);
    }
}
