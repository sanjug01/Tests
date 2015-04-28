using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Pointer;

namespace RdClient.Shared.Models.PanKnobModel
{
    public interface IPanKnobSite
    {
        ITimerFactory TimerFactory { get; }

        void SetPanKnob(IPanKnob panKnob);
        void Consume(IPointerEventBase pointer);
    }
}
