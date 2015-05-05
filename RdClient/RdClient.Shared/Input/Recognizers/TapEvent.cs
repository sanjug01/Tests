using RdClient.Shared.Input.Pointer;
using Windows.Foundation;

namespace RdClient.Shared.Input.Recognizers
{
    public class TapEvent : ITapEvent
    {
        private PointerEventAction _action;
        public PointerEventAction Action { get { return _action; } }

        private Point _position;
        public Point Position { get { return _position; } }

        private TapEventType _type;
        public TapEventType Type { get { return _type; } }

        public TapEvent(PointerEventAction action, Point position, TapEventType type)
        {
            _action = action;
            _position = position;
            _type = type;
        }
    }
}
