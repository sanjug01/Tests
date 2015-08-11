namespace RdClient.Shared.Test.Mock
{
    using RdClient.Shared.Telemetry;

    sealed class TestTelemetryClient : ITelemetryClient
    {
        private bool _isActive;

        private sealed class TelemetryEvent : ITelemetryEvent
        {
            private readonly string _eventName;

            public TelemetryEvent(string eventName)
            {
                _eventName = eventName;
            }

            void ITelemetryEvent.AddMetric(string metricName, double value)
            {
            }

            void ITelemetryEvent.StartStopwatch(string metricName)
            {
            }

            void ITelemetryEvent.PauseStopwatch(string metricName)
            {
            }

            void ITelemetryEvent.ResumeStopwatch(string metricName)
            {
            }

            void ITelemetryEvent.AddTag(string tagName, string value)
            {
            }

            void ITelemetryEvent.Report()
            {
            }
        }

        bool ITelemetryClient.IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        void ITelemetryClient.ReportEvent(object eventData)
        {
        }

        ITelemetryEvent ITelemetryClient.MakeEvent(string eventName)
        {
            return new TelemetryEvent(eventName);
        }
    }
}
