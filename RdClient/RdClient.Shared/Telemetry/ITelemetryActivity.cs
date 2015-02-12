namespace RdClient.Shared.Telemetry
{
    /// <summary>
    /// Object that tracks a non-momentary telemetry activity that is defined
    /// by beginning and end events.
    /// </summary>
    public interface ITelemetryActivity
    {
        void Complete();
    }
}
