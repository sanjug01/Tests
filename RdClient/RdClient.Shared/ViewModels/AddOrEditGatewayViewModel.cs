namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ValidationRules;
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    public class AddGatewayViewModelArgs
    {
        public AddGatewayViewModelArgs()
        {
        }    
    }

    public class EditGatewayViewModelArgs
    {
        private readonly GatewayModel _gateway;

        public GatewayModel Gateway { get { return _gateway; } }

        public EditGatewayViewModelArgs(GatewayModel gateway)
        {
            _gateway = gateway;
        }
    }

    public sealed class GatewayPromptResult
    {
        public static GatewayPromptResult CreateWithGateway(Guid gatewayId)
        {
            return new GatewayPromptResult(gatewayId);
        }

        public static GatewayPromptResult CreateCancelled()
        {
            return new GatewayPromptResult();
        }
        public static GatewayPromptResult CreateDeleted()
        {
            return new GatewayPromptResult() { Deleted = true } ;
        }

        private GatewayPromptResult()
        {
            this.UserCancelled = true;
            this.GatewayId = Guid.Empty;
            this.Deleted = false;
        }

        private GatewayPromptResult(Guid gatewayId)
        {
            this.GatewayId = gatewayId;
            this.UserCancelled = false;
            this.Deleted = false;
        }

        public Guid GatewayId { get; private set; }

        public bool UserCancelled { get; private set; }

        public bool Deleted { get; private set; }
    }

    public class AddOrEditGatewayViewModel : ViewModelBase, IDialogViewModel
    {
        private IValidatedProperty<string> _host;
        private bool _isAddingGateway;
    
        private readonly RelayCommand _saveCommand;
        private readonly RelayCommand _cancelCommand;
        private readonly RelayCommand _deleteCommand;
        private readonly RelayCommand _addUserCommand;

        private GatewayModel _gateway;
        private int _selectedUserOptionsIndex;

        public AddOrEditGatewayViewModel()
        {
            _saveCommand = new RelayCommand(o => SaveCommandExecute(), o => SaveCommandCanExecute());
            _cancelCommand = new RelayCommand(CancelCommandExecute);
            _deleteCommand = new RelayCommand(DeleteCommandExecute, p => !this.IsAddingGateway);
            _addUserCommand = new RelayCommand(LaunchAddUserView);

            _host = new ValidatedProperty<string>(new HostNameValidationRule());
            _host.PropertyChanged += (s, e) =>
            {
                if (s == _host && e.PropertyName == "State")
                {
                    _saveCommand.EmitCanExecuteChanged();
                }
            };

            this.UserOptions = new ObservableCollection<UserComboBoxElement>();
        }

        public bool IsAddingGateway
        {
            get { return _isAddingGateway; }
            private set 
            {
                this.SetProperty(ref _isAddingGateway, value, "IsAddingGateway");
            }
        }

        public ObservableCollection<UserComboBoxElement> UserOptions { get; set; }

        public int SelectedUserOptionsIndex 
        { 
            get { return _selectedUserOptionsIndex; }
            set { SetProperty(ref _selectedUserOptionsIndex, value); }
        }

        public IPresentableView PresentableView { private get; set; }

        public ICommand DefaultAction { get { return _saveCommand; } }

        public ICommand Cancel { get { return _cancelCommand; } }

        public ICommand Delete { get { return _deleteCommand; } }

        public ICommand AddUser { get { return _addUserCommand; } }

        public GatewayModel Gateway
        {
            get { return _gateway; }
            private set { this.SetProperty(ref _gateway, value); }
        }

        public IValidatedProperty<string> Host
        {
            get { return _host; }
            set { SetProperty(ref _host, value); }
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(null != activationParameter);

            AddGatewayViewModelArgs addArgs = activationParameter as AddGatewayViewModelArgs;
            EditGatewayViewModelArgs editArgs = activationParameter as EditGatewayViewModelArgs;

            if (editArgs != null)
            {
                this.Gateway = editArgs.Gateway;
                this.Host.Value = this.Gateway.HostName;
                this.IsAddingGateway = false;
            }
            else if (addArgs != null)
            {
                this.Host.Value = string.Empty;
                this.Gateway = new GatewayModel();
                this.IsAddingGateway = true;
            }

            Update();
        }

        private bool SaveCommandCanExecute()
        {
            return this.Host.State.Status != ValidationResultStatus.Empty;
        }

        private void SaveCommandExecute()
        {
            if (this.Host.State.Status == ValidationResultStatus.Valid)
            {
                this.Gateway.HostName = this.Host.Value;
                Guid gatewayId = Guid.Empty;

                if (null != this.UserOptions[this.SelectedUserOptionsIndex].Credentials)
                {
                    this.Gateway.CredentialsId = this.UserOptions[this.SelectedUserOptionsIndex].Credentials.Id;
                }
                else
                {
                    this.Gateway.CredentialsId = Guid.Empty;
                }

                if (this.IsAddingGateway)
                {
                    // returning gatewayId only if it is a new gateway
                    gatewayId = this.ApplicationDataModel.Gateways.AddNewModel(this.Gateway);
                }

                DismissModal(GatewayPromptResult.CreateWithGateway(gatewayId));
            }
        }

        private void CancelCommandExecute(object o)
        {
            DismissModal(GatewayPromptResult.CreateCancelled());
        }

        private void DeleteCommandExecute(object o)
        {
            // parent view should present the confirmation dialog and perform deletion
            DismissModal(GatewayPromptResult.CreateDeleted());
        }

        private void Update()
        {
            this.LoadUsers();
            this.SelectUserId(this.Gateway.CredentialsId);           
        }

        private void LoadUsers()
        {
            // load users list
            this.UserOptions.Clear();
            this.UserOptions.Add(new UserComboBoxElement(UserComboBoxType.AskEveryTime));

            foreach (IModelContainer<CredentialsModel> credentials in this.ApplicationDataModel.Credentials.Models)
            {
                this.UserOptions.Add(new UserComboBoxElement(UserComboBoxType.Credentials, credentials));
            }
        }

        /// <summary>
        /// change user selection without saving to the desktop instance.
        /// </summary>
        /// <param name="userId"> user id for the selected user </param>
        private void SelectUserId(Guid userId)
        {
            int idx = 0;
            if (Guid.Empty != userId)
            {
                for (idx = 0; idx < this.UserOptions.Count; idx++)
                {
                    if (this.UserOptions[idx].UserComboBoxType == UserComboBoxType.Credentials &&
                        this.UserOptions[idx].Credentials.Id == userId)
                        break;
                }

                if (idx == this.UserOptions.Count)
                {
                    idx = 0;
                }
            }

            this.SelectedUserOptionsIndex = idx;
        }

        private void LaunchAddUserView(object o)
        {
            AddUserViewArgs args = new AddUserViewArgs(new CredentialsModel(), false);
            ModalPresentationCompletion addUserCompleted = new ModalPresentationCompletion(CredentialPromptResultHandler);
            NavigationService.PushAccessoryView("AddUserView", args, addUserCompleted);
        }

        private void CredentialPromptResultHandler(object sender, PresentationCompletionEventArgs args)
        {
            CredentialPromptResult result = args.Result as CredentialPromptResult;

            if (result != null && !result.UserCancelled)
            {
                Guid credId = this.ApplicationDataModel.Credentials.AddNewModel(result.Credentials);
                LoadUsers();
                this.SelectUserId(credId);
            }
            else
            {
                this.Update();
            }
        }

    }
}
