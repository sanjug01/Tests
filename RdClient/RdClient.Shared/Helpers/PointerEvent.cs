using Windows.Foundation;

namespace RdClient.Shared.Helpers
{
    public enum PointerEventType
    {
        LeftPress = 0,
        LeftRelease = 1,
        MouseHWheel = 2,
        MouseWheel = 3,
        Move = 4,
        RightPress = 5,
        RightRelease = 6
    }

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
        public PointerType PointerType { get; private set; }
        public uint PointerId { get; private set; }

        public PointerEvent(Point position, bool inertia, Point delta, bool leftButton, bool rightButton, PointerType pointerType, uint pointerId)
        {
            Position = position;
            Inertia = inertia;
            Delta = delta;
            LeftButton = leftButton;
            RightButton = rightButton;
            PointerType = pointerType;
            PointerId = pointerId;
        }
    }
}
