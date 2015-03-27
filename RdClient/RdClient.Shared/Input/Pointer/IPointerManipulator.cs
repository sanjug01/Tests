using RdClient.Shared.CxWrappers;
using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer
{
    public interface IPointerManipulator
    {
        double MouseAcceleration { get; }
        Point MousePosition { get; set; }
        void SendMouseAction(MouseEventType eventType);
        void SendMouseWheel(int delta, bool isHorizontal);
        void SendTouchAction(TouchEventType type, uint contactId, Point position, ulong frameTime);
    }
}
