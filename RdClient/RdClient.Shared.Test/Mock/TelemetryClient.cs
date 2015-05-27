using RdClient.Shared.Telemetry;
using RdMock;
using System;

namespace RdClient.Shared.Test.Mock
{
    public class TelemetryClient : MockBase, ITelemetryClient
    {
        public bool IsActive { get; set; }

        public void Event(string eventName)
        {
            Invoke(new object[] { eventName });
        }

        public ITelemetryStopwatch StartStopwatch()
        {
            return (ITelemetryStopwatch) Invoke(new object[] { });
        }
    }
}
