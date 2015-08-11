namespace RdClient.Telemetry
{
    using RdClient.Shared.Telemetry;
    using System;
    using System.Diagnostics;

    sealed class DummyTelemetryClient : ITelemetryClient
    {
        private bool _isActive;

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
                        TurnOn();
                    else
                        TurnOff();
                }
            }
        }

        void ITelemetryClient.ReportEvent(object eventData)
        {
            if (_isActive)
                Debug.WriteLine("DummyTelemetryClient|ReportEvent{0}", eventData);
        }

        private void TurnOn()
        {
            Debug.WriteLine("DummyTelemetryClient|TurnOn");
        }

        private void TurnOff()
        {
            Debug.WriteLine("DummyTelemetryClient|TurnOff");
        }
    }
}
