namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ValidationRules;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows.Input;

    public class AddGatewayViewModelArgs
    {
        public AddGatewayViewModelArgs()
        {
        }    
    }

    public class EditGatewayViewModelArgs
    {
        private readonly IModelContainer<GatewayModel> _gateway;

        public IModelContainer<GatewayModel> Gateway { get { return _gateway; } }

        public EditGatewayViewModelArgs(IModelContainer<GatewayModel> gatewayContainer)
        {
            _gateway = gatewayContainer;
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

    public class AddOrEditGatewayViewModel : ViewModelBase, IDialogViewModel, IUsersCollector
    {
        private IValidatedProperty<string> _host;
        private bool _isAddingGateway;
    
        private readonly RelayCommand _saveCommand;
        private readonly RelayCommand _cancelCommand;
        private readonly RelayCommand _deleteCommand;
        private readonly RelayCommand _addUserCommand;

        private GatewayModel _gateway;
        private ReadOnlyObservableCollection<UserComboBoxElement> _users;
        private UserComboBoxElement _selectedUser;

        public AddOrEditGatewayViewModel()
        {
            _saveCommand = new RelayCommand(o => SaveCommandExecute(), o => SaveCommandCanExecute());
            _cancelCommand = new RelayCommand(CancelCommandExecute);
            _deleteCommand = new RelayCommand(DeleteCommandExecute, p => !this.IsAddingGateway);
            _addUserCommand = new RelayCommand(LaunchAddUserView);       
        }

        public bool IsAddingGateway
        {
            get { return _isAddingGateway; }
            private set 
            {
                this.SetProperty(ref _isAddingGateway, value, "IsAddingGateway");
            }
        }

        public ReadOnlyObservableCollection<UserComboBoxElement> Users
        {
            get { return _users; }
            private set { SetProperty(ref _users, value); }
        }

        public UserComboBoxElement SelectedUser
        {
            get { return _selectedUser; }
            set { SetProperty(ref _selectedUser, value); }             
        }

        public IPresentableView PresentableView { private get; set; }

        public ICommand DefaultAction { get { return _saveCommand; } }

        public ICommand Cancel { get { return _cancelCommand; } }

        public ICommand Delete { get { return _deleteCommand; } }

        public ICommand AddUser { get { return _addUserCommand; } }

        // edit not supported, but may be in the future
        public ICommand EditUser { get { throw new NotImplementedException(); } }

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

            // gateway id is needed for validation for the edit dialog.
            Guid gatewayId = (editArgs != null) ? editArgs.Gateway.Id : Guid.Empty;

            List<IValidationRule<string>> validationRules;
            validationRules = new List<IValidationRule<string>>();
            validationRules.Add(new HostnameValidationRule());
            validationRules.Add(
                new NotDuplicateValidationRule<GatewayModel>(
                    this.ApplicationDataModel.Gateways, 
                    gatewayId,
                    new GatewayEqualityComparer(),
                    HostnameValidationFailure.DuplicateGateway
                    )
                );
            _host = new ValidatedProperty<string>(new CompositeValidationRule<string>(validationRules));
            _host.PropertyChanged += (s, e) =>
            {
                if (s == _host && e.PropertyName == "State")
                {
                    _saveCommand.EmitCanExecuteChanged();
                }
            };

            if (editArgs != null)
            {
                this.Gateway = editArgs.Gateway.Model;
                this.Host.Value = this.Gateway.HostName;
                this.IsAddingGateway = false;
            }
            else if (addArgs != null)
            {
                this.Host.Value = string.Empty;
                this.Gateway = new GatewayModel();
                this.IsAddingGateway = true;
            }

            // initialize users colection
            UserComboBoxElement defaultUser = new UserComboBoxElement(UserComboBoxType.AskEveryTime);
            JoinedObservableCollection<UserComboBoxElement> userCollection = JoinedObservableCollection<UserComboBoxElement>.Create();
            userCollection.AddCollection(new ObservableCollection<UserComboBoxElement>() { defaultUser });
            userCollection.AddCollection(
                TransformingObservableCollection<IModelContainer<CredentialsModel>, UserComboBoxElement>.Create(
                    this.ApplicationDataModel.Credentials.Models,
                    c => new UserComboBoxElement(UserComboBoxType.Credentials, c),
                    ucbe =>
                    {
                        if (this.SelectedUser == ucbe)
                        {
                            this.SelectedUser = null;
                        }
                    })
                );
            IOrderedObservableCollection<UserComboBoxElement> orderedUsers = OrderedObservableCollection<UserComboBoxElement>.Create(userCollection);
            orderedUsers.Order = new UserComboBoxOrder();
            this.Users = orderedUsers.Models;

            this.SelectUserId(this.Gateway.CredentialsId);
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

                if (null != this.SelectedUser
                    && UserComboBoxType.Credentials == this.SelectedUser.UserComboBoxType)
                {
                    this.Gateway.CredentialsId = this.SelectedUser.Credentials.Id;
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

        /// <summary>
        /// change user selection without saving to the desktop instance.
        /// </summary>
        /// <param name="userId"> user id for the selected user </param>
        private void SelectUserId(Guid userId)
        {
            var selected = this.Users.FirstOrDefault(cbe =>
            {
                return cbe.UserComboBoxType == UserComboBoxType.Credentials && cbe.Credentials?.Id == userId;
            });

            this.SelectedUser = selected?? this.Users[0];
        }

        private void LaunchAddUserView(object o)
        {
            AddOrEditUserViewArgs args = new AddOrEditUserViewArgs(new CredentialsModel(), false);
            ModalPresentationCompletion addUserCompleted = new ModalPresentationCompletion(CredentialPromptResultHandler);
            NavigationService.PushAccessoryView("AddOrEditUserView", args, addUserCompleted);
        }

        private void CredentialPromptResultHandler(object sender, PresentationCompletionEventArgs args)
        {
            CredentialPromptResult result = args.Result as CredentialPromptResult;

            if (result != null && !result.UserCancelled)
            {
                Guid credId = this.ApplicationDataModel.Credentials.AddNewModel(result.Credentials);
                this.SelectUserId(credId);
            }
        }

    }
}
