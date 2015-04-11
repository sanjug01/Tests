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

    public interface IPointerEventConsumerOld
    {
        event EventHandler<PointerEventOld> ConsumedEvent;
        ConsumptionMode ConsumptionMode { set; }
        void ConsumeEvent(PointerEventOld pointerEvent);
        void Reset();
    }

    public interface IPointerEventConsumer
    {
        event EventHandler<IPointerEventBase> ConsumedEvent;
        ConsumptionMode ConsumptionMode { set; }
        void ConsumeEvent(IPointerEventBase pointerEvent);
        void Reset();
    }
}
