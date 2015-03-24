using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer.PointerMode
{
    public class PointerModeControl : IPointerControl
    {
        private IPointerContext _context;
        private IPointerManipulator _manipulator;
        public IPointerManipulator Manipulator
        {
            get { return _manipulator; }
        }

        public PointerModeControl(IPointerContext context, IPointerManipulator manipulator)
        {
            _context = context;
            _manipulator = manipulator;
        }

        public virtual void MouseLeftClick(PointerEvent pointerEvent)
        {
            _manipulator.SendMouseAction(MouseEventType.LeftPress);
            _manipulator.SendMouseAction(MouseEventType.LeftRelease);
        }

        public void MouseMove(PointerEvent pointerEvent)
        {
            UpdateCursorPosition(pointerEvent);
            _manipulator.SendMouseAction(MouseEventType.Move);
        }

        public void MouseScroll(PointerEvent pointerEvent)
        {
            double delta = 0.0;

            if(_context.LastMoveOrientation == PointerMoveOrientation.Vertical)
            {
                delta = _context.LastMoveVector.Y;
                _manipulator.SendMouseWheel((int)delta * GlobalConstants.TouchScrollFactor, false);
            }
            else
            {
                delta = _context.LastMoveVector.X;
                _manipulator.SendMouseWheel((int)delta * GlobalConstants.TouchScrollFactor, true);
            }            
        }

        public virtual void MouseRightClick(PointerEvent pointerEvent)
        {
            _manipulator.SendMouseAction(MouseEventType.RightPress);
            _manipulator.SendMouseAction(MouseEventType.RightRelease);
        }

        public virtual void UpdateCursorPosition(PointerEvent pointerEvent)
        {
            _manipulator.MousePosition = new Point(_manipulator.MousePosition.X + _context.LastMoveVector.X, _manipulator.MousePosition.Y + _context.LastMoveVector.Y);
        }
    }
}
