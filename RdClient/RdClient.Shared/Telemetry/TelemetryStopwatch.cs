namespace RdClient.Shared.Telemetry
{
    using System;
    using System.Diagnostics;

    public sealed class TelemetryStopwatch : IStopwatch
    {
        private readonly Stopwatch _stopwatch;

        public TelemetryStopwatch()
        {
            _stopwatch = new Stopwatch();
        }

        TimeSpan IStopwatch.Elapsed
        {
            get { return _stopwatch.Elapsed; }
        }

        void IStopwatch.Reset()
        {
            _stopwatch.Reset();
        }

        void IStopwatch.Start()
        {
            _stopwatch.Start();
        }

        void IStopwatch.Stop()
        {
            _stopwatch.Stop();
        }
    }
}
