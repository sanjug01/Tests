using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation.Extensions;
using System;
using System.Collections.Generic;
using Windows.Devices.Input;

namespace RdClient.Shared.Input.Pointer
{
    public class PointerDeviceDispatcher : IPointerEventConsumer
    {
        private PointerDeviceType _lastPointerType;

        private Dictionary<PointerDeviceType, IPointerEventConsumer> _consumers = new Dictionary<PointerDeviceType, IPointerEventConsumer>();

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

        public PointerDeviceDispatcher(ITimerFactory timerFactory, IRemoteSessionControl sessionControl, IPointerPosition pointerPosition, IDeferredExecution dispatcher)
        {
            _pointerMode = new PointerModeConsumer(
                new RdDispatcherTimer(timerFactory.CreateTimer(), dispatcher), 
                new PointerModeControl(sessionControl, pointerPosition));
            _multiTouchMode = new MultiTouchConsumer(sessionControl, pointerPosition);
            _directMode = new DirectModeConsumer(new DirectModeControl(sessionControl, pointerPosition), pointerPosition);
            
            _consumers[PointerDeviceType.Mouse] = new MouseModeConsumer(sessionControl, pointerPosition);
            _consumers[PointerDeviceType.Pen] = new MouseModeConsumer(sessionControl, pointerPosition);
            _consumers[PointerDeviceType.Touch] = _pointerMode;

        }

        public void Consume(IPointerEventBase pointerEvent)
        {
            if(pointerEvent is IPointerRoutedEventProperties)
            {
                IPointerRoutedEventProperties prep = ((IPointerRoutedEventProperties)pointerEvent);

                if(prep.DeviceType != _lastPointerType)
                {
                    Reset();
                }

                _lastPointerType = prep.DeviceType;
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
