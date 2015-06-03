namespace RdClient.Shared.Test.Mock
{
    using System;
    using System.Collections.Generic;
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

        public void Metric(string metricName, IDictionary<string, string> properties)
        {
            //
            // Do nothing.
            //
        }
    }
}
