using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.ValidationRules;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public enum CredentialPromptMode
    {
        EnterCredentials,
        InvalidCredentials,
        FreshCredentialsNeeded
    }

    public sealed class CredentialPromptResult
    {
        public CredentialPromptResult(Credentials credential, bool save, bool userCancelled)
        {
            this.Credential = credential;
            this.Save = save;
            this.UserCancelled = userCancelled;
        }

        public Credentials Credential { get; private set; }

        public bool Save { get; private set; }

        public bool UserCancelled { get; private set; }
    }

    public class AddUserViewArgs
    {
        private readonly CredentialPromptMode _mode;
        private readonly Credentials _cred;
        private readonly bool _showSave;

        public AddUserViewArgs(Credentials credential, bool showSave, CredentialPromptMode mode = CredentialPromptMode.EnterCredentials)
        {
            _cred = credential;
            _showSave = showSave;
            _mode = mode;
        }

        public bool ShowSave { get { return _showSave; } }

        public Credentials User { get { return _cred; } }

        public CredentialPromptMode Mode { get { return _mode; } }
    }

    public class AddUserViewModel : ViewModelBase
    {
        private AddUserViewArgs _args;
        private bool _storeCredentials;
        private string _user;
        private string _password;
        private readonly RelayCommand _okCommand;
        private readonly RelayCommand _cancelCommand;
        private CredentialPromptMode _mode;

        public AddUserViewModel()
        {
            _okCommand = new RelayCommand(new Action<object>(OkCommandHandler), (o) => {
                return 
                    (this.User != null) && 
                    (this.Password != null) && 
                    (this.IsUsernameValid) && 
                    (this.Password.Length > 0);
            } );
            _cancelCommand = new RelayCommand(new Action<object>(CancelCommandHandler));
        }

        public IPresentableView PresentableView { private get; set; }

        public bool ShowSave { get; private set; }

        public ICommand OkCommand { get { return _okCommand; } }

        public ICommand CancelCommand { get { return _cancelCommand; } }

        public CredentialPromptMode Mode
        {
            get { return _mode; }
            set { SetProperty(ref _mode, value); }
        }

        public bool StoreCredentials
        {
            get { return _storeCredentials; }
            set { SetProperty(ref _storeCredentials, value); }
        }

        public bool IsUsernameValid
        {
            get
            {
                UsernameValidationRule rule = new UsernameValidationRule();
                return rule.Validate(this.User, CultureInfo.CurrentCulture);
            }
        }

        public string User
        {
            get { return _user; }
            set
            {
                SetProperty(ref _user, value);
                this.EmitPropertyChanged("IsUsernameValid");
                _okCommand.EmitCanExecuteChanged();
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                SetProperty(ref _password, value);
                _okCommand.EmitCanExecuteChanged();
            }
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(null != activationParameter as AddUserViewArgs);
            _args = activationParameter as AddUserViewArgs;
            this.ShowSave = _args.ShowSave;
            this.User = _args.User.Username;
            this.Password = _args.User.Password;
            this.Mode = _args.Mode;
        }

        private void OkCommandHandler(object o)
        {
            _args.User.Username = this.User;
            _args.User.Password = this.Password;
            DismissModal(new CredentialPromptResult(_args.User, this.StoreCredentials, false));
        }

        private void CancelCommandHandler(object o)
        {
            DismissModal(new CredentialPromptResult(null, false, true));
        }
    }
}
