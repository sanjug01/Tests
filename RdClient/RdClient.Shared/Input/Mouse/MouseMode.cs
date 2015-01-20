using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using Windows.Foundation;

namespace RdClient.Shared.Input.Mouse
{
    public class MouseMode : IPointerEventConsumer
    {
        private PointerEvent _trackedPointerEvent;
        private IPointerManipulator _pointerManipulator;
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
            MouseEventType buttonState = MouseEventType.Move;

            if (MouseLeftButton(0) == false && pointerEvent.LeftButton == true)
            {
                buttonState = MouseEventType.LeftPress;
            }
            else if (MouseLeftButton(0) == true && pointerEvent.LeftButton == false)
            {
                buttonState = MouseEventType.LeftRelease;
            }
            else if (MouseRightButton(0) == false && pointerEvent.RightButton == true)
            {
                buttonState = MouseEventType.RightPress;
            }
            else if (MouseRightButton(0) == true && pointerEvent.RightButton == false)
            {
                buttonState = MouseEventType.RightRelease;
            }

            _pointerManipulator.MousePosition = new Point(x, y);
            _pointerManipulator.SendMouseAction(buttonState);
        }

        public void ConsumeEvent(PointerEvent pointerEvent)
        {
            MouseRecognizer(pointerEvent);
            _trackedPointerEvent = pointerEvent;
        }

        public void Reset()
        {
            _trackedPointerEvent = null;
        }
    }
}
