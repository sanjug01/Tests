﻿namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using RdClient.Shared.Telemetry;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    public sealed class SettingsViewModel : ViewModelBase, ISettingsViewModel, IDialogViewModel, ITelemetryClientSite 
    {
        private readonly RelayCommand _goBackCommand;
        private readonly RelayCommand _editUserCommand;
        private readonly RelayCommand _addUserCommand;        
        private readonly RelayCommand _editGatewayCommand;
        private readonly RelayCommand _addGatewayCommand;
        private GeneralSettings _generalSettings;
        private ReadOnlyObservableCollection<UserComboBoxElement> _users;
        private UserComboBoxElement _selectedUser;
        private ReadOnlyObservableCollection<GatewayComboBoxElement> _gateways;
        private GatewayComboBoxElement _selectedGateway;
        private bool _oldSendFeedback;
        private ITelemetryClient _telemetryClient;        

        public SettingsViewModel()
        {
            _goBackCommand = new RelayCommand(o => this.GoBackCommandExecute());
            _editUserCommand = new RelayCommand(o => this.EditUserCommandExecute(), o => { return this.UserCommandsEnabled(); });
            _addUserCommand = new RelayCommand(o => this.AddUserCommandExecute());
            _editGatewayCommand = new RelayCommand(o => this.EditGatewayCommandExecute(), o => { return this.GatewayCommandsEnabled(); });            
            _addGatewayCommand = new RelayCommand(o => this.AddGatewayCommandExecute());
        }

        public ICommand Cancel { get { return _goBackCommand; } }
        //Implement IDialogViewModel. Do nothing when enter is pressed
        public ICommand DefaultAction { get { return new RelayCommand(o => { }); } }
        public ICommand EditUser { get { return _editUserCommand; } }
        public ICommand AddUser { get { return _addUserCommand; } }
        public ICommand EditGateway { get { return _editGatewayCommand; } }
        public ICommand AddGateway { get { return _addGatewayCommand; } }

        public GeneralSettings GeneralSettings
        {
            get { return _generalSettings; }
            private set { SetProperty(ref _generalSettings, value); }
        }

        void ITelemetryClientSite.SetTelemetryClient(ITelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        public ReadOnlyObservableCollection<UserComboBoxElement> Users
        {
            get { return _users; }
            private set { SetProperty(ref _users, value); }
        }

        public UserComboBoxElement SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (SetProperty(ref _selectedUser, value))
                {
                    _editUserCommand.EmitCanExecuteChanged();
                    if (value != null && value.UserComboBoxType == UserComboBoxType.AddNew)
                    {
                        this.AddUser.Execute(null);
                    }
                }
            }
        }

        public ReadOnlyObservableCollection<GatewayComboBoxElement> Gateways
        {
            get { return _gateways; }
            private set { SetProperty(ref _gateways, value); }
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

        protected override void OnPresenting(object activationParameter)
        {
            this.GeneralSettings = this.ApplicationDataModel.Settings;
            _oldSendFeedback = this.GeneralSettings.SendFeedback;

            IOrderedObservableCollection<GatewayComboBoxElement> orderedGateways = OrderedObservableCollection<GatewayComboBoxElement>.Create(
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
            orderedGateways.Order = new GatewayComboBoxOrder();
            this.Gateways = orderedGateways.Models;

            IOrderedObservableCollection<UserComboBoxElement> orderedUsers = OrderedObservableCollection<UserComboBoxElement>.Create(
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
            orderedUsers.Order = new UserComboBoxOrder();
            this.Users = orderedUsers.Models;

        }

        protected override void OnDismissed()
        {
            //
            // If the "UseThumbnails" setting is off, delete all cached thumbnails.
            //
            if (!_generalSettings.UseThumbnails)
            {
                foreach (IModelContainer<RemoteConnectionModel> c in this.ApplicationDataModel.LocalWorkspace.Connections.Models)
                {
                    c.Model.EncodedThumbnail = null;
                }
            }

            if (null != _telemetryClient)
            {
                if(this.GeneralSettings.SendFeedback != _oldSendFeedback)
                {
                    //
                    // Activate telemetry for sending the event.
                    //
                    _telemetryClient.IsActive = true;
                    _telemetryClient.ReportEvent(new Telemetry.Events.SendUsage() { sendTelemetry = this.GeneralSettings.SendFeedback });
                }

                _telemetryClient.IsActive = this.GeneralSettings.SendFeedback;
            }

            this.GeneralSettings = null;
            this.Users = null;
            this.SelectedUser = null;
            this.Gateways = null;
            this.SelectedGateway = null;

            base.OnDismissed();
        }

        protected override void OnNavigatingBack(IBackCommandArgs backArgs)
        {
            this.Cancel.Execute(null);
            backArgs.Handled = true;
        }

        private void GoBackCommandExecute()
        {
            this.DismissModal(null);
        }

        private bool GatewayCommandsEnabled()
        {            
            return this.SelectedGateway?.Gateway?.Model != null;
        }

        private void EditGatewayCommandExecute()
        {
            if (GatewayCommandsEnabled())
            {
                var args = new EditGatewayViewModelArgs(this.SelectedGateway.Gateway);
                ReadOnlyObservableCollection<GatewayComboBoxElement> gatewaysUnhooked = this.Gateways;
                this.Gateways = null;
                this.NavigationService.PushAccessoryView("AddOrEditGatewayView", args, new PresentationCompletion((v, p) => { this.Gateways = gatewaysUnhooked; }));
            }
        }

        private void AddGatewayCommandExecute()
        {
            var args = new AddGatewayViewModelArgs();
            ReadOnlyObservableCollection<GatewayComboBoxElement> gatewaysUnhooked = this.Gateways;
            this.Gateways = null;
            this.NavigationService.PushAccessoryView("AddOrEditGatewayView", args, new PresentationCompletion((v, p) => { this.Gateways = gatewaysUnhooked; }));
        }

        private bool UserCommandsEnabled()
        {
            return this.SelectedUser?.Credentials?.Model != null;
        }

        private void EditUserCommandExecute()
        {
            if (UserCommandsEnabled())
            {
                var args = AddOrEditUserViewArgs.EditUser(this.SelectedUser.Credentials);
                ReadOnlyObservableCollection<UserComboBoxElement> usersUnhooked = this.Users;
                this.Users = null;
                this.NavigationService.PushAccessoryView("AddOrEditUserView", args, new PresentationCompletion((v, p) => { this.Users = usersUnhooked; }));
            }
        }

        private void AddUserCommandExecute()
        {
            var creds = new CredentialsModel() { Username = "", Password = "" };
            var args = AddOrEditUserViewArgs.AddUser();
            ReadOnlyObservableCollection<UserComboBoxElement> usersUnhooked = this.Users;
            this.Users = null;
            this.NavigationService.PushAccessoryView("AddOrEditUserView", args, new PresentationCompletion((v, p) => { this.Users = usersUnhooked; }));
        }
    }
}
