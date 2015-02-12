namespace RdClient.Shared.Navigation.Extensions
{
    using RdClient.Shared.Telemetry;
    using System.Diagnostics;

    public sealed class DummyApplicationTelemetry : IApplicationTelemetry
    {
        private sealed class TelemetryActivity : ITelemetryActivity
        {
            private readonly string _name;

            public TelemetryActivity(string name)
            {
                _name = name;
            }

            void ITelemetryActivity.Complete()
            {
                Debug.WriteLine("DummyApplicationTelemetry.TelemetryActivity|Complete|{0}", _name);
            }
        }

        void IApplicationTelemetry.Event(string eventPath)
        {
            Debug.WriteLine("DummyApplicationTelemetry|Event|{0}", eventPath);
        }

        void IApplicationTelemetry.Metric(string name, double value)
        {
            Debug.WriteLine("DummyApplicationTelemetry|Metric|{0}={1}", name, value);
        }

        ITelemetryActivity IApplicationTelemetry.PresentView(string name)
        {
            return new TelemetryActivity(name);
        }
    }
}
