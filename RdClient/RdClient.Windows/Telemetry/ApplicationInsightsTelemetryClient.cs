namespace RdClient.Telemetry
{
    using Microsoft.ApplicationInsights.DataContracts;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Telemetry;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Threading;

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

        private sealed class TelemetryEvent : DisposableObject, ITelemetryEvent
        {
            private readonly ReaderWriterLockSlim _monitor;
            private readonly EventTelemetry _eventTelemetry;
            private ApplicationInsightsTelemetryCore _core;
            private IDictionary<string, TimeAccumulator> _stopwatches;

            private sealed class TimeAccumulator
            {
                private readonly Stopwatch _stopwatch;
                private long _accumulatedMilliseconds;

                public TimeAccumulator()
                {
                    _stopwatch = Stopwatch.StartNew();
                    _accumulatedMilliseconds = 0;
                }

                public void Pause()
                {
                    _stopwatch.Stop();
                    _accumulatedMilliseconds += _stopwatch.ElapsedMilliseconds;
                    _stopwatch.Reset();
                }

                public void Resume()
                {
                    _stopwatch.Start();
                }

                public long AccumulatedTime
                {
                    get { return _accumulatedMilliseconds; }
                }
            }

            public TelemetryEvent(string eventName, ApplicationInsightsTelemetryCore core)
            {
                Contract.Assert(null != core);
                _monitor = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
                _eventTelemetry = new EventTelemetry(eventName);
                _core = core;
            }

            void ITelemetryEvent.AddTag(string tagName, string value)
            {
                using (ReadWriteMonitor.Write(_monitor))
                    _eventTelemetry.Properties.Add(tagName, value);
            }

            void ITelemetryEvent.AddMetric(string metricName, double value)
            {
                using (ReadWriteMonitor.Write(_monitor))
                    _eventTelemetry.Metrics.Add(metricName, value);
            }

            void ITelemetryEvent.StartStopwatch(string metricName)
            {
                using (ReadWriteMonitor.Write(_monitor))
                    this.Stopwatches.Add(metricName, new TimeAccumulator());
            }

            void ITelemetryEvent.PauseStopwatch(string metricName)
            {
                TimeAccumulator stopwatch;

                using (ReadWriteMonitor.Write(_monitor))
                {
                    if (null != _stopwatches && _stopwatches.TryGetValue(metricName, out stopwatch))
                    {
                        stopwatch.Pause();
                    }
                }
            }

            void ITelemetryEvent.ResumeStopwatch(string metricName)
            {
                TimeAccumulator stopwatch;

                using (ReadWriteMonitor.Write(_monitor))
                {
                    if (null != _stopwatches && _stopwatches.TryGetValue(metricName, out stopwatch))
                    {
                        stopwatch.Resume();
                    }
                }
            }

            void ITelemetryEvent.Report()
            {
                using (ReadWriteMonitor.UpgradeableRead(_monitor))
                {
                    if (null != _core)
                    {
                        if (null != _stopwatches)
                        {
                            foreach (KeyValuePair<string, TimeAccumulator> pair in _stopwatches)
                            {
                                pair.Value.Resume();
                                _eventTelemetry.Metrics.Add(pair.Key, pair.Value.AccumulatedTime);
                            }
                        }

                        _core.Event(_eventTelemetry);

                        using (ReadWriteMonitor.Write(_monitor))
                            _core = null;
                    }
                }
            }

            private IDictionary<string, TimeAccumulator> Stopwatches
            {
                get
                {
                    using (ReadWriteMonitor.Write(_monitor))
                    {
                        if (null == _stopwatches)
                            _stopwatches = new SortedDictionary<string, TimeAccumulator>();
                    }
                    return _stopwatches;
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
            if(_core.IsActive)
                _core.Event(eventName);
        }

        void ITelemetryClient.Metric(string metricName, double metricValue)
        {
            if (_core.IsActive)
                _core.Metric(metricName, metricValue);
        }

        ITelemetryStopwatch ITelemetryClient.StartStopwatch()
        {
            return new TelemetryStopwatch(_core);
        }

        ITelemetryEvent ITelemetryClient.MakeEvent(string eventName)
        {
            return new TelemetryEvent(eventName, _core);
        }
    }
}
