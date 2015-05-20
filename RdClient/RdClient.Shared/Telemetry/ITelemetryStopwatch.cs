namespace RdClient.Shared.Telemetry
{
    /// <summary>
    /// Interface of a stopwatch that is started by the telemetry client to track duration
    /// of some process in the application.
    /// </summary>
    public interface ITelemetryStopwatch
    {
        /// <summary>
        /// Stop the stopwatch and report the accumulated duration and event name to the telemetry back end.
        /// </summary>
        /// <param name="eventName"></param>
        /// <remarks>A stopwatch can only be stopped once.</remarks>
        void Stop(string eventName);
    }
}
