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
        event EventHandler<IPointerEventBase> ConsumedEvent;
        ConsumptionMode ConsumptionMode { set; }
        void Consume(IPointerEventBase pointerEvent);
        void Reset();
    }
}
