namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using System.Diagnostics.Contracts;

    internal sealed class RemoteSessionControl : IRemoteSessionControl
    {
        private readonly IRdpConnection _connection;

        public RemoteSessionControl(IRdpConnection connection)
        {
            Contract.Requires(null != connection);

            _connection = connection;
        }
    }
}
