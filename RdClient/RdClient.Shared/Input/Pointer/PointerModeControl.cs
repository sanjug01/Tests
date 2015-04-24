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

        public void LeftClick(Point position)
        {
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.LeftPress, _pointerPosition.SessionPosition));
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.LeftRelease, _pointerPosition.SessionPosition));
        }

        public void LeftDrag(PointerDragAction action, Point delta, Point position)
        {
            _pointerPosition.ViewportPosition = new Point(
                _pointerPosition.ViewportPosition.X + delta.X, 
                _pointerPosition.ViewportPosition.Y + delta.Y);

            DraggingHelper.Dragging(_sessionControl, action, DragButton.Left, _pointerPosition.SessionPosition);
        }

        public void Move(Point delta)
        {
            _pointerPosition.ViewportPosition = new Point(
                _pointerPosition.ViewportPosition.X + delta.X, 
                _pointerPosition.ViewportPosition.Y + delta.Y);
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.Move, _pointerPosition.SessionPosition));
        }

        public void RightClick(Point position)
        {
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.RightPress, _pointerPosition.SessionPosition));
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.RightRelease, _pointerPosition.SessionPosition));
        }

        public void RightDrag(PointerDragAction action, Point delta, Point position)
        {
            _pointerPosition.ViewportPosition = new Point(
                _pointerPosition.ViewportPosition.X + delta.X, 
                _pointerPosition.ViewportPosition.Y + delta.Y);

            DraggingHelper.Dragging(_sessionControl, action, DragButton.Right, _pointerPosition.SessionPosition);
        }

        public void ZoomPan(Point center, Point translation, double scale)
        {
            _sessionControl.RenderingPanel.Viewport.PanAndZoom(center, translation.X, translation.Y, scale, 0);
        }
    }
}
