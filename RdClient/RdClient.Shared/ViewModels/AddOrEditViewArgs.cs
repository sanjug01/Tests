namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using System;

    public class AddOrEditUserViewArgs
    {
        private readonly CredentialPromptMode _mode;
        private readonly IModelContainer<CredentialsModel> _credentials;
        private readonly bool _showSave;
        private readonly bool _save;
        private readonly bool _canDelete;
        private readonly bool _showMessage;

        private AddOrEditUserViewArgs(IModelContainer<CredentialsModel> credentials, CredentialPromptMode mode, bool save, bool showSave, bool canDelete, bool showMessage)
        {
            _credentials = credentials;
            _showSave = showSave;
            _save = save;
            _mode = mode;
            _canDelete = canDelete;
            _showMessage = showMessage;
        }

        public static AddOrEditUserViewArgs AddUser()
        {
            var creds = TemporaryModelContainer<CredentialsModel>.WrapModel<CredentialsModel>(Guid.Empty, new CredentialsModel());
            creds.Model.Username = "";
            creds.Model.Password = "";
            return new AddOrEditUserViewArgs(
                credentials: creds,
                mode: CredentialPromptMode.EnterCredentials,
                save: true,
                showSave: false,
                canDelete: false,
                showMessage: false);
        }

        public static AddOrEditUserViewArgs EditUser(IModelContainer<CredentialsModel> creds)
        {
            return new AddOrEditUserViewArgs(
                credentials: creds,
                save: false,
                showSave: false,
                canDelete: true,
                showMessage: false,
                mode: CredentialPromptMode.EditCredentials);
        }

        public bool ShowSave { get { return _showSave; } }

        public bool Save { get { return _save; } }

        public bool CanDelete { get { return _canDelete; } }

        public bool ShowMessage { get { return _showMessage; } }

        public IModelContainer<CredentialsModel> Credentials { get { return _credentials; } }

        public CredentialPromptMode Mode { get { return _mode; } }
    }
}