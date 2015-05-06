namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;
    using Windows.Security.Credentials;

    [DataContract(IsReference = true)]
    public class CredentialsModel : SerializableModel
    {
        private IDataScrambler _scrambler;

        [DataMember(Name = "Username")]
        private string _username;

        private string _password;

        [DataMember(Name ="ScrambledPassword", EmitDefaultValue = false, IsRequired = false)]
        private ScrambledString _scrambledPassword;

        public CredentialsModel()
        {
        }

        public CredentialsModel(CredentialsModel otherModel)
        {
            Contract.Assert(null != otherModel);

            _username = otherModel._username;
            this.Password = otherModel.Password;
        }

        public void SetScrambler(IDataScrambler scrambler)
        {
            Contract.Assert(null != scrambler);

            if (!object.ReferenceEquals(scrambler, _scrambler))
            {
                _scrambler = scrambler;

                if (null == _scrambledPassword)
                {
                    if (null != _password)
                        _scrambledPassword = _scrambler.Scramble(_password);
                }
                else
                {
                    _password = _scrambler.Unscramble(_scrambledPassword);
                }
            }
        }

        public void CopyTo(CredentialsModel otherModel)
        {
            Contract.Requires(null != otherModel);
            otherModel.Username = _username;
            otherModel.Password = _password;
            if (null != _scrambler)
            {
                //
                // SetScrambler requires a non-null scrambler
                //
                otherModel.SetScrambler(_scrambler);
            }
        }

        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if(SetProperty(ref _password, value) && null != _scrambler)
                {
                    if (null == value)
                    {
                        _scrambledPassword = null;
                    }
                    else
                    {
                        _scrambledPassword = _scrambler.Scramble(value);
                    }
                }
            }
        }
    }
}
