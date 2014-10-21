using RdClient.Shared.Models;

namespace RdClient.Shared.CxWrappers
{
    public interface IRdpConnection
    {
        int SetUserCredentials(Credentials credentials, bool fUsingSavedCreds);
        void Connect();
        void Disconnect();
        void Suspend();
        void Resume();
    }
}
