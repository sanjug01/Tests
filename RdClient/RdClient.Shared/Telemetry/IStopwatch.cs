namespace RdClient.Shared.Telemetry
{
    using System;

    /// <summary>
    /// Wrapper for System.Diagnostics.Stopwatch that enables injection of stopwatches in test code.
    /// </summary>
    public interface IStopwatch
    {
        TimeSpan Elapsed { get; }

        void Start();

        void Stop();

        void Reset();
    }
}
