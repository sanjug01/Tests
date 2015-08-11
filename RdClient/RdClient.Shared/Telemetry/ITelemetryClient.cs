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
        /// Report collection of public properties of an arbitrary object. Numerical and boolean properties are reported as double metric values.
        /// All other properties are reported as strings. The object class is used as the name of the telemetry event.
        /// </summary>
        /// <param name="eventData">Object that represents a telemetry event.</param>
        void ReportEvent(object eventData);
    }
}
