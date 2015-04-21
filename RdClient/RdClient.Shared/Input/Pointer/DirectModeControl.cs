using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void HScroll(double delta)
        {
            throw new NotImplementedException();
        }

        public void LeftClick(Point position)
        {
            _pointerPosition.PointerPosition = position;
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.LeftPress, _pointerPosition.PointerPosition));
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.LeftRelease, _pointerPosition.PointerPosition));
        }

        public void LeftDrag(PointerDragAction action, Point delta, Point position)
        {
            _pointerPosition.PointerPosition = new Point(
				_pointerPosition.PointerPosition.X + delta.X,
				_pointerPosition.PointerPosition.Y + delta.Y);

            DraggingHelper.Dragging(_sessionControl, action, DragButton.Left, _pointerPosition.PointerPosition);
        }

        public void Move(Point delta)
        {
            throw new NotImplementedException();
        }

        public void RightClick(Point position)
        {
            _pointerPosition.PointerPosition = position;
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.RightPress, _pointerPosition.PointerPosition));
            _sessionControl.SendMouseAction(new MouseAction(MouseEventType.RightRelease, _pointerPosition.PointerPosition));
        }

        public void RightDrag(PointerDragAction action, Point delta, Point position)
        {
            _pointerPosition.PointerPosition = new Point(
                _pointerPosition.PointerPosition.X + delta.X,
                _pointerPosition.PointerPosition.Y + delta.Y);

            DraggingHelper.Dragging(_sessionControl, action, DragButton.Right, _pointerPosition.PointerPosition);
        }

        public void Scroll(double delta)
        {
            throw new NotImplementedException();
        }

        public void ZoomPan(Point center, Point translation, double scale)
        {
            throw new NotImplementedException();
        }
    }
}
