namespace RdClient.Telemetry
{
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Diagnostics;
    using System;
    using Windows.System.Threading;



    /// <summary>
    /// Core of the Application Insights telemetry client. The core may be safely passed to
    /// child objects created by the implementation of ITelemetryClient.
    /// </summary>
    sealed class ApplicationInsightsTelemetryCore
    {
        private TelemetryClient _client;
        private TimeSpan _TimePeriod;
        private readonly object FlushLock = new object();
        ThreadPoolTimer _PeriodicTimer;

        private void FlushTelemetryData(ThreadPoolTimer timer)
        {
            lock(FlushLock)
            {
                _client.Flush();
            }
        }
        public ApplicationInsightsTelemetryCore()
        {
            _client = new TelemetryClient();
            _TimePeriod = TimeSpan.FromSeconds(120);
            _PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(FlushTelemetryData, _TimePeriod);
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
            _client = null;
            _PeriodicTimer.Cancel();
            _PeriodicTimer = null;
        }

        public void Event(string eventName)
        {
            if (null != _client)
            {
                _client.TrackEvent(eventName);
            }
        }

        public void Event(EventTelemetry eventTelemetry)
        {
            if(null != _client)
            {
                _client.TrackEvent(eventTelemetry);
            }
        }

        public void Metric(string metricName, double metricValue)
        {
            if (null != _client)
            {
                _client.TrackMetric(metricName, metricValue);
            }
        }

        public void Duration(string eventName, long milliseconds)
        {
            if (null != _client)
            {
                _client.TrackMetric(eventName, milliseconds / 60000);
            }
        }
    }
}
