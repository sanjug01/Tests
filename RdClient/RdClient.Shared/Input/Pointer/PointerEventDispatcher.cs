using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Pointer.PointerMode;
using RdClient.Shared.Models;
using System.Collections.Generic;


namespace RdClient.Shared.Input.Pointer
{
    public class PointerEventDispatcher : IPointerEventConsumer
    {
        public event System.EventHandler<PointerEvent> ConsumedEvent;

        private Dictionary<PointerType, IPointerEventConsumer> _pointerConsumers = new Dictionary<PointerType,IPointerEventConsumer>();
        private PointerType _lastPointerType = PointerType.Mouse;

        private IPointerEventConsumer _pointerMode;
        private IPointerEventConsumer _directMode;
        private IPointerEventConsumer _multiTouchMode;

        private ConsumptionMode _consumptionMode = ConsumptionMode.Pointer;
        public ConsumptionMode ConsumptionMode {
            get { return _consumptionMode; }
            set 
            {
                _consumptionMode = value;

                switch(_consumptionMode)
                {
                    case ConsumptionMode.Pointer:
                        _pointerConsumers[PointerType.Touch] = _pointerMode;
                        break;
                    case ConsumptionMode.DirectTouch:
                        _pointerConsumers[PointerType.Touch] = _directMode;
                        break;
                    case ConsumptionMode.MultiTouch:
                        _pointerConsumers[PointerType.Touch] = _multiTouchMode;
                        break;
                    default:
                        break;
                }
            } 
        }

        public PointerEventDispatcher(ITimer timer, IPointerManipulator manipulator, IRenderingPanel panel)
        {
            _pointerMode = PointerModeFactory.CreatePointerMode(timer, manipulator, panel);
            _directMode = PointerModeFactory.CreateDirectMode(timer, manipulator, panel);
            _multiTouchMode = new MultiTouchMode(manipulator);

            _pointerConsumers[PointerType.Mouse] = new MouseMode(manipulator);
            _pointerConsumers[PointerType.Pen] = new MouseMode(manipulator);
            _pointerConsumers[PointerType.Touch] = _pointerMode;
        }

        public void ConsumeEvent(PointerEvent pointerEvent)
        {
            if (_lastPointerType != pointerEvent.PointerType)
            {
                _pointerConsumers[_lastPointerType].Reset();
            }

            _pointerConsumers[pointerEvent.PointerType].ConsumeEvent(pointerEvent);

            _lastPointerType = pointerEvent.PointerType;

            if (ConsumedEvent != null)
            {
                ConsumedEvent(this, pointerEvent);
            }
        }

        public void Reset()
        {
            _pointerConsumers[_lastPointerType].Reset();
        }
    }
}
