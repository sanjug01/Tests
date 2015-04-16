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
        public Point MousePosition
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void HScroll(double delta)
        {
            throw new NotImplementedException();
        }

        public void LeftClick(IPointerRoutedEventProperties pointerEvent)
        {
            throw new NotImplementedException();
        }

        public void LeftDrag(PointerDragAction action, Point delta)
        {
            throw new NotImplementedException();
        }

        public void Move(Point delta)
        {
            throw new NotImplementedException();
        }

        public void RightClick(IPointerRoutedEventProperties pointerEvent)
        {
            throw new NotImplementedException();
        }

        public void RightDrag(PointerDragAction action, Point delta)
        {
            throw new NotImplementedException();
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
