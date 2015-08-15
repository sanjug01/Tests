namespace RdClient.Shared.Telemetry.Events
{
    /// <summary>
    /// Telemetry data point reported after user has requested to enter the full screen mode.
    /// </summary>
    public sealed class EnterFullScreen
    {
        public EnterFullScreen(IStopwatch stopwatch)
        {
            this.timeMark = stopwatch.Elapsed.TotalSeconds;
        }

        /// <summary>
        /// Time mark in seconds relative to the beginning of the session.
        /// </summary>
        public readonly double timeMark;
    }
}
