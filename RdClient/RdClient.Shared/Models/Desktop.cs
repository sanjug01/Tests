namespace RdClient.Shared.Models
{
    using System;
    using System.Runtime.Serialization;

    [DataContract(IsReference = true)]
    public sealed class Desktop : RemoteResource
    {
        private string _hostName;
        private Guid _credId;

        public Desktop(Workspace parentWorkspace)
            : base(parentWorkspace)
        {
        }
        
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
    }
}
