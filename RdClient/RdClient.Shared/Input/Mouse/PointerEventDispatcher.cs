using RdClient.Shared.Helpers;
using System.Collections.Generic;


namespace RdClient.Shared.Input.Mouse
{
    public class PointerEventDispatcher : IPointerEventConsumer
    {
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

        public PointerEventDispatcher(ITimer timer, IPointerManipulator manipulator)
        {
            _pointerMode = TouchModeFactory.CreatePointerMode(timer, manipulator);
            _directMode = TouchModeFactory.CreateDirectMode(timer, manipulator);
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
        }

        public void Reset()
        {
            _pointerConsumers[_lastPointerType].Reset();
        }
    }
}
