using RdClient.Shared.Helpers;
using System.Collections.Generic;


namespace RdClient.Shared.Input.Mouse
{
    public class PointerEventConsumer : IPointerEventConsumer
    {
        private Dictionary<PointerType, IPointerEventConsumer> _pointerConsumers = new Dictionary<PointerType,IPointerEventConsumer>();
        private PointerType _lastPointerType = PointerType.Mouse;

        public PointerEventConsumer(ITimer timer, IPointerManipulator pointerManipulator)
        {
            _pointerConsumers[PointerType.Mouse] = new MouseMode(pointerManipulator);
            _pointerConsumers[PointerType.Pen] = new MouseMode(pointerManipulator);
            _pointerConsumers[PointerType.Touch] = new PointerMode(timer, pointerManipulator);
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
