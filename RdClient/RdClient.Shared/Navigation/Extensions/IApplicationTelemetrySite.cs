namespace RdClient.Shared.Navigation.Extensions
{
    using RdClient.Shared.Telemetry;

    public interface IApplicationTelemetrySite
    {
        void SetApplicationTelemetry(IApplicationTelemetry applicationTelemetry);
    }
}
