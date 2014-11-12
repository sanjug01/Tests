using System;

namespace RdClient.Shared.Models
{
    public class Desktop : ModelBase
    {
        private string _hostName;
        private Guid _credId;
        
        public string HostName
        {
            get { return _hostName; }
            set { SetProperty(ref _hostName, value);  }
        }

        public Guid CredentialId
        {            
            get { return _credId; }
            set { SetProperty(ref _credId, value); }
        }
    }
}
