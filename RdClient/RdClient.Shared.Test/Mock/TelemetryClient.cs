using RdClient.Shared.Telemetry;
using RdMock;
using System;
using System.Collections.Generic;

namespace RdClient.Shared.Test.Mock
{
    public class TelemetryClient : MockBase, ITelemetryClient
    {
        public bool IsActive { get; set; }

        public void Metric(string metricName, IDictionary<string, string> properties)
        {
            Invoke(new object[] { metricName, properties });
        }

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
    }
}
