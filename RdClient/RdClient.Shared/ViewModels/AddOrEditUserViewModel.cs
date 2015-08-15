namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ValidationRules;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    public class AddOrEditUserViewModel : ViewModelBase, IDialogViewModel
    {
        private readonly RelayCommand _okCommand;
        private readonly RelayCommand _cancelCommand;
        private readonly RelayCommand _deleteCommand;

        private CredentialPromptMode _mode;
        private bool _showSave;
        private bool _storeCredentials;
        private bool _showMessage;
        private bool _canDelete;
        private IValidatedProperty<string> _user;
        private string _password;
        private IModelContainer<CredentialsModel> _creds;


        public AddOrEditUserViewModel()
        {
            _okCommand = new RelayCommand(o => OkCommandHandler(), o => OkCommandIsEnabled());
            _cancelCommand = new RelayCommand(new Action<object>(CancelCommandHandler));
            _deleteCommand = new RelayCommand(new Action<object>(DeleteCommandHandler), p => this.CanDelete );
        }

        public ICommand DefaultAction { get { return _okCommand; } }

        public ICommand Cancel { get { return _cancelCommand; } }

        public ICommand Delete { get { return _deleteCommand; } }

        public CredentialPromptMode Mode
        {
            get { return _mode; }
            private set { SetProperty(ref _mode, value); }
        }

        public bool ShowSave
        {
            get { return _showSave; }
            private set { SetProperty(ref _showSave, value); }
        }

        public bool ShowMessage
        {
            get { return _showMessage; }
            private set { SetProperty(ref _showMessage, value); }
        }

        public bool CanDelete
        {
            get { return _canDelete; }
            private set { SetProperty(ref _canDelete, value); }
        }

        public bool StoreCredentials
        {
            get { return _storeCredentials; }
            set { SetProperty(ref _storeCredentials, value); }
        }

        public IValidatedProperty<string> User
        {
            get { return _user; }
            private set { SetProperty(ref _user, value); }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                SetProperty(ref _password, value);
            }
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(activationParameter is AddOrEditUserViewArgs);
            var args = activationParameter as AddOrEditUserViewArgs;
            
            this.Mode = args.Mode;
            this.ShowMessage = args.ShowMessage;
            this.ShowSave = args.ShowSave;
            this.StoreCredentials = args.Save;            
            this.ShowMessage = args.ShowMessage;
            this.CanDelete = args.CanDelete;
            _creds = args.Credentials;
            
            var usernameRules = new List<IValidationRule<string>>();
            usernameRules.Add(new UsernameFormatValidationRule());
            usernameRules.Add(
                new NotDuplicateValidationRule<CredentialsModel>(
                    this.ApplicationDataModel.Credentials,
                    _creds.Id,
                    new CredentialsEqualityComparer(),
                    UsernameValidationFailure.Duplicate));
            this.User = new ValidatedProperty<string>(new CompositeValidationRule<string>(usernameRules));
            this.User.PropertyChanged += (s, e) =>
            {
                if (s == _user && e.PropertyName == "State")
                {
                    _okCommand.EmitCanExecuteChanged();
                }
            };
            this.User.Value = _creds.Model.Username;
            this.Password = _creds.Model.Password;
        }

        protected override void OnDismissed()
        {
            this.User = null;
            this.Password = null;

            base.OnDismissed();
        }

        private bool OkCommandIsEnabled()
        {
            return this.User.State.Status != ValidationResultStatus.Empty;
        }

        private void OkCommandHandler()
        {
            this.User.Value = this.User.Value.Trim();
            if (this.User.State.Status == ValidationResultStatus.Valid)
            {
                _creds.Model.Username = this.User.Value;
                _creds.Model.Password = this.Password;
                if (this.StoreCredentials && !this.ApplicationDataModel.Credentials.HasModel(_creds.Id))
                {
                    Guid id = this.ApplicationDataModel.Credentials.AddNewModel(_creds.Model);
                    _creds = TemporaryModelContainer<CredentialsModel>.WrapModel(id, _creds.Model);
                }
                DismissModal(CredentialPromptResult.CreateWithCredentials(_creds, this.StoreCredentials));
            }
        }

        private void CancelCommandHandler(object o)
        {
            DismissModal(CredentialPromptResult.CreateCancelled());
        }

        private void DeleteCommandHandler(object o)
        {
            if (this.CanDelete)
            {
                ApplicationDataModel.Credentials.RemoveModel(_creds.Id);
                DismissModal(CredentialPromptResult.CreateDeleted());
            }            
        }
    }
}
