using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private void MouseRecognizer(object parameter)
        {
            PointerEvent pointerEvent = parameter as PointerEvent;

            float x = (float)pointerEvent.Position.X;
            float y = (float)pointerEvent.Position.Y;
            PointerEventType buttonState = PointerEventType.Move;

            if (MouseLeftButton(0) == false && pointerEvent.LeftButton == true)
            {
                buttonState = PointerEventType.LeftPress;
            }
            else if (MouseLeftButton(0) == true && pointerEvent.LeftButton == false)
            {
                buttonState = PointerEventType.LeftRelease;
            }
            else if (MouseRightButton(0) == false && pointerEvent.RightButton == true)
            {
                buttonState = PointerEventType.RightPress;
            }
            else if (MouseRightButton(0) == true && pointerEvent.RightButton == false)
            {
                buttonState = PointerEventType.RightRelease;
            }

            _pointerManipulator.CursorPosition = new Point(x, y);
            _pointerManipulator.ChangeMousePointer(buttonState);
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
