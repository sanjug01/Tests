namespace RdClient.Telemetry
{
    using Microsoft.ApplicationInsights.DataContracts;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Telemetry;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Threading;

    sealed class ApplicationInsightsTelemetryClient : ITelemetryClient
    {
        private readonly ApplicationInsightsTelemetryCore _core;

        public ApplicationInsightsTelemetryClient()
        {
            Contract.Ensures(null != _core);
            _core = new ApplicationInsightsTelemetryCore();
        }

        bool ITelemetryClient.IsActive
        {
            get
            {
                return _core.IsActive;
            }

            set
            {
                if (value != _core.IsActive)
                {
                    if (value)
                    {
                        _core.Activate();
                    }
                    else
                    {
                        _core.Deactivate();
                    }
                }
            }
        }

        void ITelemetryClient.ReportEvent(object eventData)
        {
            if (_core.IsActive)
                _core.ReportEvent(eventData);
        }
    }
}
