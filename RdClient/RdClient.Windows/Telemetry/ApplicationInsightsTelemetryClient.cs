namespace RdClient.Telemetry
{
    using RdClient.Shared.Telemetry;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;

    sealed class ApplicationInsightsTelemetryClient : ITelemetryClient
    {
        private readonly ApplicationInsightsTelemetryCore _core;

        private sealed class TelemetryStopwatch : ITelemetryStopwatch
        {
            private readonly Stopwatch _stopwatch;
            private ApplicationInsightsTelemetryCore _core;

            public TelemetryStopwatch(ApplicationInsightsTelemetryCore core)
            {
                Contract.Requires(null != core);
                Contract.Ensures(null != _core);
                _core = core;
                _stopwatch = new Stopwatch();
                _stopwatch.Start();
            }

            void ITelemetryStopwatch.Stop(string eventName)
            {
                if (_core.IsActive)
                {
                    _stopwatch.Stop();
                    _core.Duration(eventName, _stopwatch.ElapsedMilliseconds);
                    _core = null;
                }
            }
        }

        public ApplicationInsightsTelemetryClient()
        {
            Contract.Ensures(null != _core);
            _core = new ApplicationInsightsTelemetryCore();
        }

        bool ITelemetryClient.IsActive
        {
            get
            {
                return _core.IsActive;
            }

            set
            {
                if (value != _core.IsActive)
                {
                    if (value)
                    {
                        _core.Activate();
                        _core.Metric("SendUsage", 1);
                    }
                    else
                    {
                        _core.Metric("SendUsage", 0);
                        _core.Deactivate();
                    }
                }
            }
        }

        void ITelemetryClient.Event(string eventName)
        {
                _core.Event(eventName);
        }

        void ITelemetryClient.Metric(string metricName, double metricValue)
        {
                _core.Metric(metricName, metricValue);
        }

        ITelemetryStopwatch ITelemetryClient.StartStopwatch()
        {
            return new TelemetryStopwatch(_core);
        }
    }
}
