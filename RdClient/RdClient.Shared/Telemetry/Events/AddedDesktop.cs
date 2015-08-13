namespace RdClient.Shared.Telemetry.Events
{
    /// <summary>
    /// Telemetry data point reported after user has added a new local desktop.
    /// </summary>
    public sealed class AddedDesktop
    {
        public AddedDesktop(int count)
        {
            this.desktops = count;
        }

        /// <summary>
        /// New number of desktops in the local configuration.
        /// </summary>
        public readonly double desktops;
    }
}
