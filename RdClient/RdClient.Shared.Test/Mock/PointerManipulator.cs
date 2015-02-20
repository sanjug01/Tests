using RdClient.Shared.CxWrappers;
using RdClient.Shared.Input.Mouse;
using RdMock;
using System;
using Windows.Foundation;

namespace RdClient.Shared.Test.Mock
{
    public class PointerManipulator : MockBase, IPointerManipulator
    {
        public double MouseAcceleration { get; set; }

        public Point MousePosition { get; set; }

        public void SendMouseAction(MouseEventType eventType)
        {
            Invoke(new object[] { eventType });
        }

        public void SendMouseWheel(int delta, bool isHorizontal)
        {
            Invoke(new object[] { delta, isHorizontal });
        }

        public void SendTouchAction(TouchEventType type, uint contactId, Point position, ulong frameTime)
        {
            Invoke(new object[] { type, contactId, position, frameTime });
        }
    }
}
