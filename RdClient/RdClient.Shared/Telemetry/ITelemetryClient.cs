namespace RdClient.Shared.Telemetry
{
    /// <summary>
    /// Abstraction of a telemetry client - application facility that sends telemetry data to a telemetry back-end.
    /// One instance of the client is created by the application and is injected into view models by a navigatin extension.
    /// </summary>
    public interface ITelemetryClient
    {
        bool IsActive { get; set; }
    }
}
