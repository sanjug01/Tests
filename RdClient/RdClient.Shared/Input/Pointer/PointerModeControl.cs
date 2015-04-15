using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using System;
using System.Diagnostics;
using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer
{
    public class PointerModeControl : IPointerControl
    {
        private IRemoteSessionControl _sessionControl;
        private Point _mousePosition;

        public Point MousePosition
        {
            get { return _mousePosition; }
            private set
            {
                _mousePosition = value;
                _sessionControl.RenderingPanel.MoveMouseCursor(_mousePosition);
            }
        }


        public PointerModeControl(IRemoteSessionControl sessionControl)
        {
            _sessionControl = sessionControl;
            _mousePosition = new Point(0, 0);
        }

        public void HScroll(double delta)
        {
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.MouseHWheel, new Point(delta, 0)));
        }

        public void Scroll(double delta)
        {
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.MouseWheel, new Point(0, delta)));
        }

        public void LeftClick(IPointerRoutedEventProperties pointerEvent)
        {
            //MousePosition = pointerEvent.Position;
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.LeftPress, MousePosition));
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.LeftRelease, MousePosition));
        }

        public void LeftDrag(PointerDragAction action, Point delta)
        {
            MousePosition = new Point(MousePosition.X + delta.X, MousePosition.Y + delta.Y);

            switch (action)
            {
                case PointerDragAction.Begin:
                    _sessionControl.SendMouseAction(new MouseAction(MouseEventType.LeftPress, MousePosition));
                    break;
                case PointerDragAction.Update:
                    _sessionControl.SendMouseAction(new MouseAction(MouseEventType.Move, MousePosition));
                    break;
                case PointerDragAction.End:
                    _sessionControl.SendMouseAction(new MouseAction(MouseEventType.LeftRelease, MousePosition));
                    break;
                default:
                    break;
            }
        }

        public void Move(Point delta)
        {
            MousePosition = new Point(MousePosition.X + delta.X, MousePosition.Y + delta.Y);
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.Move, MousePosition));
        }

        public void RightClick(IPointerRoutedEventProperties pointerEvent)
        {
            //MousePosition = pointerEvent.Position;
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.RightPress, MousePosition));
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.RightRelease, MousePosition));
        }

        public void RightDrag(PointerDragAction action, Point delta)
        {
            MousePosition = new Point(MousePosition.X + delta.X, MousePosition.Y + delta.Y);

            switch (action)
            {
                case PointerDragAction.Begin:
                    _sessionControl.SendMouseAction(new MouseAction(MouseEventType.RightPress, MousePosition));
                    break;
                case PointerDragAction.Update:
                    _sessionControl.SendMouseAction(new MouseAction(MouseEventType.Move, MousePosition));
                    break;
                case PointerDragAction.End:
                    _sessionControl.SendMouseAction(new MouseAction(MouseEventType.RightRelease, MousePosition));
                    break;
                default:
                    break;
            }
        }

        public void ZoomPan(Point center, Point translation, double scale)
        {
            _sessionControl.RenderingPanel.Viewport.PanAndZoom(center, translation.X, translation.Y, scale, 0);
        }
    }
}
