using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using System;

namespace RdClient.Shared.Input.Pointer
{
    public class MouseModeConsumer : IPointerEventConsumer
    {
        private ConsumptionMode _consumptionMode;
        ConsumptionMode IPointerEventConsumer.ConsumptionMode
        {
            set
            {
                _consumptionMode = value;
            }
        }

        public event EventHandler<IPointerEventBase> ConsumedEvent;

        private IPointerRoutedEventProperties _tracked;
        private IRemoteSessionControl _sessionControl;
        private IPointerPosition _pointerPosition;

        public MouseModeConsumer(IRemoteSessionControl sessionControl, IPointerPosition pointerPosition)
        {
            _sessionControl = sessionControl;
            _pointerPosition = pointerPosition;
        }

        private bool IsLeftMouseButtonPressed(IPointerRoutedEventProperties prep)
        {
            if(prep != null && prep.LeftButton)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsRightMouseButtonPressed(IPointerRoutedEventProperties prep)
        {
            if (prep != null && prep.RightButton)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void MouseRecognizer(IPointerRoutedEventProperties prep)
        {
            _pointerPosition.PointerPosition = prep.Position;

            if(IsLeftMouseButtonPressed(_tracked) == false && IsLeftMouseButtonPressed(prep) == true)
            {
                _sessionControl.SendMouseAction(new MouseAction(MouseEventType.LeftPress, prep.Position));
            }
            else if (IsLeftMouseButtonPressed(_tracked) == true && IsLeftMouseButtonPressed(prep) == false)
            {
                _sessionControl.SendMouseAction(new MouseAction(MouseEventType.LeftRelease, prep.Position));
            }
            else if (IsRightMouseButtonPressed(_tracked) == false && IsRightMouseButtonPressed(prep) == true)
            {
                _sessionControl.SendMouseAction(new MouseAction(MouseEventType.RightPress, prep.Position));
            }
            else if (IsRightMouseButtonPressed(_tracked) == true && IsRightMouseButtonPressed(prep) == false)
            {
                _sessionControl.SendMouseAction(new MouseAction(MouseEventType.RightRelease, prep.Position));
            }
            else if (Math.Abs(prep.MouseWheelDelta) > 0)
            {
                _sessionControl.SendMouseWheel(prep.MouseWheelDelta, prep.IsHorizontalWheel);
            }
            else
            {
                _sessionControl.SendMouseAction(new MouseAction(MouseEventType.Move, prep.Position));
            }
        }

        void IPointerEventConsumer.Consume(IPointerEventBase pointerEvent)
        {   
            if(pointerEvent is IPointerRoutedEventProperties)
            {
                MouseRecognizer((IPointerRoutedEventProperties) pointerEvent);
                _tracked = new PointerRoutedEventArgsCopy((IPointerRoutedEventProperties)pointerEvent);
            }
                     
            if(ConsumedEvent != null)
            {
                ConsumedEvent(this, pointerEvent);
            }
        }

        void IPointerEventConsumer.Reset()
        {
            _tracked = null;
        }
    }
}
