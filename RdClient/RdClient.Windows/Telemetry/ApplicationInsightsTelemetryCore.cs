namespace RdClient.Telemetry
{
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using System;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    /// <summary>
    /// Core of the Application Insights telemetry client. The core may be safely passed to
    /// child objects created by the implementation of ITelemetryClient.
    /// </summary>
    sealed internal class ApplicationInsightsTelemetryCore
    {
        private TelemetryClient _client;

        public ApplicationInsightsTelemetryCore()
        {
            _client = new TelemetryClient();
        }

        public bool IsActive
        {
            get { return null != _client; }
        }

        public void Activate()
        {
            Contract.Assert(null == _client);
            _client = new TelemetryClient();
        }

        public void Deactivate()
        {
            Contract.Assert(null != _client);
            _client = null;
        }

        public void ReportEvent(object eventData)
        {
            TelemetryClient c = _client;

            if(null != c)
            {
                TypeInfo ti = eventData.GetType().GetTypeInfo();
                EventTelemetry et = new EventTelemetry(ti.Name);

                foreach (PropertyInfo pi in ti.DeclaredProperties)
                {
                    if (pi.CanRead)
                    {
                        Type pt = pi.PropertyType;
                        object v = pi.GetValue(eventData);

                        if (pt.Equals(typeof(int)) || pt.Equals(typeof(long)) || pt.Equals(typeof(double)) || pt.Equals(typeof(float)) || pt.Equals(typeof(bool)))
                        {
                            et.Metrics[pi.Name] = Convert.ToDouble(v);
                        }
                        else
                        {
                            et.Properties[pi.Name] = v.ToString();
                        }
                    }
                }

                c.TrackEvent(et);
            }
        }

        public void Event(string eventName)
        {
            if (null != _client)
            {
                _client.TrackEvent(eventName);
            }
        }

        public void Event(EventTelemetry eventTelemetry)
        {
            if(null != _client)
            {
                _client.TrackEvent(eventTelemetry);
            }
        }

        public void Metric(string metricName, double metricValue)
        {
            if (null != _client)
            {
                _client.TrackMetric(metricName, metricValue);
            }
        }

        public void Duration(string eventName, long milliseconds)
        {
            if (null != _client)
            {
                _client.TrackMetric(eventName, milliseconds / 60000);
            }
        }
    }
}
