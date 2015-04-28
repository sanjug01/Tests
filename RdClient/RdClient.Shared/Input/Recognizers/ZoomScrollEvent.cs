using RdClient.Shared.Input.Pointer;
using Windows.Foundation;
using Windows.UI.Input;

namespace RdClient.Shared.Input.Recognizers
{
    public class ZoomScrollEvent : IZoomScrollEvent
    {
        public PointerEventAction Action { get; private set; }

		public Point Position { get; private set; }

        public ManipulationDelta Delta { get; private set; }

        public bool IsInertial { get; private set; }

        public ZoomScrollType Type { get; private set; }

		public ZoomScrollEvent(PointerEventAction action, ZoomScrollType type, ManipulationDelta delta, bool isInertial, Point position)
        {
            Action = action;
            Type = type;
            Delta = delta;
            IsInertial = isInertial;
            Position = position;
        }
    }
}
