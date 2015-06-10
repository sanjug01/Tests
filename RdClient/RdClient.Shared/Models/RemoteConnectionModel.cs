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

        /// <summary>
        /// Create an RDP connection object specific to the connection model.
        /// </summary>
        /// <param name="connectionFactory">RDP connection factory one of whose methods the method must call
        /// to properly create and initialize a new RDP connection.</param>
        /// <returns>An RDP connection attached to the rendering panel.</returns>
        public abstract IRdpConnection CreateConnection(IRdpConnectionFactory connectionFactory, IRenderingPanel renderingPanel);

        /// <summary>
        /// Create a telemetry event object and populate it with information about the connection model.
        /// </summary>
        /// <param name="telemetryClient">Telemetry client object used to create the session telemetry event.</param>
        /// <param name="telemetryEventName">Name of the telemetry event.</param>
        /// <returns>Telemetry event object that will be retained by the session and reported after the session will have disconnected.</returns>
        public abstract ITelemetryEvent CreateSessionTelemetry(ApplicationDataModel dataModel, ITelemetryClient telemetryClient, string telemetryEventName);

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
