namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
    using System.Runtime.Serialization;
    using System.Windows.Input;

    [DataContract(IsReference = true)]
    public class CredentialsModel : SerializableModel
    {
        private string _username;
        private string _domain;
        private string _password;

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
    }
}
