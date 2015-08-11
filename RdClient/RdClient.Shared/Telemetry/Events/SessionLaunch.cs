namespace RdClient.Shared.Telemetry.Events
{
    public sealed class SessionLaunch
    {
        public string state;
        public string sourceType;
        public string gwyCreds;
        public string hostAddressType;
        public string hostCreds;
        public string userInteractionMode;
        public bool userInitiated;
        public int disconnectReason;
        public string disconnectCode;
        public string disconnectExtendedCode;
        public bool success;
        public string networkType;
    }
}
