using RdClient.Shared.CxWrappers;
using RdClient.Shared.Input.Pointer;
using System.Collections.Generic;
using Windows.Foundation;

namespace RdClient.Shared.Test.Mock
{
    public class TestMousePointerEvent
    {
        public MouseEventType Type { get; set; }
        public Point Position { get; set; }
    }

    public class TestMultiTouchEvent
    {
        public TouchEventType ActionType { get; set; }
        public uint ContactId { get; set; }
        public Point Position { get; set; }
        public ulong FrameTime { get; set; }
    }

    public class PointerManipulatorRecorder : IPointerManipulator
    {
        public List<Mock.TestMousePointerEvent> _mouseEventLog = new List<Mock.TestMousePointerEvent>();
        public List<TestMultiTouchEvent> _touchEventLog = new List<TestMultiTouchEvent>();

        public double MouseAcceleration
        {
            get { return 1.0; }
            set { }
        }

        public Point MousePosition
        {
            get;
            set;
        }

        public void SendMouseAction(MouseEventType type)
        {
            _mouseEventLog.Add(new Mock.TestMousePointerEvent() { Position = MousePosition, Type = type });
        }

        public void SendMouseWheel(int delta, bool isHorizontal)
        {
            Point position;
            MouseEventType type;

            if(isHorizontal)
            {
                position = new Point(delta, 0);
                type = MouseEventType.MouseHWheel;
            }
            else
            {
                position = new Point(0, delta);
                type = MouseEventType.MouseWheel;
            }

            _mouseEventLog.Add(new Mock.TestMousePointerEvent() { Position = position, Type = type });
        }

        public void SendTouchAction(TouchEventType type, uint contactId, Point position, ulong frameTime)
        {
            _touchEventLog.Add(new TestMultiTouchEvent() { ActionType = type, ContactId = contactId, Position = position, FrameTime = frameTime });
        }

        public void SendPinchAndZoom(double centerX, double centerY, double fromLength, double toLength)
        {

        }

        public void SendPanAction(double deltaX, double deltaY)
        {

        }
    }
}
