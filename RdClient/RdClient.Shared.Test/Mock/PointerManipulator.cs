using RdClient.Shared.CxWrappers;
using RdClient.Shared.Input.Mouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public PointerActionType ActionType { get; set; }
        public uint ContactId { get; set; }
        public Point Position { get; set; }
        public ulong FrameTime { get; set; }
    }

    public class PointerManipulator : IPointerManipulator
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

        public void SendTouchAction(PointerActionType type, uint contactId, Point position, ulong frameTime)
        {
            _touchEventLog.Add(new TestMultiTouchEvent() { ActionType = type, ContactId = contactId, Position = position, FrameTime = frameTime });
        }
    }
}
