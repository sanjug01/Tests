using RdClient.Shared.CxWrappers;
using RdClient.Shared.Input.Pointer;
using RdClient.Shared.Models;
using RdMock;
using Windows.Foundation;

namespace RdClient.Shared.Test.Mock
{
    public class RemoteSessionControl : MockBase, IRemoteSessionControl
    {
        public IRenderingPanel RenderingPanel { get; set; }

        public void SendKeystroke(int keyCode, bool isScanCode, bool isExtendedKey, bool isKeyReleased)
        {
            Invoke(new object[] { keyCode, isScanCode, isExtendedKey, isKeyReleased });
        }

        public void SendMouseAction(MouseAction action)
        {
            Invoke(new object[] { action });
        }

        public void SendMouseWheel(int delta, bool isHorizontal)
        {
            Invoke(new object[] { delta, isHorizontal });
        }

        public void SendTouchAction(TouchEventType type, uint contactId, Point position, ulong frameTime)
        {
            Invoke(new object[] { type, contactId, position, frameTime });
        }
    }
}
