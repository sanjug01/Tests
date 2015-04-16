using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using System;

namespace RdClient.Shared.Input.Pointer
{
    public class MouseModeControl : IPointerEventConsumer
    {
        private ConsumptionMode _consumptionMode;
        public ConsumptionMode ConsumptionMode
        {
            set
            {
                _consumptionMode = value;
            }
        }

        public event EventHandler<IPointerEventBase> ConsumedEvent;

        private IPointerRoutedEventProperties _tracked;
        private IRemoteSessionControl _sessionControl;

        public MouseModeControl(IRemoteSessionControl sessionControl)
        {
            _sessionControl = sessionControl;
        }

        private bool MouseLeftButton(IPointerRoutedEventProperties prep)
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

        private bool MouseRightButton(IPointerRoutedEventProperties prep)
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
            _sessionControl.RenderingPanel.MoveMouseCursor(prep.Position);

            if(MouseLeftButton(_tracked) == false && MouseLeftButton(prep) == true)
            {
                _sessionControl.SendMouseAction(new MouseAction(MouseEventType.LeftPress, prep.Position));
            }
            else if (MouseLeftButton(_tracked) == true && MouseLeftButton(prep) == false)
            {
                _sessionControl.SendMouseAction(new MouseAction(MouseEventType.LeftRelease, prep.Position));
            }
            else if (MouseRightButton(_tracked) == false && MouseRightButton(prep) == true)
            {
                _sessionControl.SendMouseAction(new MouseAction(MouseEventType.RightPress, prep.Position));
            }
            else if (MouseRightButton(_tracked) == true && MouseRightButton(prep) == false)
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

        public void Consume(IPointerEventBase pointerEvent)
        {   
            if(pointerEvent is IPointerRoutedEventProperties)
            {
                MouseRecognizer((IPointerRoutedEventProperties) pointerEvent);
                _tracked = new PointerRoutedEventArgsCopy(pointerEvent as IPointerRoutedEventProperties);
            }
                     
            if(ConsumedEvent != null)
            {
                ConsumedEvent(this, pointerEvent);
            }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
