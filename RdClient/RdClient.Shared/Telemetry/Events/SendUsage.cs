namespace RdClient.Shared.Telemetry.Events
{
    /// <summary>
    /// Telemetry data point reported when user opts in or out of sending telemetry.
    /// </summary>
    public sealed class SendUsage
    {
        public SendUsage(bool send)
        {
            this.sendTelemetry = send;
        }

        /// <summary>
        /// True if user has opted in; false, otherwise.
        /// </summary>
        public readonly bool sendTelemetry;
    }
}
