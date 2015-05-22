namespace RdClient.Telemetry
{
    using RdClient.Shared.Telemetry;
    using System;
    using System.Diagnostics.Contracts;

    sealed class ApplicationInsightsTelemetryClient : ITelemetryClient
    {
        private readonly ApplicationInsightsTelemetryCore _core;

        private sealed class Stopwatch : ITelemetryStopwatch
        {
            private readonly DateTime _startTime;
            private ApplicationInsightsTelemetryCore _core;

            public Stopwatch(ApplicationInsightsTelemetryCore core)
            {
                Contract.Requires(null != core);
                Contract.Ensures(null != _core);
                _startTime = DateTime.UtcNow;
                _core = core;
            }

            void ITelemetryStopwatch.Stop(string eventName)
            {
                if (null != _core)
                {
                    _core.Duration(eventName, DateTime.UtcNow - _startTime);
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
                if(value != _core.IsActive)
                {
                    if (value)
                    {
                        _core.Activate();
                    }
                    else
                    {
                        _core.Deactivate();
                    }
                }
            }
        }

        void ITelemetryClient.Event(string eventName)
        {
            _core.Event(eventName);
        }

        ITelemetryStopwatch ITelemetryClient.StartStopwatch()
        {
            return new Stopwatch(_core);
        }
    }
}
