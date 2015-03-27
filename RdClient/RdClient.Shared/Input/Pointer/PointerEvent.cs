using RdClient.Shared.CxWrappers;
using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer
{
    public enum PointerType
    {
        Unknown,
        Mouse,
        Pen,
        Touch
    }

    public class PointerEvent
    {
        public Point Position { get; private set; }
        public bool Inertia { get; private set; }
        public Point Delta { get; private set; }
        public bool LeftButton { get; private set; }
        public bool RightButton { get; private set; }
        public PointerType PointerType { get; set; }
        public uint PointerId { get; private set; }
        public ulong TimeStamp { get; private set; }
        public TouchEventType ActionType { get; private set; }
        public int MouseWheelDelta { get; private set; }
        public bool IsHorizontalMouseWheel { get; private set; }

        public PointerEvent(
            Point position, 
            bool inertia, 
            Point delta, 
            bool leftButton, 
            bool rightButton, 
            PointerType pointerType, 
            uint pointerId, 
            ulong timeStamp = 0,
            TouchEventType actionType = TouchEventType.Unknown,
            int mouseWheelDelta = 0,
            bool isHorizontalMouseWheel = false)
        {
            Position = position;
            Inertia = inertia;
            Delta = delta;
            LeftButton = leftButton;
            RightButton = rightButton;
            PointerType = pointerType;
            PointerId = pointerId;
            TimeStamp = timeStamp;
            ActionType = actionType;
            MouseWheelDelta = mouseWheelDelta;
            IsHorizontalMouseWheel = isHorizontalMouseWheel;
        }
    }
}
