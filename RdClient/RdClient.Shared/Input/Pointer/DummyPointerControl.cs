﻿using System.Diagnostics;
using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer
{
    public class DummyPointerControl : IPointerControl
    {
        public void HScroll(double delta)
        {
            //Debug.WriteLine("HSCroll");
        }

        public void LeftClick(IPointerRoutedEventProperties pointerEvent)
        {
            //Debug.WriteLine("LeftClick");
        }

        public void LeftDrag(PointerDragAction action, Point delta)
        {
            //Debug.WriteLine("LeftDrag");
        }

        public void Move(Point delta)
        {
            //Debug.WriteLine("Move");
        }

        public void RightClick(IPointerRoutedEventProperties pointerEvent)
        {
            //Debug.WriteLine("RightClick");
        }

        public void RightDrag(PointerDragAction action, Point delta)
        {
            //Debug.WriteLine("RightDrag");
        }

        public void Scroll(double delta)
        {
            //Debug.WriteLine("Scroll");
        }

        public void ZoomPan(Point center, Point translation, double scale)
        {
            //Debug.WriteLine("ZoomPan");
        }
    }
}