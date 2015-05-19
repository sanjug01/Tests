namespace RdClient.Telemetry
{
    using RdClient.Shared.Telemetry;
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
