namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers.Errors;
    using System;

    public sealed class SessionFailureEventArgs : EventArgs
    {
        private readonly RdpDisconnectCode _disconnectCode;

        public RdpDisconnectCode DisconnectCode
        {
            get { return _disconnectCode; }
        }

        public SessionFailureEventArgs(RdpDisconnectCode disconnectCode)
        {
            _disconnectCode = disconnectCode;
        }
    }
}
