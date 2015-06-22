namespace RdClient.Telemetry
{
    using RdClient.Shared.Telemetry;
    using System.Diagnostics;
    using System;

    sealed class DummyTelemetryClient : ITelemetryClient
    {
        private bool _isActive;

        private sealed class Stopwatch : ITelemetryStopwatch
        {
            private readonly DateTime _startTime;

            public Stopwatch(ApplicationInsightsTelemetryCore core)
            {
                _startTime = DateTime.UtcNow;
            }

            void ITelemetryStopwatch.Stop(string eventName)
            {
                TimeSpan duration = DateTime.UtcNow - _startTime;
                Debug.WriteLine("DummyTelemetryClient|Duration:{0}={1}", eventName, duration.Milliseconds);
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
            get
            {
                return _isActive;
            }

            set
            {
                if(value != _isActive)
                {
                    _isActive = value;

                    if (value)
                        TurnOn();
                    else
                        TurnOff();
                }
            }
        }

        void ITelemetryClient.Event(string eventName)
        {
            if(_isActive)
                Debug.WriteLine("DummyTelemetryClient|Event:{0}", eventName);
        }

        void ITelemetryClient.Metric(string metricName, double metricValue)
        {
            if (_isActive)
                Debug.WriteLine("DummyTelemetryClient|Metric:{0}={1}", metricName, metricValue);
        }

        ITelemetryStopwatch ITelemetryClient.StartStopwatch()
        {
            throw new NotImplementedException();
        }

        ITelemetryEvent ITelemetryClient.MakeEvent(string eventName)
        {
            return new TelemetryEvent(eventName);
        }

        private void TurnOn()
        {
            Debug.WriteLine("DummyTelemetryClient|TurnOn");
        }

        private void TurnOff()
        {
            Debug.WriteLine("DummyTelemetryClient|TurnOff");
        }
    }
}
