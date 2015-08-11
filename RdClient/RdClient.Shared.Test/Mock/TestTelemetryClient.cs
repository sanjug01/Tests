namespace RdClient.Shared.Test.Mock
{
    using RdClient.Shared.Telemetry;

    sealed class TestTelemetryClient : ITelemetryClient
    {
        private bool _isActive;

        bool ITelemetryClient.IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        void ITelemetryClient.ReportEvent(object eventData)
        {
        }
    }
}
