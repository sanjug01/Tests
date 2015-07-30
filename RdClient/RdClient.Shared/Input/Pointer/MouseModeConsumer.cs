using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using System;
using System.Diagnostics;

namespace RdClient.Shared.Input.Pointer
{
    public class MouseModeConsumer : IPointerEventConsumer
    {
        public event EventHandler<IPointerEventBase> ConsumedEvent;

        private bool _rightMouseButtonWasPressed;
        private bool _leftMouseButtonWasPressed;

        private IPointerRoutedEventProperties _tracked;
        private IRemoteSessionControl _sessionControl;
        private IPointerPosition _pointerPosition;

        public MouseModeConsumer(IRemoteSessionControl sessionControl, IPointerPosition pointerPosition)
        {
            _sessionControl = sessionControl;
            _pointerPosition = pointerPosition;
            _rightMouseButtonWasPressed = false;
            _leftMouseButtonWasPressed = false;
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
            _pointerPosition.ViewportPosition = prep.Position;
            int mouseWheelDelta = prep.MouseWheelDelta;

            if(_leftMouseButtonWasPressed == false && prep.LeftButton == true)
            {
                _sessionControl.SendMouseAction(new MouseAction(MouseEventType.LeftPress, _pointerPosition.SessionPosition));
            }
            else if (_leftMouseButtonWasPressed == true && prep.LeftButton == false)
            {
                _sessionControl.SendMouseAction(new MouseAction(MouseEventType.LeftRelease, _pointerPosition.SessionPosition));
            }
            else if (_rightMouseButtonWasPressed == false && prep.RightButton == true)
            {
                _sessionControl.SendMouseAction(new MouseAction(MouseEventType.RightPress, _pointerPosition.SessionPosition));
            }
            else if (_rightMouseButtonWasPressed == true && prep.RightButton == false)
            {
                _sessionControl.SendMouseAction(new MouseAction(MouseEventType.RightRelease, _pointerPosition.SessionPosition));
            }
            else if (mouseWheelDelta < 0 || mouseWheelDelta >  0)
            {
                _sessionControl.SendMouseWheel(mouseWheelDelta, prep.IsHorizontalWheel);
            }
            else
            {
                _sessionControl.SendMouseAction(new MouseAction(MouseEventType.Move, _pointerPosition.SessionPosition));
            }
        }

        void IPointerEventConsumer.Consume(IPointerEventBase pointerEvent)
        {   
            if(pointerEvent is IPointerRoutedEventProperties)
            {
                IPointerRoutedEventProperties prep = (IPointerRoutedEventProperties)pointerEvent;
                MouseRecognizer(prep);
                _leftMouseButtonWasPressed = (prep).LeftButton;
                _rightMouseButtonWasPressed = (prep).RightButton;
            }
                     
            if(ConsumedEvent != null)
            {
                ConsumedEvent(this, pointerEvent);
            }
        }

        void IPointerEventConsumer.Reset()
        {
            _rightMouseButtonWasPressed = false;
            _leftMouseButtonWasPressed = false;
        }
    }
}
