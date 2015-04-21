using Windows.UI.Xaml;

namespace RdClient.Shared.Input.Pointer
{
    public class PointerEvent
    {
        public PointerEventAction Action { get; private set; }
        public PointerEventType Type { get; private set; }
        public object Args { get; private set; }
        public UIElement Receiver { get; private set; }

        public PointerEvent(PointerEventAction action, PointerEventType type, object args, UIElement receiver)
        {
            Action = action;
            Type = type;
            Args = args;
            Receiver = receiver;
        }
    }
}
