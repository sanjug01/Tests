﻿namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
    using System.Runtime.Serialization;

    [DataContract(IsReference=true)]
    public abstract class RemoteConnectionModel : SerializableModel
    {
        [DataMember(Name = "Thumbnail", IsRequired = false, EmitDefaultValue = false)]
        private ThumbnailModel _thumbnail;

        private ICertificateTrust _certificateTrust;

        protected RemoteConnectionModel()
        {
        }

        public ThumbnailModel Thumbnail
        {
            get
            {
                if (null == _thumbnail)
                    _thumbnail = new ThumbnailModel();
                return _thumbnail;
            }
        }

        public ICertificateTrust CertificateTrust
        {
            get { return _certificateTrust; }
            set { this.SetProperty(ref _certificateTrust, value); }
        }
    }
}
