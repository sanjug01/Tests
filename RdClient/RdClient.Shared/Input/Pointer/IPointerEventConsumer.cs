using RdClient.Shared.Helpers;
using System;

namespace RdClient.Shared.Input.Pointer
{
    public enum ConsumptionMode
    {
        Pointer,
        DirectTouch,
        MultiTouch
    }

    public interface IPointerEventConsumer
    {
        event EventHandler<PointerEvent> ConsumedEvent;
        ConsumptionMode ConsumptionMode { set; }
        void ConsumeEvent(PointerEvent pointerEvent);
        void Reset();
    }
}
