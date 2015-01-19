using RdClient.Shared.CxWrappers;
using Windows.Foundation;

namespace RdClient.Shared.Input.Mouse
{
    public interface IPointerManipulator
    {
        double MouseAcceleration { get; set; }
        Point MousePosition { get; set; }
        void SendMouseAction(MouseEventType eventType);
    }
}
