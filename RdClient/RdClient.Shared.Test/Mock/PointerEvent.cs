using RdClient.Shared.Input.Pointer;
using Windows.Foundation;

namespace RdClient.Shared.Test.Mock
{
    public class PointerEventBase : IPointerEventBase
    {
        private PointerEventAction _action;
        public PointerEventAction Action { get { return _action; } }

        private Point _position;
        public Point Position { get { return _position; } }

        public PointerEventBase(PointerEventAction action, Point position)
        {
            _action = action;
            _position = position;
        }
    }
}
