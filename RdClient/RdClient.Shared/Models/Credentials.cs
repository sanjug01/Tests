using System.Runtime.Serialization;

namespace RdClient.Shared.Models
{
    [DataContract]
    public class Credentials : ModelBase
    {
        private string _username;
        private string _domain;
        private string _password;
        private bool _haveBeenPersisted;

        [DataMember]
        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        [DataMember]
        public string Domain
        {
            get { return _domain; }
            set { SetProperty(ref _domain, value); }
        }

        [DataMember]
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        [DataMember]
        public bool HaveBeenPersisted
        {
            get { return _haveBeenPersisted; }
            set { SetProperty(ref _haveBeenPersisted, value); }
        }
    }
}
