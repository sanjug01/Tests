using RdClient.Shared.CxWrappers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Input.Mouse
{
    public class MultiTouchMode : IPointerEventConsumer
    {
        private ConsumptionMode _consumptionMode;
        public ConsumptionMode ConsumptionMode
        {
            set { _consumptionMode = value; }
        }

        private Dictionary<uint, PointerEvent> _trackedTouches;
        private PointerEvent _masterTouch;
        private PointerEvent _lastTouch;
        private IPointerManipulator _manipulator;

        public MultiTouchMode(IPointerManipulator manipulator)
        {
            _manipulator = manipulator;
            _trackedTouches = new Dictionary<uint, PointerEvent>();
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

            _manipulator.SendTouchAction(pointerEvent.ActionType, pointerEvent.PointerId, pointerEvent.Position, delta);
        }

        public void ConsumeEvent(PointerEvent pointerEvent)
        {

            if(pointerEvent.ActionType == TouchEventType.Down)
            {
                if (_masterTouch == null)
                {
                    _masterTouch = pointerEvent;
                }

                _trackedTouches[pointerEvent.PointerId] = pointerEvent;
            }

            if(_masterTouch != null)
            {
                this.InvokeManipulator(pointerEvent);
            }

            if (pointerEvent.ActionType == TouchEventType.Up)
            {
                if (_trackedTouches.ContainsKey(pointerEvent.PointerId))
                {
                    _trackedTouches.Remove(pointerEvent.PointerId);
                }
            }
        }

        public void Reset()
        {
            _trackedTouches.Clear();
        }
    }
}
