using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;

namespace RdClient.Shared.Input.Pointer
{
    public class PointerEventDispatcher : IPointerEventConsumer
    {
        private PointerDeviceType _lastPointerType;

        private Dictionary<PointerDeviceType, IPointerEventConsumer> _consumers = new Dictionary<PointerDeviceType, IPointerEventConsumer>();

        private IPointerEventConsumer _pointerMode;
        private IPointerEventConsumer _directMode;
        private IPointerEventConsumer _multiTouchMode;

        private ConsumptionMode _consumptionMode;
        public ConsumptionMode ConsumptionMode
        {
            set
            {
                _consumptionMode = value;

                switch(_consumptionMode)
                {
                    case ConsumptionMode.Pointer:
                        _consumers[PointerDeviceType.Touch] = _pointerMode;
                        break;
                    case ConsumptionMode.DirectTouch:
                        _consumers[PointerDeviceType.Touch] = _directMode;
                        break;
                    case ConsumptionMode.MultiTouch:
                        _consumers[PointerDeviceType.Touch] = _multiTouchMode;
                        break;
                }
            }
        }

        public event EventHandler<IPointerEventBase> ConsumedEvent;

        public PointerEventDispatcher(ITimerFactory timerFactory, IRemoteSessionControl sessionControl)
        {
            _pointerMode = new PointerModeConsumer(timerFactory.CreateTimer(), new PointerModeControl(sessionControl));
            _multiTouchMode = new MultiTouchConsumer(sessionControl);
            //_directMode = PointerModeFactory.CreateDirectMode(timer, manipulator, panel);

            _consumers[PointerDeviceType.Mouse] = new MouseModeConsumer(sessionControl);
            _consumers[PointerDeviceType.Mouse] = new MouseModeConsumer(sessionControl);
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
