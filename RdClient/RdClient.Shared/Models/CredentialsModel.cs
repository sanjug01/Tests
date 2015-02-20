namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;
    using System.Windows.Input;

    [DataContract(IsReference = true)]
    public class CredentialsModel : SerializableModel
    {
        [DataMember(Name = "Username")]
        private string _username;

        [DataMember(Name = "Password", IsRequired = false, EmitDefaultValue = false)]
        private string _password;

        public CredentialsModel()
        {
        }

        public CredentialsModel(CredentialsModel otherModel)
        {
            Contract.Assert(null != otherModel);

            _username = otherModel._username;
            _password = otherModel.Password;
        }

        public void CopyTo(CredentialsModel otherModel)
        {
            Contract.Requires(null != otherModel);
            otherModel.Username = _username;
            otherModel.Password = _password;
        }

        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }
    }
}
