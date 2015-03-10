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

        public byte[] EncodedThumbnail
        {
            get { return _encodedThumbnail; }
            set { this.SetProperty(ref _encodedThumbnail, value); }
        }

        /// <summary>
        /// Set up the newly created IRdpConnection object.
        /// </summary>
        /// <param name="connection">New, unused RDP connection object created just before the call
        /// of SetUpConnection.</param>
        public abstract void SetUpConnection(IRdpProperties connectionProperties);
    }
}
