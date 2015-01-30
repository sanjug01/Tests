using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using System;
using Windows.Foundation;

namespace RdClient.Shared.Input.Mouse
{
    public class MouseMode : IPointerEventConsumer
    {
        public event System.EventHandler<PointerEvent> ConsumedEvent;

        private PointerEvent _trackedPointerEvent;
        private IPointerManipulator _pointerManipulator;

        public ConsumptionMode ConsumptionMode
        {
            set { }
        }

        public MouseMode(IPointerManipulator pointerManipulator)
        {
            _pointerManipulator = pointerManipulator;
        }

        private bool MouseLeftButton(uint id)
        {
            if(_trackedPointerEvent != null)
            {
                return _trackedPointerEvent.LeftButton;
            }
            else
            {
                return false;
            }
        }

        private bool MouseRightButton(uint id)
        {
            if (_trackedPointerEvent != null)
            {
                return _trackedPointerEvent.RightButton;
            }
            else
            {
                return false;
            }
        }

        private void MouseRecognizer(PointerEvent pointerEvent)
        {
            float x = (float)pointerEvent.Position.X;
            float y = (float)pointerEvent.Position.Y;

            _pointerManipulator.MousePosition = new Point(x, y);

            if (MouseLeftButton(0) == false && pointerEvent.LeftButton == true)
            {
                _pointerManipulator.SendMouseAction(MouseEventType.LeftPress);
            }
            else if (MouseLeftButton(0) == true && pointerEvent.LeftButton == false)
            {
                _pointerManipulator.SendMouseAction(MouseEventType.LeftRelease);
            }
            else if (MouseRightButton(0) == false && pointerEvent.RightButton == true)
            {
                _pointerManipulator.SendMouseAction(MouseEventType.RightPress);
            }
            else if (MouseRightButton(0) == true && pointerEvent.RightButton == false)
            {
                _pointerManipulator.SendMouseAction(MouseEventType.RightRelease);
            }
            else if(Math.Abs(pointerEvent.MouseWheelDelta) > 0)
            {
                _pointerManipulator.SendMouseWheel(pointerEvent.MouseWheelDelta, pointerEvent.IsHorizontalMouseWheel);
            }
            else
            {
                _pointerManipulator.SendMouseAction(MouseEventType.Move);
            }
        }

        public void ConsumeEvent(PointerEvent pointerEvent)
        {
            MouseRecognizer(pointerEvent);
            _trackedPointerEvent = pointerEvent;
            if (ConsumedEvent != null)
            {
                ConsumedEvent(this, pointerEvent);
            }
        }

        public void Reset()
        {
            _trackedPointerEvent = null;
        }
    }
}
