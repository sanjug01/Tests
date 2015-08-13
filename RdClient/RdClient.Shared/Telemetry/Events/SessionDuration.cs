using System.Diagnostics;

namespace RdClient.Shared.Telemetry.Events
{
    /// <summary>
    /// Telemetry data point reported when a session disconnects.
    /// </summary>
    public sealed class SessionDuration
    {
        private readonly Stopwatch _duration;

        public SessionDuration(string source)
        {
            _duration = new Stopwatch();
            this.sourceType = source;
        }

        public void Start()
        {
            _duration.Start();
        }

        public void Stop()
        {
            _duration.Stop();
        }

        /// <summary>
        /// Source of the session (type of a tile activated in the UI to launch the session; may be desktop, desktop with gateway, etc.)
        /// </summary>
        public readonly string sourceType;

        /// <summary>
        /// Total duration of the session in minutes from the moment of first connect to the final termination.
        /// </summary>
        public double minutes
        {
            get { return _duration.Elapsed.TotalMinutes; }
        }
    }
}
