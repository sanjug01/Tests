using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer
{
    public class DirectModeControl : IPointerControl
    {
        private IRemoteSessionControl _sessionControl;
        private IPointerPosition _pointerPosition;

        public DirectModeControl(IRemoteSessionControl sessionControl, IPointerPosition pointerPosition)
        {
            _sessionControl = sessionControl;
            _pointerPosition = pointerPosition;
        }

        void IPointerControl.HScroll(double delta)
        {
        }

        void IPointerControl.LeftClick(Point position)
        {
            _pointerPosition.PointerPosition = position;
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.LeftPress, _pointerPosition.PointerPosition));
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.LeftRelease, _pointerPosition.PointerPosition));
        }

        void IPointerControl.LeftDrag(PointerDragAction action, Point delta, Point position)
        {
            _pointerPosition.PointerPosition = new Point(
				_pointerPosition.PointerPosition.X + delta.X,
				_pointerPosition.PointerPosition.Y + delta.Y);

            DraggingHelper.Dragging(_sessionControl, action, DragButton.Left, _pointerPosition.PointerPosition);
        }

        void IPointerControl.Move(Point delta)
        {
        }

        void IPointerControl.RightClick(Point position)
        {
            _pointerPosition.PointerPosition = position;
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.RightPress, _pointerPosition.PointerPosition));
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.RightRelease, _pointerPosition.PointerPosition));
        }

        void IPointerControl.RightDrag(PointerDragAction action, Point delta, Point position)
        {
            _pointerPosition.PointerPosition = new Point(
                _pointerPosition.PointerPosition.X + delta.X,
                _pointerPosition.PointerPosition.Y + delta.Y);

            DraggingHelper.Dragging(_sessionControl, action, DragButton.Right, _pointerPosition.PointerPosition);
        }

        void IPointerControl.Scroll(double delta)
        {
        }

        void IPointerControl.ZoomPan(Point center, Point translation, double scale)
        {
        }
    }
}
