using RdClient.Shared.CxWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer
{
    public class MouseAction
    {
        public MouseEventType MouseEventType { get; private set; }
        public Point Point { get; private set; }

        public MouseAction(MouseEventType e, Point p)
        {
            MouseEventType = e;
            Point = p;
        }
    }
}
