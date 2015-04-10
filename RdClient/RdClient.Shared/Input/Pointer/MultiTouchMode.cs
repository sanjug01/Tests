using RdClient.Shared.CxWrappers;

namespace RdClient.Shared.Input.Pointer
{
    public class MultiTouchMode : IPointerEventConsumer
    {
        public event System.EventHandler<PointerEventOld> ConsumedEvent;

        private ConsumptionMode _consumptionMode;
        public ConsumptionMode ConsumptionMode
        {
            set { _consumptionMode = value; }
        }

        private PointerEventOld _masterTouch;
        private PointerEventOld _lastTouch;
        private IPointerManipulator _manipulator;

        public MultiTouchMode(IPointerManipulator manipulator)
        {
            _manipulator = manipulator;
        }

        private void InvokeManipulator(PointerEventOld pointerEvent)
        {
            // the time-stamp delta is relative to the first touch ever
            ulong delta = pointerEvent.TimeStamp - _masterTouch.TimeStamp;

            // don't send duplicate position updates
            if (pointerEvent.ActionType == TouchEventType.Update && _lastTouch != null && _lastTouch.Position == pointerEvent.Position)
            {
                return;
            }

            // if the position to the up event is not the same as the last recorded position, 
            // update the position before sending the up event
            if (pointerEvent.ActionType == TouchEventType.Up && _lastTouch != null && _lastTouch.Position != pointerEvent.Position)
            {
                _manipulator.SendTouchAction(TouchEventType.Update, pointerEvent.PointerId, pointerEvent.Position, delta);
            }

            // touch events have a location indicator hint which needs the correct position
            _manipulator.MousePosition = pointerEvent.Position;


            _manipulator.SendTouchAction(pointerEvent.ActionType, pointerEvent.PointerId, pointerEvent.Position, delta);
        }

        public void ConsumeEvent(PointerEventOld pointerEvent)
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

            if (ConsumedEvent != null)
            {
                ConsumedEvent(this, pointerEvent);
            }
        }

        public void Reset()
        {
            // don't reset the master touch because it may crash the protocol stack o_O
            _lastTouch = null;
        }
    }
}
