namespace RdClient.Shared.Data
{
    using RdClient.Shared.CxWrappers;

    public interface IServerIdentityTrust
    {
        /// <summary>
        /// Establish trust to the specified server (which may not support authentication).
        /// </summary>
        /// <param name="host">Server name or ip address.</param>
        void TrustServer(string hostName);

        /// <summary>
        /// Remove all established server trust.
        /// </summary>
        void RemoveAllTrust();

        /// <summary>
        /// Verifies that the host has been trusted.
        /// Note: name, fdqn and IP address are managed separately 
        /// </summary>
        /// <param name="host">Server name or ip address.</param>
        /// <returns>true, if the server has been trusted</returns>
        bool IsServerTrusted(string hostName);
    }
}
