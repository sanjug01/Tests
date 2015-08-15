namespace RdClient.Shared.Telemetry.Events
{
    /// <summary>
    /// Telemetry data point reported upon every launch of the application.
    /// </summary>
    public sealed class LaunchConfiguration
    {
        /// <summary>
        /// Number of desktops saved in the local configuration.
        /// </summary>
        public int localDesktopCount;

        /// <summary>
        /// Number of credentials saved in the local configuration.
        /// </summary>
        public int credentialsCount;

        /// <summary>
        /// Number of gateways saved in the local configuration.
        /// </summary>
        public int gatewaysCount;
    }
}
