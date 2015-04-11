using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Pointer.PointerMode;
using RdClient.Shared.Models;
using System.Collections.Generic;


namespace RdClient.Shared.Input.Pointer
{
    public class PointerEventDispatcher : IPointerEventConsumerOld
    {
        public event System.EventHandler<PointerEventOld> ConsumedEvent;

        private Dictionary<PointerTypeOld, IPointerEventConsumerOld> _pointerConsumers = new Dictionary<PointerTypeOld,IPointerEventConsumerOld>();
        private PointerTypeOld _lastPointerType = PointerTypeOld.Mouse;

        private IPointerEventConsumerOld _pointerMode;
        private IPointerEventConsumerOld _directMode;
        private IPointerEventConsumerOld _multiTouchMode;

        private ConsumptionMode _consumptionMode = ConsumptionMode.Pointer;
        public ConsumptionMode ConsumptionMode {
            get { return _consumptionMode; }
            set 
            {
                _consumptionMode = value;

                switch(_consumptionMode)
                {
                    case ConsumptionMode.Pointer:
                        _pointerConsumers[PointerTypeOld.Touch] = _pointerMode;
                        break;
                    case ConsumptionMode.DirectTouch:
                        _pointerConsumers[PointerTypeOld.Touch] = _directMode;
                        break;
                    case ConsumptionMode.MultiTouch:
                        _pointerConsumers[PointerTypeOld.Touch] = _multiTouchMode;
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

            _pointerConsumers[PointerTypeOld.Mouse] = new MouseMode(manipulator);
            _pointerConsumers[PointerTypeOld.Pen] = new MouseMode(manipulator);
            _pointerConsumers[PointerTypeOld.Touch] = _pointerMode;
        }

        public void ConsumeEvent(PointerEventOld pointerEvent)
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
