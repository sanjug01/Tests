namespace RdClient.Shared.Telemetry.Events
{
    public sealed class ExitFullScreen
    {
        public ExitFullScreen(IStopwatch stopwatch)
        {
            this.duration = stopwatch.Elapsed.TotalSeconds;
        }

        public readonly double duration;
    }
}
