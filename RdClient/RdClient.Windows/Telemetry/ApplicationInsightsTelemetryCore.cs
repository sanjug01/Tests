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

                foreach(FieldInfo fi in ti.DeclaredFields)
                {
                    if (fi.IsPublic)
                    {
                        object v = fi.GetValue(eventData);

                        if (null != v)
                        {
                            AddValue(et, fi.Name, fi.FieldType, v);
                        }
                    }
                }

                foreach (PropertyInfo pi in ti.DeclaredProperties)
                {
                    if (pi.CanRead)
                    {
                        object v = pi.GetValue(eventData);

                        if (null != v)
                        {
                            AddValue(et, pi.Name, pi.PropertyType, v);
                        }
                    }
                }

                c.TrackEvent(et);
            }
        }

        private void AddValue(EventTelemetry et, string name, Type type, object value)
        {
            if (type.Equals(typeof(int)) || type.Equals(typeof(long)) || type.Equals(typeof(double)) || type.Equals(typeof(float)) || type.Equals(typeof(bool)))
            {
                et.Metrics[name] = Convert.ToDouble(value);
            }
            else
            {
                et.Properties[name] = value.ToString();
            }
        }
    }
}
