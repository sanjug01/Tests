namespace RdClient.Shared.Models
{
    using System;
    using System.Runtime.Serialization;

    [DataContract(IsReference = true)]
    public sealed class DesktopModel : RemoteConnectionModel
    {
        [DataMember(Name="HostName")]
        private string _hostName;

        [DataMember(Name="CredentialsId")]
        private Guid _credentialsId;

        public string HostName
        {
            get { return _hostName; }
            set { this.SetProperty(ref _hostName, value); }
        }

        public Guid CredentialsId
        {
            get { return _credentialsId; }
            set
            {
                if (this.SetProperty(ref _credentialsId, value))
                    EmitPropertyChanged("HasCredentials");
            }
        }

        public bool HasCredentials
        {
            get { return !Guid.Empty.Equals(_credentialsId); }
        }

        public DesktopModel()
        {
            _credentialsId = Guid.Empty;
        }
    }
}
