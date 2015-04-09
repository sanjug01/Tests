using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.Models;
using Windows.Foundation;

namespace RdClient.Shared.CxWrappers
{
    public interface IRdpConnection
    {
        IRdpEvents Events { get; }
        void SetCredentials(CredentialsModel credentials, bool fUsingSavedCreds);
        void SetGateway(GatewayModel gateway, CredentialsModel credentials);
        void Connect();
        void Disconnect();
        void Suspend();
        void Resume();
        void TerminateInstance();
        void Cleanup();
        void HandleAsyncDisconnectResult(RdpDisconnectReason disconnectReason, bool reconnectToServer);
        IRdpScreenSnapshot GetSnapshot();
        IRdpCertificate GetServerCertificate();
        void SendMouseEvent(MouseEventType type, float xPos, float yPos);
        void SendKeyEvent(int keyValue, bool scanCode, bool extended, bool keyUp);
        void SendTouchEvent(TouchEventType type, uint contactId, Point position, ulong frameTime);

        void SetLeftHandedMouseMode(bool value);
    }
}
