namespace RdClient.Shared.Telemetry
{
    public interface ITelemetryEvent
    {
        void AddTag(string tagName, string value);
        void AddMetric(string metricName, double value);
        void StartStopwatch(string metricName);
        void PauseStopwatch(string metricName);
        void ResumeStopwatch(string metricName);
        void Report();
    }
}
