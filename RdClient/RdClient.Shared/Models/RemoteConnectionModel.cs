namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Data;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
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
    }
}
