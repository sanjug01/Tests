using RdClient.Shared.Helpers;
using System;

namespace RdClient.Shared.Input.Pointer
{

    public interface IPointerEventConsumer
    {
        event EventHandler<IPointerEventBase> ConsumedEvent;
        void Consume(IPointerEventBase pointerEvent);
        void Reset();
    }
}
