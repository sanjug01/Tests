using RdClient.Shared.Telemetry;
using RdMock;
using System;

namespace RdClient.Shared.Test.Mock
{
    public class TelemetryClient : MockBase, ITelemetryClient
    {
        public bool IsActive { get; set; }

        void ITelemetryClient.Event(string eventName)
        {
            Invoke(new object[] { eventName });
        }

        void ITelemetryClient.Metric(string metricName, double metricValue)
        {
            Invoke(new object[] { metricName, metricValue });
        }

        ITelemetryStopwatch ITelemetryClient.StartStopwatch()
        {
            return (ITelemetryStopwatch)Invoke(new object[] { });
        }

        ITelemetryEvent ITelemetryClient.MakeEvent(string eventName)
        {
            return (ITelemetryEvent)Invoke(new object[] { eventName });
        }
    }
}
