﻿namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ValidationRules;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
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

        private CredentialPromptResult()
        {
            this.UserCancelled = true;
        }

        private CredentialPromptResult(CredentialsModel credentials, bool save)
        {
            this.Credentials = credentials;
            this.Save = save;
            this.UserCancelled = false;
        }

        public CredentialsModel Credentials { get; private set; }

        public bool Save { get; private set; }

        public bool UserCancelled { get; private set; }
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
        private readonly IValidatedProperty<string> _user;
        private string _password;
        private readonly RelayCommand _okCommand;
        private readonly RelayCommand _cancelCommand;
        private CredentialPromptMode _mode;
        private bool _showMessage;

        public AddUserViewModel()
        {
            var usernameRule = new CompositeValidationRule<string>(new List<IValidationRule<string>>() { new UsernameFormatValidationRule() });
            _user = new ValidatedProperty<string>(usernameRule);
            _okCommand = new RelayCommand(o => OkCommandHandler(o), o => !_user.ShowErrors || _user.State.IsValid);
            _user.PropertyChanged += (s, e) =>
            {
                if (s == _user && e.PropertyName == "State")
                {
                    _okCommand.EmitCanExecuteChanged();
                }
            };
            _cancelCommand = new RelayCommand(new Action<object>(CancelCommandHandler));
        }

        public IPresentableView PresentableView { private get; set; }

        public bool ShowSave { get; private set; }

        public ICommand DefaultAction { get { return _okCommand; } }

        public ICommand Cancel { get { return _cancelCommand; } }

        public CredentialPromptMode Mode
        {
            get { return _mode; }
            set
            {
                if (SetProperty(ref _mode, value))
                {
                    this.ShowMessage = this.Mode != CredentialPromptMode.EnterCredentials;
                }
            }
        }

        public bool ShowMessage
        {
            get { return _showMessage; }
            private set { SetProperty(ref _showMessage, value); }
        }

        public bool StoreCredentials
        {
            get { return _storeCredentials; }
            set { SetProperty(ref _storeCredentials, value); }
        }

        public IValidatedProperty<string> User
        {
            get { return _user; }
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

            this.ShowSave = _args.ShowSave;
            this.User.ShowErrors = false;
            this.User.Value = _args.Credentials.Username;
            this.Password = _args.Credentials.Password;
            this.Mode = _args.Mode;         
        }

        private void OkCommandHandler(object o)
        {
            this.User.ShowErrors = true;
            if (_okCommand.CanExecute(o))
            {
                _args.Credentials.Username = this.User.Value;
                _args.Credentials.Password = this.Password;
                DismissModal(CredentialPromptResult.CreateWithCredentials(_args.Credentials, this.StoreCredentials));
            }
        }

        private void CancelCommandHandler(object o)
        {
            DismissModal(CredentialPromptResult.CreateCancelled());
        }
    }
}
