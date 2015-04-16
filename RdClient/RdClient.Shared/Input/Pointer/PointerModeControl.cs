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
        private Point _mousePosition;

        public Point MousePosition
        {
            get { return _mousePosition; }
            private set
            {
                Point mP = new Point(
                    Math.Min(_sessionControl.RenderingPanel.Viewport.Size.Width, Math.Max(0, value.X)),
                    Math.Min(_sessionControl.RenderingPanel.Viewport.Size.Height, Math.Max(0, value.Y)));

                _mousePosition = mP;
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
            _sessionControl.SendMouseWheel((int)(delta * GlobalConstants.TouchScrollFactor), true);
        }

        public void Scroll(double delta)
        {
            _sessionControl.SendMouseWheel((int)(delta * GlobalConstants.TouchScrollFactor), false);
        }

        public virtual void LeftClick(IPointerRoutedEventProperties pointerEvent)
        {
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.LeftPress, MousePosition));
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.LeftRelease, MousePosition));
        }

        public virtual void LeftDrag(PointerDragAction action, Point delta)
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

        public virtual void Move(Point delta)
        {
            MousePosition = new Point(MousePosition.X + delta.X, MousePosition.Y + delta.Y);
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.Move, MousePosition));
        }

        public virtual void RightClick(IPointerRoutedEventProperties pointerEvent)
        {
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.RightPress, MousePosition));
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.RightRelease, MousePosition));
        }

        public virtual void RightDrag(PointerDragAction action, Point delta)
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

        public virtual void ZoomPan(Point center, Point translation, double scale)
        {
            _sessionControl.RenderingPanel.Viewport.PanAndZoom(center, translation.X, translation.Y, scale, 0);
        }
    }
}
