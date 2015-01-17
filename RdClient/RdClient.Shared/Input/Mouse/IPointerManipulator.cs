using RdClient.Shared.CxWrappers;
using Windows.Foundation;

namespace RdClient.Shared.Input.Mouse
{
    public interface IPointerManipulator
    {
        public bool SupportsMultiTouch { get; }
        Point MousePosition { get; set; }
        void SendMouseAction(MouseEventType eventType);
    }
}
