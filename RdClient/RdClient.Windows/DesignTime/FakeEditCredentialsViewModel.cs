namespace RdClient.DesignTime
{
    using RdClient.Shared.ViewModels;
    using System.Windows.Input;
    using RdClient.Shared.Navigation;
    using System;

    sealed class FakeEditCredentialsViewModel : ViewModelBase, IEditCredentialsViewModel
    {
        private readonly RelayCommand _cancel;
        private readonly RelayCommand _dismiss;
        private readonly RelayCommand _confirm;
        private readonly RelayCommand _cancelConfirmation;

        private string _resourceName;
        private CredentialPromptMode _promptMode;
        private bool _saveCredentials;
        private bool _canSaveCredentials;
        private bool _canRevealPassword;
        private string _userName;
        private string _password;
        private bool _canDismiss;
        private bool _showPrompt;
        private EditCredentialsConfirmation _confirmationMessage;

        public ICommand Cancel
        {
            get { return _cancel; }
        }

        public ICommand Dismiss
        {
            get { return _dismiss; }
        }

        public ICommand Confirm
        {
            get { return _confirm; }
        }

        public ICommand CancelConfirmation
        {
            get { return _cancelConfirmation; }
        }

        public string ResourceName
        {
            get { return _resourceName; }
            set { this.SetProperty(ref _resourceName, value); }
        }

        public CredentialPromptMode PromptMode
        {
            get { return _promptMode; }
            set { this.SetProperty(ref _promptMode, value); }
        }

        public bool SaveCredentials
        {
            get { return _saveCredentials; }
            set { this.SetProperty(ref _saveCredentials, value); }
        }

        public bool CanSaveCredentials
        {
            get { return _canSaveCredentials; }
            set { this.SetProperty(ref _canSaveCredentials, value); }
        }

        public bool CanRevealPassword
        {
            get { return _canRevealPassword; }
            set { this.SetProperty(ref _canRevealPassword, value); }
        }

        public bool ShowPrompt
        {
            get { return _showPrompt; }
            set { this.SetProperty(ref _showPrompt, value); }
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                if(this.SetProperty(ref _userName, value))
                {
                    //
                    // TODO: Perform validation;
                    //
                }
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (this.SetProperty(ref _password, value))
                {
                    //
                    // TODO: Perform validation;
                    //
                }
            }
        }

        public bool IsConfirmationVisible
        {
            get { return true; }
        }

        public EditCredentialsConfirmation ConfirmationMessage
        {
            get { return _confirmationMessage; }
            set { this.SetProperty(ref _confirmationMessage, value); }
        }

        public FakeEditCredentialsViewModel()
        {
            _cancel = new RelayCommand(this.CancelView);
            _dismiss = new RelayCommand(this.DismissView, p => this.CanDismiss);
            _confirm = new RelayCommand(this.InternalConfirm);
            _cancelConfirmation = new RelayCommand(this.InternalCancelConformation);
            _canDismiss = true;
            _canRevealPassword = true;
            _canSaveCredentials = true;
            _resourceName = "d:Resource Name";
            _promptMode = CredentialPromptMode.InvalidCredentials;
            _showPrompt = true;
        }

        protected override void OnPresenting(object activationParameter)
        {
        }

        protected override void OnDismissed()
        {
        }

        private void CancelView(object parameter)
        {
            this.DismissModal(null);
        }

        private void DismissView(object parameter)
        {
            this.DismissModal(null);
        }

        private bool CanDismiss
        {
            get { return _canDismiss; }
            set
            {
                if(this.SetProperty(ref _canDismiss, value))
                {
                    _dismiss.EmitCanExecuteChanged();
                }
            }
        }

        private void InternalConfirm(object parameter)
        {
            throw new NotImplementedException();
        }

        private void InternalCancelConformation(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
