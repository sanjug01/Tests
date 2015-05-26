namespace RdClient.Shared.Test.Mock
{
    using RdClient.Shared.Telemetry;

    sealed class TestTelemetryClient : ITelemetryClient
    {
        private bool _isActive;

        private sealed class Stopwatch : ITelemetryStopwatch
        {
            void ITelemetryStopwatch.Stop(string eventName)
            {
                //
                // Do nothing.
                //
            }
        }

        bool ITelemetryClient.IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        void ITelemetryClient.Event(string eventName)
        {
            //
            // Do nothing.
            //
        }

        ITelemetryStopwatch ITelemetryClient.StartStopwatch()
        {
            return new Stopwatch();
        }
    }
}
