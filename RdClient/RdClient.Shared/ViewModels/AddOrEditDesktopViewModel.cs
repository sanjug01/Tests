namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ValidationRules;
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows.Input;

    public class AddDesktopViewModelArgs
    {
        public AddDesktopViewModelArgs()
        {
        }    
    }

    public class EditDesktopViewModelArgs
    {
        private readonly DesktopModel _desktop;

        public DesktopModel Desktop { get {return _desktop; } }

        public EditDesktopViewModelArgs(DesktopModel desktop)
        {
            _desktop = desktop;
        }
    }

    public class AddOrEditDesktopViewModel : ViewModelBase, IAddOrEditDesktopViewModel, IDialogViewModel
    {
        private bool _isAddingDesktop;
        private bool _isExpandedView;
        private string _friendlyName;
        private bool _isSwapMouseButtons;
        private bool _isUseAdminSession;
        private int _audioMode;

        private readonly RelayCommand _saveCommand;
        private readonly RelayCommand _cancelCommand;
        private readonly RelayCommand _showDetailsCommand;
        private readonly RelayCommand _hideDetailsCommand;
        private readonly RelayCommand _addUserCommand;
        private readonly RelayCommand _editUserCommand;
        private readonly RelayCommand _addGatewayCommand;
        private readonly RelayCommand _editGatewayCommand;
        private readonly ValidatedProperty<string> _host;
        private DesktopModel _desktop;

        private ReadOnlyObservableCollection<UserComboBoxElement> _users;
        private ReadOnlyObservableCollection<GatewayComboBoxElement> _gateways;
        private UserComboBoxElement _selectedUser;
        private GatewayComboBoxElement _selectedGateway;
        private Guid _selectedUserId;

        public AddOrEditDesktopViewModel()
        {
            _saveCommand = new RelayCommand(o => SaveCommandExecute(), o => SaveCommandCanExecute());
            _cancelCommand = new RelayCommand(CancelCommandExecute);

            _showDetailsCommand = new RelayCommand((o) => { this.IsExpandedView = true; });
            _hideDetailsCommand = new RelayCommand((o) => { this.IsExpandedView = false; });

            _addUserCommand = new RelayCommand(AddUserCommandExecute);
            _editUserCommand = new RelayCommand(EditUserCommandExecute, EditUserCommandCanExecute);

            _addGatewayCommand = new RelayCommand(AddGatewayCommandExecute);            
            _editGatewayCommand = new RelayCommand(EditGatewayCommandExecute, EditGatewayCommandCanExecute);

            _host = new ValidatedProperty<string>(new HostnameValidationRule());
            _host.PropertyChanged += (s, e) =>
            {
                if (s == _host && e.PropertyName == "State")
                {
                    _saveCommand.EmitCanExecuteChanged();
                }
            };

            this.IsExpandedView = false;
        }

        public ReadOnlyObservableCollection<UserComboBoxElement> Users
        {
            get { return _users; }
            private set { SetProperty(ref _users, value); }
        }
        public ReadOnlyObservableCollection<GatewayComboBoxElement> Gateways
        {
            get { return _gateways; }
            private set { SetProperty(ref _gateways, value); }
        }

        public IPresentableView PresentableView { private get; set; }
        public ICommand DefaultAction { get { return _saveCommand; } }
        public ICommand Cancel { get { return _cancelCommand; } }
        public ICommand AddUser { get { return _addUserCommand; } }
        public ICommand EditUser { get { return _editUserCommand; } }
        public ICommand AddGateway { get { return _addGatewayCommand; } }
        public ICommand EditGateway { get { return _editGatewayCommand; } }
        public ICommand ShowDetailsCommand { get { return _showDetailsCommand; } }
        public ICommand HideDetailsCommand { get { return _hideDetailsCommand; } }

        public bool IsAddingDesktop
        {
            get { return _isAddingDesktop; }
            private set
            {
                this.SetProperty(ref _isAddingDesktop, value, "IsAddingDesktop");
            }
        }
        
        public UserComboBoxElement SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (SetProperty(ref _selectedUser, value))
                {
                    _editUserCommand.EmitCanExecuteChanged();
                }
            }
        }

        public GatewayComboBoxElement SelectedGateway
        {
            get { return _selectedGateway; }
            set
            {
                if (SetProperty(ref _selectedGateway, value))
                {
                    _editGatewayCommand.EmitCanExecuteChanged();
                }
            }
        }

        public DesktopModel Desktop
        {
            get { return _desktop; }
            private set { this.SetProperty(ref _desktop, value); }
        }

        public IValidatedProperty<string> Host
        {
            get { return _host; }
        }

        public bool IsExpandedView
        {
            get { return _isExpandedView; }
            set { SetProperty(ref _isExpandedView, value); }
        }

        public string FriendlyName
        {
            get { return _friendlyName; }
            set { SetProperty(ref _friendlyName, value); }
        }

        public bool IsUseAdminSession
        {
            get { return _isUseAdminSession; }
            set { SetProperty(ref _isUseAdminSession, value); }
        }

        public bool IsSwapMouseButtons
        {
            get { return _isSwapMouseButtons; }
            set { SetProperty(ref _isSwapMouseButtons, value); }
        }

        public int AudioMode
        {
            get { return _audioMode; }
            set { SetProperty(ref _audioMode, value); }
        }

        private bool SaveCommandCanExecute()
        {
            return this.Host.State.Status != ValidationResultStatus.Empty;
        }

        private void SaveCommandExecute()
        {
            if (this.Host.State.Status == ValidationResultStatus.Valid)
            {
                this.Desktop.HostName = this.Host.Value;
                this.Desktop.FriendlyName = this.FriendlyName;
                this.Desktop.IsAdminSession = this.IsUseAdminSession;
                this.Desktop.IsSwapMouseButtons = this.IsSwapMouseButtons;
                this.Desktop.AudioMode = (AudioMode) this.AudioMode;

                if (null != this.SelectedUser 
                    && UserComboBoxType.Credentials == this.SelectedUser.UserComboBoxType)
                {
                    this.Desktop.CredentialsId = this.SelectedUser.Credentials.Id;
                }
                else
                {
                    this.Desktop.CredentialsId = Guid.Empty;
                }

                if (null != this.SelectedGateway
                    && GatewayComboBoxType.Gateway == this.SelectedGateway.GatewayComboBoxType)
                {
                    this.Desktop.GatewayId = this.SelectedGateway.Gateway.Id;
                }
                else
                {
                    this.Desktop.GatewayId = Guid.Empty;
                }

                if (this.IsAddingDesktop)
                {
                    this.ApplicationDataModel.LocalWorkspace.Connections.AddNewModel(this.Desktop);
                }

                this.DismissModal(null);
            }
        }

        private void CancelCommandExecute(object o)
        {
            this.DismissModal(null);
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(null != activationParameter);

            AddDesktopViewModelArgs addArgs = activationParameter as AddDesktopViewModelArgs;
            EditDesktopViewModelArgs editArgs = activationParameter as EditDesktopViewModelArgs;

            if (editArgs != null)
            {
                this.Desktop = editArgs.Desktop;
                this.Host.Value = this.Desktop.HostName;
                this.FriendlyName = this.Desktop.FriendlyName;
                this.AudioMode = (int) this.Desktop.AudioMode;
                this.IsSwapMouseButtons = this.Desktop.IsSwapMouseButtons;
                this.IsUseAdminSession = this.Desktop.IsAdminSession;

                this.IsAddingDesktop = false;
            }
            else if(addArgs != null)
            {
                this.Desktop = new DesktopModel();
                this.Host.Value = string.Empty;
                this.FriendlyName = string.Empty;
                this.AudioMode = (int)RdClient.Shared.Models.AudioMode.Local;
                this.IsSwapMouseButtons = false;
                this.IsUseAdminSession = false;

                this.IsAddingDesktop = true;
            }
            this.IsExpandedView = false;

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

            this.SelectUserId(this.Desktop.CredentialsId);

            // initialize gateways colection
            GatewayComboBoxElement defaultGateway = new GatewayComboBoxElement(GatewayComboBoxType.None);
            JoinedObservableCollection<GatewayComboBoxElement> gatewayCollection = JoinedObservableCollection<GatewayComboBoxElement>.Create();
            gatewayCollection.AddCollection(new ObservableCollection<GatewayComboBoxElement>() { defaultGateway });
            gatewayCollection.AddCollection(
                TransformingObservableCollection<IModelContainer<GatewayModel>, GatewayComboBoxElement>.Create(
                    this.ApplicationDataModel.Gateways.Models,
                    g => new GatewayComboBoxElement(GatewayComboBoxType.Gateway, g),
                    gcbe =>
                    {
                        if (this.SelectedGateway == gcbe)
                        {
                            this.SelectedGateway = null;
                        }
                    })
                );

            IOrderedObservableCollection<GatewayComboBoxElement> orderedGateways = OrderedObservableCollection<GatewayComboBoxElement>.Create(gatewayCollection);
            orderedGateways.Order = new GatewayComboBoxOrder();
            this.Gateways = orderedGateways.Models;

            this.SelectGatewayId(this.Desktop.GatewayId);
        }

        private void AddGatewayCommandExecute(object o)
        {
            AddGatewayViewModelArgs args = new AddGatewayViewModelArgs();
            ModalPresentationCompletion addGatewayCompleted = new ModalPresentationCompletion(AddGatewayPromptResultHandler);            
            // save user selection
            _selectedUserId = (null != this.SelectedUser && UserComboBoxType.Credentials == this.SelectedUser.UserComboBoxType) ? this.SelectedUser.Credentials.Id : Guid.Empty;
            NavigationService.PushAccessoryView("AddOrEditGatewayView", args, addGatewayCompleted);
        }

        private void EditGatewayCommandExecute(object o)
        {
            EditGatewayViewModelArgs args = new EditGatewayViewModelArgs(this.SelectedGateway.Gateway.Model);
            ModalPresentationCompletion editGatewayCompleted = new ModalPresentationCompletion(EditGatewayPromptResultHandler);
            // save user selection
            _selectedUserId = (null != this.SelectedUser && UserComboBoxType.Credentials == this.SelectedUser.UserComboBoxType) ? this.SelectedUser.Credentials.Id : Guid.Empty;
            this.NavigationService.PushAccessoryView("AddOrEditGatewayView", args, editGatewayCompleted);
        }

        private bool EditGatewayCommandCanExecute(object o)
        {
            return this.SelectedGateway?.Gateway?.Model != null;
        }

        private void AddUserCommandExecute(object o)
        {
            AddUserViewArgs args = new AddUserViewArgs(new CredentialsModel(), false);
            ModalPresentationCompletion addUserCompleted = new ModalPresentationCompletion(AddCredentialPromptResultHandler);
            NavigationService.PushAccessoryView("AddUserView", args, addUserCompleted);
        }

        private void EditUserCommandExecute(object o)
        {
            AddUserViewArgs args = new AddUserViewArgs(this.SelectedUser.Credentials.Model, false, CredentialPromptMode.EditCredentials);            
            ModalPresentationCompletion editUserCompleted = new ModalPresentationCompletion(EditGatewayPromptResultHandler);
            this.NavigationService.PushAccessoryView("AddUserView", args, editUserCompleted);
        }

        private bool EditUserCommandCanExecute(object o)
        {
            return this.SelectedUser?.Credentials?.Model != null;
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

            this.SelectedUser = (null != selected) ? selected : this.Users[0];
        }

        /// <summary>
        /// change gateway selection without saving to the desktop instance.
        /// </summary>
        /// <param name="gatewayId"> gateway id for the selected gateway </param>
        private void SelectGatewayId(Guid gatewayId)
        {
            var selected = this.Gateways.FirstOrDefault(cbe =>
            {
                return cbe.GatewayComboBoxType == GatewayComboBoxType.Gateway && cbe.Gateway?.Id == gatewayId;
            });

            this.SelectedGateway = (null != selected) ? selected : this.Gateways[0];
        }

        private void AddCredentialPromptResultHandler(object sender, PresentationCompletionEventArgs args)
        {
            CredentialPromptResult result = args.Result as CredentialPromptResult;

            if (result != null && !result.UserCancelled)
            {
                Guid credId = this.ApplicationDataModel.Credentials.AddNewModel(result.Credentials);
                this.SelectUserId(credId);
            }

        }
        private void EditCredentialPromptResultHandler(object sender, PresentationCompletionEventArgs args)
        {
            CredentialPromptResult result = args.Result as CredentialPromptResult;
            if (result != null && !result.UserCancelled)
            {
                Guid credId = this.SelectedUser.Credentials.Id;
                this.SelectUserId(credId);
            }
        }

        private void AddGatewayPromptResultHandler(object sender, PresentationCompletionEventArgs args)
        {
            GatewayPromptResult result = args.Result as GatewayPromptResult;
            if (result != null && !result.UserCancelled)
            {
                this.SelectGatewayId(result.GatewayId);
            }

            // restore user selection in case user collection has been changed
            this.SelectedUser = null;
            this.SelectUserId(_selectedUserId);
        }
        private void EditGatewayPromptResultHandler(object sender, PresentationCompletionEventArgs args)
        {

            GatewayPromptResult result = args.Result as GatewayPromptResult;
            if (result != null && !result.UserCancelled)
            {
                Guid gatewayId = this.SelectedGateway.Gateway.Id;
                this.SelectGatewayId(gatewayId);
            }

            // restore user selection in case user collection has been changed
            this.SelectedUser = null;
            this.SelectUserId(_selectedUserId);
        }

    }
}
