using RdClient.Shared.Helpers;
using System.Collections.Generic;


namespace RdClient.Shared.Input.Mouse
{
    public class PointerEventConsumer : IPointerEventConsumer
    {
        private bool _multiTouch = true;
        public bool MultiTouch
        {
            get
            {
                return _multiTouch;
            }
            set
            {
                Reset();
                _multiTouch = value;
            }
        }

        private Dictionary<PointerType, IPointerEventConsumer> _pointerConsumers = new Dictionary<PointerType,IPointerEventConsumer>();
        private PointerType _lastPointerType = PointerType.Mouse;

        public PointerEventConsumer(ITimer timer, IPointerManipulator manipulator)
        {
            _pointerConsumers[PointerType.Mouse] = new MouseMode(manipulator);
            _pointerConsumers[PointerType.Pen] = new MouseMode(manipulator);
            _pointerConsumers[PointerType.Touch] = TouchModeFactory.CreatePointerMode(timer, manipulator);
            _pointerConsumers[PointerType.Click] = TouchModeFactory.CreateClickMode(timer, manipulator);
        }

        public void ConsumeEvent(PointerEvent pointerEvent)
        {
            if (_lastPointerType != pointerEvent.PointerType)
            {
                _pointerConsumers[_lastPointerType].Reset();
            }

            if(_multiTouch == false && pointerEvent.PointerType == PointerType.Touch)
            {
                pointerEvent.PointerType = PointerType.Click;
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
