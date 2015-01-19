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

        private bool _directModeEnabled = false;
        public bool DirectModeEnabled {
            get { return _directModeEnabled; }
            set 
            { 
                _directModeEnabled = value; 
                if(_directModeEnabled)
                {
                    _pointerConsumers[PointerType.Touch] = _directMode;
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
            _directMode = TouchModeFactory.CreateDirectMode(timer, manipulator);

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
