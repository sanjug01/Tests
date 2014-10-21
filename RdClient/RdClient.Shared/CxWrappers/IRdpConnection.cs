using RdClient.Shared.Models;

namespace RdClient.Shared.CxWrappers
{
    public interface IRdpConnection
    {
        void Connect(Credentials credentials, bool fUsingSavedCreds);
        void Disconnect();
        void Suspend();
        void Resume();
    }
}
