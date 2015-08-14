namespace RdClient.Shared.Telemetry.Events
{
    /// <summary>
    /// Telemetry data point reported after user has requested to exit out of the full screen mode.
    /// </summary>
    public sealed class ExitFullScreen
    {
        public ExitFullScreen(IStopwatch stopwatch)
        {
            this.timeMark = stopwatch.Elapsed.TotalSeconds;
        }

        /// <summary>
        /// Time mark in seconds relative to the beginning of the session.
        /// </summary>
        public readonly double timeMark;
    }
}
