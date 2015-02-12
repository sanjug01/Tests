namespace RdClient.Telemetry
{
    using Microsoft.ApplicationInsights;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Telemetry;
    using System.Diagnostics.Contracts;

    sealed class ApplicationInsightsTelemetry : IApplicationTelemetry
    {
        private readonly TelemetryClient _telemetryClient;

        private sealed class TelemetryActivity : ITelemetryActivity
        {
            private readonly TelemetryClient _telemetryClient;
            private readonly string _name;

            public TelemetryActivity(TelemetryClient telemetryClient, string name)
            {
                _telemetryClient = telemetryClient;
                _name = name;
            }

            void ITelemetryActivity.Complete()
            {
                _telemetryClient.TrackPageView(_name);
            }
        }

        public ApplicationInsightsTelemetry()
        {
            _telemetryClient = new TelemetryClient();
        }

        void IApplicationTelemetry.Event(string eventPath)
        {
            Contract.Requires(null != eventPath);

            _telemetryClient.TrackEvent(eventPath);
        }

        void IApplicationTelemetry.Metric(string name, double value)
        {
            Contract.Requires(null != name);

            _telemetryClient.TrackMetric(name, value);
        }

        ITelemetryActivity IApplicationTelemetry.PresentView(string name)
        {
            return new TelemetryActivity(_telemetryClient, name);
        }
    }
}
