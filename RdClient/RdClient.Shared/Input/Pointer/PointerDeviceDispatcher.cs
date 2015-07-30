using System;
using System.Collections.Generic;
using Windows.Devices.Input;

namespace RdClient.Shared.Input.Pointer
{
    public class PointerDeviceDispatcher : IPointerEventConsumer
    {
        private PointerDeviceType _lastPointerType;

        private IDictionary<PointerDeviceType, IPointerEventConsumer> _consumers = new Dictionary<PointerDeviceType, IPointerEventConsumer>();

        private IPointerEventConsumer _pointerMode;
        private IPointerEventConsumer _directMode;
        private IPointerEventConsumer _multiTouchMode;

        private ConsumptionModeType _consumptionMode;

        public void SetConsumptionMode (ConsumptionModeType consumptionMode)
        {
            _consumptionMode = consumptionMode;
            switch(_consumptionMode)
            {
                case ConsumptionModeType.Pointer:
                    _consumers[PointerDeviceType.Touch] = _pointerMode;
                    break;
                case ConsumptionModeType.DirectTouch:
                    _consumers[PointerDeviceType.Touch] = _directMode;
                    break;
                case ConsumptionModeType.MultiTouch:
                    _consumers[PointerDeviceType.Touch] = _multiTouchMode;
                    break;
            }
        }

        public event EventHandler<IPointerEventBase> ConsumedEvent;

        public PointerDeviceDispatcher(PointerModeConsumer pointerModeConsumer, MultiTouchConsumer multiTouchConsumer, DirectModeConsumer directModeConsumer, MouseModeConsumer mouseModeConsumer)
        {
            _pointerMode = pointerModeConsumer;
            _multiTouchMode = multiTouchConsumer;
            _directMode = directModeConsumer;

            _consumers[PointerDeviceType.Mouse] = mouseModeConsumer;
            _consumers[PointerDeviceType.Pen] = mouseModeConsumer;
            _consumers[PointerDeviceType.Touch] = _pointerMode;

        }

        public void Consume(IPointerEventBase pointerEvent)
        {
            if(pointerEvent is IPointerRoutedEventProperties)
            {
                IPointerRoutedEventProperties prep = ((IPointerRoutedEventProperties)pointerEvent);

                if(_lastPointerType != prep.DeviceType)
                {
                    Reset();
                    _lastPointerType = prep.DeviceType;                    
                }
            }

            if(_consumers.ContainsKey(_lastPointerType))
            {
                _consumers[_lastPointerType].Consume(pointerEvent);
            }

            if(ConsumedEvent != null)
            {
                ConsumedEvent(this, pointerEvent);
            }
        }

        public void Reset()
        {
            if(_consumers.ContainsKey(_lastPointerType))
            {
                _consumers[_lastPointerType].Reset();
            }
        }
    }
}
