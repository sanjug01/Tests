namespace RdClient.Telemetry
{
    using Microsoft.ApplicationInsights;
    using RdClient.Shared.Telemetry;
    using System.Diagnostics.Contracts;

    sealed class ApplicationInsightsTelemetryClient : ITelemetryClient
    {
        private bool _isActive;
        private TelemetryClient _telemetryClient;

        public ApplicationInsightsTelemetryClient()
        {
        }

        bool ITelemetryClient.IsActive
        {
            get
            {
                return _isActive;
            }

            set
            {
                if(value != _isActive)
                {
                    _isActive = value;

                    if (value)
                    {
                        Contract.Assert(null == _telemetryClient);
                        _telemetryClient = new TelemetryClient();
                    }
                    else
                    {
                        Contract.Assert(null != _telemetryClient);
                        _telemetryClient.Flush();
                        _telemetryClient = null;
                    }
                }
            }
        }
    }
}
