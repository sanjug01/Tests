namespace RdClient.Shared.Telemetry
{
    /// <summary>
    /// Abstraction of a telemetry client - application facility that sends telemetry data to a telemetry back-end.
    /// One instance of the client is created by the application and is injected into view models by a navigation extension.
    /// </summary>
    public interface ITelemetryClient
    {
        bool IsActive { get; set; }

        /// <summary>
        /// Report a unique application event.
        /// </summary>
        /// <param name="eventName">Name of the event that can be queried from the telemetry back-end.</param>
        void Event(string eventName);

        /// <summary>
        /// Start a stopwatch that will track the duration of a process in the app.
        /// </summary>
        /// <returns>The stopwatch object that can be stopped to report the duration to the telemetry back-end.</returns>
        ITelemetryStopwatch StartStopwatch();
    }
}
