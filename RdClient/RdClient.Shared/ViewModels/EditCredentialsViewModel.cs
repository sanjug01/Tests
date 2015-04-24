namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Navigation;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    /// <summary>
    /// Extension for INavigationService that presents the EditCredentialsView modally with
    /// a presentation task.
    /// </summary>
    public static class EditCredentialsExtension
    {
        public static void EditCredentials(this INavigationService navigationService, IEditCredentialsTask task)
        {
            Contract.Assert(null != navigationService, "EditCredentials|cannot present with null navigation service");
            Contract.Requires(null != task);

            navigationService.PushModalView("EditCredentialsView", task,
                new ModalPresentationCompletion((sender, e) => { }));
        }
    }

    public sealed class EditCredentialsViewModel : ViewModelBase, IEditCredentialsViewModel, IEditCredentialsViewControl
    {
        private readonly RelayCommand _cancel;
        private readonly RelayCommand _dismiss;
        private readonly RelayCommand _confirm;
        private readonly RelayCommand _cancelConfirmation;

        private string _resourceName;
        private string _prompt;
        private string _dismissLabel;
        private bool _saveCredentials;
        private bool _canSaveCredentials;
        private bool _canRevealPassword;
        private string _userName;
        private string _password;
        private bool _canDismiss;
        private bool _confirmationVisible;
        private EditCredentialsConfirmation _confirmationMessage;
        private IEditCredentialsTask _task;
        private object _taskToken;

        public static readonly string
            UserNamePropertyName = "UserName",
            PasswordPropertyName = "Password";

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
                    bool canDismiss = _task.ValidateChangedProperty(_taskToken, UserNamePropertyName);

                    if (canDismiss)
                        canDismiss = _task.Validate(_taskToken);

                    this.InternalCanDismiss = canDismiss;
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
                    bool canDismiss = _task.ValidateChangedProperty(_taskToken, PasswordPropertyName);

                    if (canDismiss)
                        canDismiss = _task.Validate(_taskToken);

                    this.InternalCanDismiss = canDismiss;
                    //
                    // Enable the "reveal password" button if the password box is cleared completely,
                    // after which any password will have to be entered by user.
                    //
                    if (string.IsNullOrEmpty(value))
                        this.CanRevealPassword = true;
                }
            }
        }

        public bool IsConfirmationVisible
        {
            get { return _confirmationVisible; }
            private set { this.SetProperty(ref _confirmationVisible, value); }
        }

        public EditCredentialsConfirmation ConfirmationMessage
        {
            get { return _confirmationMessage; }
            private set { this.SetProperty(ref _confirmationMessage, value); }
        }

        void IEditCredentialsViewControl.Submit()
        {
            Contract.Ensures(null == _task);
            Contract.Ensures(null == _taskToken);

            if (null != _task)
            {
                _task.Dismissed(_taskToken);
                this.DismissModal(null);
            }
        }

        void IEditCredentialsViewControl.AskConfirmation(EditCredentialsConfirmation message)
        {
            this.ConfirmationMessage = message;
            this.IsConfirmationVisible = true;
        }

        public EditCredentialsViewModel()
        {
            _cancel = new RelayCommand(this.InternalCancel);
            _dismiss = new RelayCommand(this.InternalDismiss, p => this.InternalCanDismiss);
            _confirm = new RelayCommand(this.InternalConfirm);
            _cancelConfirmation = new RelayCommand(this.InternalCancelConformation);
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(null != activationParameter, "EditCredentialsViewModel|presented with a null parameter");
            Contract.Assert(null == _task, "EditCredentialsViewModel|presented without dismissing previous task");
            Contract.Ensures(null != _task);
            _task = activationParameter as IEditCredentialsTask;
            Contract.Assert(null != _task, string.Format("EditCredentialsViewModel|presented with an invalid parameter|{0}", activationParameter));

            _taskToken = _task.Presenting(this, this);
            _task.Populate(_taskToken);
            this.InternalCanDismiss = _task.Validate(_taskToken);

            this.CanRevealPassword = string.IsNullOrEmpty(_password);
        }

        protected override void OnDismissed()
        {
            Contract.Assert(null != _task);
            _task = null;
            _taskToken = null;
        }

        protected override void OnNavigatingBack(IBackCommandArgs backArgs)
        {
            this.Cancel.Execute(null);
            backArgs.Handled = true;
        }

        private void InternalCancel(object parameter)
        {
            Contract.Assert(null != _task, "EditCredentialsViewModel.CancelView|cancelled without task");
            //TODO: Bug 2555470 - If task.Cancelled() is called before DismissModal() it causes a crash. 
            // _task and _taskToken must be saved to local variables as they are set to null in DismissModal()
            IEditCredentialsTask task = _task;
            object taskToken = _taskToken;
            this.DismissModal(null);
            task.Cancelled(taskToken);            
        }

        private void InternalDismiss(object parameter)
        {
            Contract.Assert(null != _task, "EditCredentialsViewModel.DismissView|dismissed without task");
            _task.Dismissing(_taskToken);
        }

        private bool InternalCanDismiss
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
            this.IsConfirmationVisible = false;
            this.CastAndCall<IEditCredentialsViewControl>(ctl => ctl.Submit());
            }

        private void InternalCancelConformation(object parameter)
        {
            this.IsConfirmationVisible = false;
        }
    }
}
