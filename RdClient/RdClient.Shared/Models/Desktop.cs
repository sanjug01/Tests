using System;
using System.Runtime.Serialization;

namespace RdClient.Shared.Models
{
    [DataContract(IsReference = true)]
    public class Desktop : ModelBase
    {
        private string _hostName;
        private Guid _credId;
        
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
    }
}
