namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Data;
    using RdClient.Shared.Telemetry;
    using System;
    using System.Runtime.Serialization;

    [DataContract(IsReference=true)]
    public abstract class RemoteConnectionModel : SerializableModel
    {
        [DataMember(Name = "EncodedThumbnail", IsRequired = false, EmitDefaultValue = false)]
        private byte[] _encodedThumbnail;

        protected RemoteConnectionModel()
        {
        }

        /// <summary>
        /// Encoded thumbnail image that can be converted to a XAML image source. The property is updated on the UI thread by sessions
        /// created from the model object.
        /// </summary>
        public byte[] EncodedThumbnail
        {
            get { return _encodedThumbnail; }
            set { this.SetProperty(ref _encodedThumbnail, value); }
        }

        public string TelemetrySourceType
        {
            get { return this.GetTelemetrySourceType(); }
        }

        /// <summary>
        /// Create an RDP connection object specific to the connection model.
        /// </summary>
        /// <param name="connectionFactory">RDP connection factory one of whose methods the method must call
        /// to properly create and initialize a new RDP connection.</param>
        /// <returns>An RDP connection attached to the rendering panel.</returns>
        public abstract IRdpConnection CreateConnection(IRdpConnectionFactory connectionFactory, IRenderingPanel renderingPanel);

        /// <summary>
        /// Populate a telemetry event with initial session information.
        /// </summary>
        /// <param name="dataModel">Application data model object.</param>
        /// <param name="telemetryEvent">Telemetry event to be populated with metrics and tags.</param>
        public abstract void InitializeSessionTelemetry(ApplicationDataModel dataModel, ITelemetryEvent telemetryEvent);

        protected abstract string GetTelemetrySourceType();

        protected static string GetHostAddressTypeTag(string hostString)
        {
            string tag = "unknown";

            switch(Uri.CheckHostName(hostString))
            {
                case UriHostNameType.IPv4:
                    tag = "ipv4";
                    break;

                case UriHostNameType.IPv6:
                    tag = "ipv6";
                    break;

                case UriHostNameType.Dns:
                    tag = GetDomainNameTypeTag(hostString.Trim());
                    break;
            }

            return tag;
        }

        private static string GetDomainNameTypeTag(string domainName)
        {
            //
            // Just looking for a dot is probably too primitive but it should cover most of the cases.
            //
            return domainName.IndexOf('.') >= 0 ? "fqdn" : "shortname";
        }
    }
}
