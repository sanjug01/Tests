namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;

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

        [OnSerializing]
        public void OnSerializing(StreamingContext context)
        {
            Contract.Assert(null != _scrambler);

            if (null == _password)
                _scrambledPassword = null;
            else
                _scrambledPassword = _scrambler.Scramble(_password);
        }

        /// <summary>
        /// Called immediately after loading the model from storage.
        /// </summary>
        /// <param name="scrambler">Data scrambler object used to scramble the password before saving it to the storage.</param>
        public void SetScrambler(IDataScrambler scrambler)
        {
            Contract.Assert(null != scrambler);

            if (!object.ReferenceEquals(_scrambler, scrambler))
            {
                _scrambler = scrambler;

                if (null != _scrambledPassword)
                {
                    Contract.Assert(null == _password);
                    try
                    {
                        _password = _scrambler.Unscramble(_scrambledPassword);
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine("Failed to unscramble the password for {0}|{1}", _username, ex);
                    }
                    _scrambledPassword = null;
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
            set { this.SetProperty(ref _password, value); }
        }
    }
}
