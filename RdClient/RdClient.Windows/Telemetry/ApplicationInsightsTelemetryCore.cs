﻿namespace RdClient.Telemetry
{
    using Microsoft.ApplicationInsights;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Core of the Application Insights telemetry client. The core may be safely passed to
    /// child objects created by the implementation of ITelemetryClient.
    /// </summary>
    sealed class ApplicationInsightsTelemetryCore
    {
        private TelemetryClient _client;

        public ApplicationInsightsTelemetryCore()
        {
            _client = new TelemetryClient();
        }

        public bool IsActive
        {
            get { return null != _client; }
        }

        public void Activate()
        {
            Contract.Assert(null == _client);
            _client = new TelemetryClient();
        }

        public void Deactivate()
        {
            Contract.Assert(null != _client);
            _client.Flush();
            _client = null;
        }

        public void Event(string eventName)
        {
            if (null != _client)
                _client.TrackEvent(eventName);
        }

        public void Metric(string metricName, double metricValue)
        {
            if (null != _client)
                _client.TrackMetric(metricName, metricValue);
        }

        public void Duration(string eventName, long milliseconds)
        {
            if (null != _client)
                _client.TrackMetric(eventName, milliseconds / 60000);
        }
    }
}