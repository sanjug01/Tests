using Windows.Foundation;

namespace RdClient.Shared.Helpers
{
    public enum PointerType
    {
        Unknown,
        Mouse,
        Pen,
        Touch,
        Click
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
