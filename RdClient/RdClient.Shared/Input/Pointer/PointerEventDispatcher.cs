using RdClient.Shared.Helpers;
using System;

namespace RdClient.Shared.Input.Pointer
{
    public class PointerEventDispatcher : IPointerEventConsumer
    {
        private PointerDeviceDispatcher _deviceDispatcher;
        private PointerVisibilityConsumer _visibilityConsumer;

        public void SetConsumptionMode(ConsumptionModeType consumptionMode)
        {
            _deviceDispatcher.SetConsumptionMode(consumptionMode);
            _visibilityConsumer.SetConsumptionMode(consumptionMode);
        }

        public event EventHandler<IPointerEventBase> ConsumedEvent;

        public PointerEventDispatcher(ITimerFactory timerFactory, PointerDeviceDispatcher deviceDispatcher, PointerVisibilityConsumer pointerVisibilityConsumer)
        {
            _deviceDispatcher = deviceDispatcher;
            _visibilityConsumer = pointerVisibilityConsumer;
        }

        public void Consume(IPointerEventBase pointerEvent)
        {   
            _visibilityConsumer.Consume(pointerEvent);
            _deviceDispatcher.Consume(pointerEvent);
             
            if (ConsumedEvent != null)
            {
                ConsumedEvent(this, pointerEvent);
            }
        }

        public void Reset()
        {
            _deviceDispatcher.Reset();
        }
    }
}
