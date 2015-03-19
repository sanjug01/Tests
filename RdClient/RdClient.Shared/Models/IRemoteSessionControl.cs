using RdClient.Shared.CxWrappers;
using Windows.Foundation;
namespace RdClient.Shared.Models
{
    /// <summary>
    /// Interface for controlling input sent to an active remote session.
    /// </summary>
    public interface IRemoteSessionControl
    {
        IRenderingPanel RenderingPanel { get; }

        void SendKeystroke(int keyCode, bool isScanCode, bool isExtendedKey, bool isKeyReleased);
        void SendMouseAction(MouseEventType eventType);
        void SendTouchAction(TouchEventType type, uint contactId, Point position, ulong frameTime);
        void SendMouseWheel(int delta, bool isHorizontal);
    }
}
