namespace RdClient.Shared.Navigation.Extensions
{
    using RdClient.Shared.Telemetry;

    public interface ITelemetryClientSite
    {
        void SetTelemetryClient(ITelemetryClient telemetryClient);
    }
}
