using RdClient.Shared.Helpers;
using System.Collections.Generic;


namespace RdClient.Shared.Input.Mouse
{
    public class PointerEventDispatcher : IPointerEventConsumer
    {
        private Dictionary<PointerType, IPointerEventConsumer> _pointerConsumers = new Dictionary<PointerType,IPointerEventConsumer>();
        private PointerType _lastPointerType = PointerType.Mouse;

        private IPointerEventConsumer _pointerMode;
        private IPointerEventConsumer _clickMode;

        private bool _clickModeEnabled = false;
        public bool ClickModeEnabled {
            get { return _clickModeEnabled; }
            set 
            { 
                _clickModeEnabled = value; 
                if(_clickModeEnabled)
                {
                    _pointerConsumers[PointerType.Touch] = _clickMode;
                }
                else
                {
                    _pointerConsumers[PointerType.Touch] = _pointerMode;
                }
            } 
        }

        public PointerEventDispatcher(ITimer timer, IPointerManipulator manipulator)
        {
            _pointerMode = TouchModeFactory.CreatePointerMode(timer, manipulator);
            _clickMode = TouchModeFactory.CreateClickMode(timer, manipulator);

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
