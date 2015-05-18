namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ValidationRules;
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    public enum CredentialPromptMode
    {        
        EnterCredentials,
        EditCredentials,
        InvalidCredentials,
        FreshCredentialsNeeded
    }

    public sealed class CredentialPromptResult
    {
        public static CredentialPromptResult CreateWithCredentials(CredentialsModel credentials, bool save)
        {
            return new CredentialPromptResult(credentials, save);
        }

        public static CredentialPromptResult CreateCancelled()
        {
            return new CredentialPromptResult();
        }

        public static CredentialPromptResult CreateDeleted()
        {
            return new CredentialPromptResult() { Deleted = true };
        }

        private CredentialPromptResult()
        {
            this.UserCancelled = true;
            this.Deleted = false;
        }

        private CredentialPromptResult(CredentialsModel credentials, bool save)
        {
            this.Credentials = credentials;
            this.Save = save;
            this.UserCancelled = false;
            this.Deleted = false;
        }

        public CredentialsModel Credentials { get; private set; }

        public bool Save { get; private set; }

        public bool UserCancelled { get; private set; }

        public bool Deleted { get; private set; }
    }

    public class AddUserViewArgs
    {
        private readonly CredentialPromptMode _mode;
        private readonly CredentialsModel _credentials;
        private readonly bool _showSave;

        public AddUserViewArgs(CredentialsModel credentials, bool showSave, CredentialPromptMode mode = CredentialPromptMode.EnterCredentials)
        {
            _credentials = credentials;
            _showSave = showSave;
            _mode = mode;
        }

        public bool ShowSave { get { return _showSave; } }

        public CredentialsModel Credentials { get { return _credentials; } }

        public CredentialPromptMode Mode { get { return _mode; } }
    }

    public class AddUserViewModel : ViewModelBase, IDialogViewModel
    {
        private AddUserViewArgs _args;
        private bool _storeCredentials;
        private IValidatedProperty<string> _user;
        private string _password;
        private readonly RelayCommand _okCommand;
        private readonly RelayCommand _cancelCommand;
        private readonly RelayCommand _deleteCommand;
        private CredentialPromptMode _mode;
        private bool _showMessage;
        private bool _canDelete;

        public AddUserViewModel()
        {
            _okCommand = new RelayCommand(o => OkCommandHandler(o), o => OkCommandIsEnabled(o));
            _cancelCommand = new RelayCommand(new Action<object>(CancelCommandHandler));
            _deleteCommand = new RelayCommand(new Action<object>(DeleteCommandHandler), p => this.CanDelete );
        }

        public IPresentableView PresentableView { private get; set; }

        public bool ShowSave { get; private set; }

        public ICommand DefaultAction { get { return _okCommand; } }

        public ICommand Cancel { get { return _cancelCommand; } }

        public ICommand Delete { get { return _deleteCommand; } }

        public CredentialPromptMode Mode
        {
            get { return _mode; }
            private set
            {
                if (SetProperty(ref _mode, value))
                {
                    this.ShowMessage = this.Mode != CredentialPromptMode.EnterCredentials;
                    // Delete only makes sense if editing existing credentials
                    this.CanDelete = this.Mode == CredentialPromptMode.EditCredentials;
                }
            }
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
            Contract.Assert(activationParameter is AddUserViewArgs);

            _args = activationParameter as AddUserViewArgs;

            var usernameRule = new UsernameFormatValidationRule();

            this.Mode = _args.Mode;
            this.ShowSave = _args.ShowSave;
            this.User = new ValidatedProperty<string>(usernameRule, _args.Credentials.Username);
            this.Password = _args.Credentials.Password;            

            this.User.PropertyChanged += (s, e) =>
            {
                if (s == _user && e.PropertyName == "State")
                {
                    _okCommand.EmitCanExecuteChanged();
                }
            };
        }

        private void OkCommandHandler(object o)
        {
            if (_okCommand.CanExecute(o) && this.User.ValidateNow())
            {
                _args.Credentials.Username = this.User.Value;
                _args.Credentials.Password = this.Password;
                DismissModal(CredentialPromptResult.CreateWithCredentials(_args.Credentials, this.StoreCredentials));
            }
        }

        private bool OkCommandIsEnabled(object o)
        {
            return !string.IsNullOrWhiteSpace(this.User?.Value);
        }

        private void CancelCommandHandler(object o)
        {
            DismissModal(CredentialPromptResult.CreateCancelled());
        }

        private void DeleteCommandHandler(object o)
        {
            // parent view should present the confirmation dialog and perform deletion
            DismissModal(CredentialPromptResult.CreateDeleted());
        }
    }
}
