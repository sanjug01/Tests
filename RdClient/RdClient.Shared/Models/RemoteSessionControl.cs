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

        void IRemoteSessionControl.SendKeystroke(int keyCode, bool isScanCode, bool isExtendedKey, bool isKeyReleased)
        {
            _connection.SendKeyEvent(keyCode, isScanCode, isExtendedKey, isKeyReleased);
        }
    }
}
