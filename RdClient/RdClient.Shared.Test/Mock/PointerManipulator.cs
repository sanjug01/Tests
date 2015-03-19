using RdClient.Shared.CxWrappers;
using RdClient.Shared.Input.Pointer;
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

        public void SendPinchAndZoom(double centerX, double centerY, double fromLength, double toLength)
        {
            Invoke(new object[] { centerX, centerY, fromLength, toLength });
        }

        public void SendPanAction(double deltaX, double deltaY)
        {
            Invoke(new object[] { deltaX, deltaY });
        }
    }
}
