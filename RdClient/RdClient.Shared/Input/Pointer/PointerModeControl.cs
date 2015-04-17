using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using System;
using System.Diagnostics;
using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer
{
    public class PointerModeControl : IPointerControl
    {
        private IRemoteSessionControl _sessionControl;
        private IPointerPosition _pointerPosition;

        public PointerModeControl(IRemoteSessionControl sessionControl, IPointerPosition pointerPosition)
        {
            _sessionControl = sessionControl;
            _pointerPosition = pointerPosition;
        }

        public void HScroll(double delta)
        {
            _sessionControl.SendMouseWheel((int)(delta * GlobalConstants.TouchScrollFactor), true);
        }

        public void Scroll(double delta)
        {
            _sessionControl.SendMouseWheel((int)(delta * GlobalConstants.TouchScrollFactor), false);
        }

        public virtual void LeftClick(IPointerRoutedEventProperties pointerEvent)
        {
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.LeftPress, _pointerPosition.PointerPosition));
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.LeftRelease, _pointerPosition.PointerPosition));
        }

        public virtual void LeftDrag(PointerDragAction action, Point delta)
        {
            _pointerPosition.PointerPosition = new Point(
                _pointerPosition.PointerPosition.X + delta.X, 
                _pointerPosition.PointerPosition.Y + delta.Y);

            switch (action)
            {
                case PointerDragAction.Begin:
                    _sessionControl.SendMouseAction(new MouseAction(MouseEventType.LeftPress, _pointerPosition.PointerPosition));
                    break;
                case PointerDragAction.Update:
                    _sessionControl.SendMouseAction(new MouseAction(MouseEventType.Move, _pointerPosition.PointerPosition));
                    break;
                case PointerDragAction.End:
                    _sessionControl.SendMouseAction(new MouseAction(MouseEventType.LeftRelease, _pointerPosition.PointerPosition));
                    break;
                default:
                    break;
            }
        }

        public virtual void Move(Point delta)
        {
            _pointerPosition.PointerPosition = new Point(
                _pointerPosition.PointerPosition.X + delta.X, 
                _pointerPosition.PointerPosition.Y + delta.Y);
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.Move, _pointerPosition.PointerPosition));
        }

        public virtual void RightClick(IPointerRoutedEventProperties pointerEvent)
        {
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.RightPress, _pointerPosition.PointerPosition));
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.RightRelease, _pointerPosition.PointerPosition));
        }

        public virtual void RightDrag(PointerDragAction action, Point delta)
        {
            _pointerPosition.PointerPosition = new Point(
                _pointerPosition.PointerPosition.X + delta.X, 
                _pointerPosition.PointerPosition.Y + delta.Y);

            switch (action)
            {
                case PointerDragAction.Begin:
                    _sessionControl.SendMouseAction(new MouseAction(MouseEventType.RightPress, _pointerPosition.PointerPosition));
                    break;
                case PointerDragAction.Update:
                    _sessionControl.SendMouseAction(new MouseAction(MouseEventType.Move, _pointerPosition.PointerPosition));
                    break;
                case PointerDragAction.End:
                    _sessionControl.SendMouseAction(new MouseAction(MouseEventType.RightRelease, _pointerPosition.PointerPosition));
                    break;
                default:
                    break;
            }
        }

        public virtual void ZoomPan(Point center, Point translation, double scale)
        {
            _sessionControl.RenderingPanel.Viewport.PanAndZoom(center, translation.X, translation.Y, scale, 0);
        }
    }
}
