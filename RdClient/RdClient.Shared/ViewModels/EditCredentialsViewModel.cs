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
                new ModalPresentationCompletion((sender, e) =>
                {
                    if (null != e.Result)
                        task.Completed(e.Result);
                    else
                        task.Cancelled();
                }));
        }
    }

    public sealed class EditCredentialsViewModel : ViewModelBase, IEditCredentialsViewModel
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
        private IEditCredentialsTask _task;

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
            private set { this.SetProperty(ref _resourceName, value); }
        }

        public string Prompt
        {
            get { return _prompt; }
            private set { this.SetProperty(ref _prompt, value); }
        }

        public string DismissLabel
        {
            get { return _dismissLabel; }
            private set { this.SetProperty(ref _dismissLabel, value); }
        }

        public bool SaveCredentials
        {
            get { return _saveCredentials; }
            set { this.SetProperty(ref _saveCredentials, value); }
        }

        public bool CanSaveCredentials
        {
            get { return _canSaveCredentials; }
            private set { this.SetProperty(ref _canSaveCredentials, value); }
        }

        public bool CanRevealPassword
        {
            get { return _canRevealPassword; }
            private set { this.SetProperty(ref _canRevealPassword, value); }
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

        public EditCredentialsViewModel()
        {
            _cancel = new RelayCommand(this.CancelView);
            _dismiss = new RelayCommand(this.DismissView, p => this.CanDismiss);
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(null != activationParameter, "EditCredentialsViewModel|presented with a null parameter");
            Contract.Assert(null == _task, "EditCredentialsViewModel|presented without dismissing previous task");
            Contract.Ensures(null != _task);
            _task = activationParameter as IEditCredentialsTask;
            Contract.Assert(null != _task, string.Format("EditCredentialsViewModel|presented with an invalid parameter|{0}", activationParameter));
        }

        protected override void OnDismissed()
        {
            Contract.Assert(null != _task, "EditCredentialsViewModel|dismissed without presenting");
            Contract.Ensures(null == _task);
            _task = null;
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
