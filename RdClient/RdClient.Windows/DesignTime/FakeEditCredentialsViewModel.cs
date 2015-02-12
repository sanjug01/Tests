namespace RdClient.DesignTime
{
    using RdClient.Shared.ViewModels;
    using System.Windows.Input;

    sealed class FakeEditCredentialsViewModel : ViewModelBase, IEditCredentialsViewModel
    {
        private readonly RelayCommand _cancel;
        private readonly RelayCommand _dismiss;

        private string _resourceName;
        private string _prompt;
        private string _dismissLabel;
        private bool _saveCredentials;
        private bool _canSaveCredentials;
        private bool _canRevealPassword;
        private string _userName;
        private string _password;
        private bool _canDismiss;

        public ICommand Cancel
        {
            get { return _cancel; }
        }

        public ICommand Dismiss
        {
            get { return _dismiss; }
        }

        public string ResourceName
        {
            get { return _resourceName; }
            set { this.SetProperty(ref _resourceName, value); }
        }

        public string Prompt
        {
            get { return _prompt; }
            set { this.SetProperty(ref _prompt, value); }
        }

        public string DismissLabel
        {
            get { return _dismissLabel; }
            set { this.SetProperty(ref _dismissLabel, value); }
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

        public FakeEditCredentialsViewModel()
        {
            _cancel = new RelayCommand(this.CancelView);
            _dismiss = new RelayCommand(this.DismissView, p => this.CanDismiss);
            _canDismiss = true;
            _canRevealPassword = true;
            _canSaveCredentials = true;
            _resourceName = "d:Resource Name";
            _prompt = "d:Prompt";
            _dismissLabel = "d:Dismiss";
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
    }
}
