using RdClient.Shared.CxWrappers;

namespace RdClient.Shared.Input.Mouse
{
    public class MultiTouchMode : IPointerEventConsumer
    {
        private ConsumptionMode _consumptionMode;
        public ConsumptionMode ConsumptionMode
        {
            set { _consumptionMode = value; }
        }

        private PointerEvent _masterTouch;
        private PointerEvent _lastTouch;
        private IPointerManipulator _manipulator;

        public MultiTouchMode(IPointerManipulator manipulator)
        {
            _manipulator = manipulator;
        }

        private void InvokeManipulator(PointerEvent pointerEvent)
        {
            ulong delta = pointerEvent.TimeStamp - _masterTouch.TimeStamp;

            if (pointerEvent.ActionType == TouchEventType.Update && _lastTouch != null && _lastTouch.Position == pointerEvent.Position)
            {
                return;
            }

            if (pointerEvent.ActionType == TouchEventType.Up && _lastTouch != null && _lastTouch.Position != pointerEvent.Position)
            {
                _manipulator.SendTouchAction(TouchEventType.Update, pointerEvent.PointerId, pointerEvent.Position, delta);
            }

            _manipulator.MousePosition = pointerEvent.Position;
            _manipulator.SendTouchAction(pointerEvent.ActionType, pointerEvent.PointerId, pointerEvent.Position, delta);
        }

        public void ConsumeEvent(PointerEvent pointerEvent)
        {
            if (_masterTouch == null && pointerEvent.ActionType == TouchEventType.Down)
            {
                _masterTouch = pointerEvent;
            }

            if(_masterTouch != null)
            {
                this.InvokeManipulator(pointerEvent);
            }

            _lastTouch = pointerEvent;
        }

        public void Reset()
        {
            // don't reset the master touch because it may crash the stack
            _lastTouch = null;
        }
    }
}
