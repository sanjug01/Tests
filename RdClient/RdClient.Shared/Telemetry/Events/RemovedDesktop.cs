namespace RdClient.Shared.Telemetry.Events
{
    /// <summary>
    /// Telemetry data point reported after user has removed a desktop from local configuration.
    /// </summary>
    public sealed class RemovedDesktop
    {
        public RemovedDesktop(int count)
        {
            this.desktops = count;
        }

        /// <summary>
        /// New number of desktops in the local configuration.
        /// </summary>
        public readonly double desktops;
    }
}
