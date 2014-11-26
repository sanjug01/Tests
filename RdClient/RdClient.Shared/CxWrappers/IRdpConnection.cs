using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.Models;

namespace RdClient.Shared.CxWrappers
{
    public interface IRdpConnection
    {
        IRdpEvents Events { get; }

        void Connect(Credentials credentials, bool fUsingSavedCreds);
        void Disconnect();
        void Suspend();
        void Resume();
        void TerminateInstance();
        void HandleAsyncDisconnectResult(RdpDisconnectReason disconnectReason, bool reconnectToServer);
        void GetSnapshot(out int width, out int height, out byte[] bytes);
    }
}
