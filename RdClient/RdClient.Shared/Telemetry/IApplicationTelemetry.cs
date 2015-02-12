namespace RdClient.Shared.Telemetry
{
    using RdClient.Shared.Navigation;
    using System;

    public interface IApplicationTelemetry
    {
        void Event(string eventPath);
        void Metric(string name, double value);
        ITelemetryActivity PresentView(string name);
    }
}
