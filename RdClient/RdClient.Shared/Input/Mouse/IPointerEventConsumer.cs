using RdClient.Shared.Helpers;

namespace RdClient.Shared.Input.Mouse
{
    public enum ConsumptionMode
    {
        Pointer,
        DirectTouch,
        MultiTouch
    }

    public interface IPointerEventConsumer
    {
        ConsumptionMode ConsumptionMode { set; }
        void ConsumeEvent(PointerEvent pointerEvent);
        void Reset();
    }
}
