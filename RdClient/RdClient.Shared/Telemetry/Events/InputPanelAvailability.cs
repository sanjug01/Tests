namespace RdClient.Shared.Telemetry.Events
{
    /// <summary>
    /// Telemetry data point reportyed when availability of the touch keyboard (input pane) changes.
    /// </summary>
    public sealed class InputPanelAvailability
    {
        public InputPanelAvailability(bool can)
        {
            this.canShow = can;
        }

        /// <summary>
        /// True if the touch keyboard has become available; otherwise, false.
        /// </summary>
        public readonly bool canShow;
    }
}
