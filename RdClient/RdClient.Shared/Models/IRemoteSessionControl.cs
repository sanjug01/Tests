using RdClient.Shared.CxWrappers;
using RdClient.Shared.Input.Pointer;
using RdClient.Shared.Models.PanKnobModel;
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
        void SendMouseAction(MouseAction action);
        void SendTouchAction(TouchEventType type, uint contactId, Point position, ulong frameTime);
        void SendMouseWheel(int delta, bool isHorizontal);
    }
}
