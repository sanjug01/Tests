using RdClient.Shared.Telemetry;
using RdMock;

namespace RdClient.Shared.Test.Mock
{
    public class TelemetryClient : MockBase, ITelemetryClient
    {
        public bool IsActive { get; set; }

        void ITelemetryClient.ReportEvent(object eventData)
        {
            Invoke(new object[] { eventData });
        }
    }
}
