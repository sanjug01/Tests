using RdClient.Shared.CxWrappers;
using Windows.Foundation;

namespace RdClient.Shared.Input.Mouse
{
    public interface IPointerManipulator
    {
        Point MousePosition { get; set; }
        void SendMouseAction(MouseEventType eventType);
    }
}
