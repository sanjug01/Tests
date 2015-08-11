namespace RdClient.Shared.Telemetry.Events
{
    public sealed class EnterFullScreen
    {
        public EnterFullScreen(IStopwatch stopwatch)
        {
            this.duration = stopwatch.Elapsed.TotalSeconds;
        }

        public readonly double duration;
    }
}
