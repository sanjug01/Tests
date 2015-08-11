using System.Diagnostics;

namespace RdClient.Shared.Telemetry.Events
{
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

        public readonly string sourceType;

        public double minutes
        {
            get { return _duration.Elapsed.TotalMinutes; }
        }
    }
}
