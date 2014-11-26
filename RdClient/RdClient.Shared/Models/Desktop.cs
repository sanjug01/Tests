using System;
using System.Runtime.Serialization;

namespace RdClient.Shared.Models
{
    [DataContract(IsReference = true)]
    public class Desktop : ModelBase
    {
        private string _hostName;
        private Guid _credId;
        private Guid _thumbnailId;
        
        [DataMember]
        public string HostName
        {
            get { return _hostName; }
            set { SetProperty(ref _hostName, value);  }
        }

        [DataMember]
        public Guid CredentialId
        {            
            get { return _credId; }
            set { SetProperty(ref _credId, value); }
        }

        public bool HasCredential
        {
            get { return !this.CredentialId.Equals(Guid.Empty); }
        }

        [DataMember]
        public Guid ThumbnailId
        {
            get { return _thumbnailId; }
            set { SetProperty(ref _thumbnailId, value); }
        }

        public bool HasThumbnail
        {
            get { return !Guid.Empty.Equals(this.ThumbnailId); }
        }
    }
}
