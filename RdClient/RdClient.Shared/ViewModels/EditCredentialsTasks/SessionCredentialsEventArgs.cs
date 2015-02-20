namespace RdClient.Shared.ViewModels.EditCredentialsTasks
{
    using RdClient.Shared.Models;
    using System;

    public sealed class SessionCredentialsEventArgs : EventArgs
    {
        private readonly CredentialsModel _credentials;
        private readonly Guid _credentialsId;
        private readonly bool _passwordChanged;
        private readonly bool _userWantsToSavePassword;

        public CredentialsModel Credentials
        {
            get { return _credentials; }
        }

        public Guid CredentialsId
        {
            get { return _credentialsId; }
        }

        public bool IsPasswordChanged
        {
            get { return _passwordChanged; }
        }

        public bool UserWantsToSavePassword
        {
            get { return _userWantsToSavePassword; }
        }

        public SessionCredentialsEventArgs(CredentialsModel credentials, Guid credentialsId, bool passwordChanged, bool userWantsToSavePassword)
        {
            _credentials = credentials;
            _credentialsId = credentialsId;
            _passwordChanged = passwordChanged;
            _userWantsToSavePassword = userWantsToSavePassword;
        }
    }
}
