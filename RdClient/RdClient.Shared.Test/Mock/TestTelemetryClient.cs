namespace RdClient.Shared.Test.Mock
{
    using RdClient.Shared.Telemetry;

    sealed class TestTelemetryClient : ITelemetryClient
    {
        private bool _isActive;

        private sealed class Stopwatch : ITelemetryStopwatch
        {
            void ITelemetryStopwatch.Stop(string eventName)
            {
                //
                // Do nothing.
                //
            }
        }

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

        void ITelemetryClient.Event(string eventName)
        {
            //
            // Do nothing.
            //
        }

        void ITelemetryClient.Metric(string metricName, double metricValue)
        {
            //
            // Do nothing.
            //
        }

        ITelemetryStopwatch ITelemetryClient.StartStopwatch()
        {
            return new Stopwatch();
        }

        ITelemetryEvent ITelemetryClient.MakeEvent(string eventName)
        {
            return new TelemetryEvent(eventName);
        }
    }
}
