using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using System;
using System.Diagnostics;
using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer.PointerMode
{
    public class PointerModeControl : IPointerControl
    {
        private readonly IPointerContext _context;
        private readonly IPointerManipulator _manipulator;
        private readonly IRenderingPanel _panel;

        public IPointerManipulator Manipulator
        {
            get { return _manipulator; }
        }

        public PointerModeControl(IPointerContext context, IPointerManipulator manipulator, IRenderingPanel panel)
        {
            _context = context;
            _manipulator = manipulator;
            _panel = panel;
        }

        public virtual void MouseLeftClick(PointerEventOld pointerEvent)
        {
            _manipulator.SendMouseAction(MouseEventType.LeftPress);
            _manipulator.SendMouseAction(MouseEventType.LeftRelease);
        }

        public void MouseMove(PointerEventOld pointerEvent)
        {
            UpdateCursorPosition(pointerEvent);
            _manipulator.SendMouseAction(MouseEventType.Move);
        }

        public void MouseScroll(PointerEventOld pointerEvent)
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

        public virtual void MouseRightClick(PointerEventOld pointerEvent)
        {
            _manipulator.SendMouseAction(MouseEventType.RightPress);
            _manipulator.SendMouseAction(MouseEventType.RightRelease);
        }

        public virtual void UpdateCursorPosition(PointerEventOld pointerEvent)
        {
            if(pointerEvent.Inertia)
            {
                _manipulator.MousePosition = new Point(_manipulator.MousePosition.X + pointerEvent.Delta.X, _manipulator.MousePosition.Y + pointerEvent.Delta.Y);            
            }
            else 
            {
                _manipulator.MousePosition = new Point(_manipulator.MousePosition.X + _context.LastMoveVector.X, _manipulator.MousePosition.Y + _context.LastMoveVector.Y);            
            }
        }

        public void ZoomAndPan(PointerEventOld pointerEvent)
        {
            double diagonal = Math.Sqrt(Math.Pow(_panel.Viewport.Size.Width, 2) + Math.Pow(_panel.Viewport.Size.Height, 2));
            double scale = (100.0 * _context.LastSpreadDelta) / diagonal;

            if(scale > 0)
            {
                scale = 1 + scale / 8;
            }
            else
            {
                scale = 1 - Math.Abs(scale / 8);
            }

            _panel.Viewport.PanAndZoom(_context.LastSpreadCenter, _context.LastPanDelta.X * 5, _context.LastPanDelta.Y * 5, scale, 0.0);
        }
    }
}
