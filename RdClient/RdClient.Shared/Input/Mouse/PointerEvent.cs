using Windows.Foundation;

namespace RdClient.Shared.Input.Mouse
{
    public enum PointerType
    {
        Unknown,
        Mouse,
        Pen,
        Touch
    }

    public enum PointerActionType
    {
        Unknown,
        Up,
        Down,
        Update
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
        public PointerActionType ActionType { get; private set; }

        public PointerEvent(
            Point position, 
            bool inertia, 
            Point delta, 
            bool leftButton, 
            bool rightButton, 
            PointerType pointerType, 
            uint pointerId, 
            ulong timeStamp = 0, 
            PointerActionType actionType = PointerActionType.Unknown)
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
        }
    }
}
