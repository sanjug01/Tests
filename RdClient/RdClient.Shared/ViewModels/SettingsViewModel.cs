using RdClient.Shared.Data;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using RdClient.Shared.Navigation.Extensions;
using RdClient.Shared.Telemetry;

namespace RdClient.Shared.ViewModels
{
    public sealed class SettingsViewModel : ViewModelBase, ISettingsViewModel, IDialogViewModel, ITelemetryClientSite 
    {
        private readonly RelayCommand _goBackCommand;
        private readonly RelayCommand _deleteUserCommand;
        private readonly RelayCommand _editUserCommand;
        private readonly RelayCommand _addUserCommand;
        private readonly RelayCommand _deleteGatewayCommand;
        private readonly RelayCommand _editGatewayCommand;
        private readonly RelayCommand _addGatewayCommand;
        private GeneralSettings _generalSettings;
        private ReadOnlyObservableCollection<UserComboBoxElement> _users;
        private UserComboBoxElement _selectedUser;
        private ReadOnlyObservableCollection<GatewayComboBoxElement> _gateways;
        private GatewayComboBoxElement _selectedGateway;
        ITelemetryClient _telemetryClient;

        public SettingsViewModel()
        {
            _goBackCommand = new RelayCommand(o => this.GoBackCommandExecute());
            _deleteUserCommand = new RelayCommand(o => this.DeleteUserCommandExecute(), o => { return this.UserCommandsEnabled(); });
            _editUserCommand = new RelayCommand(o => this.EditUserCommandExecute(), o => { return this.UserCommandsEnabled(); });
            _addUserCommand = new RelayCommand(o => this.AddUserCommandExecute());
            _deleteGatewayCommand = new RelayCommand(o => this.DeleteGatewayCommandExecute(), o => { return this.GatewayCommandsEnabled(); });
            _editGatewayCommand = new RelayCommand(o => this.EditGatewayCommandExecute(), o => { return this.GatewayCommandsEnabled(); });
            _addGatewayCommand = new RelayCommand(o => this.AddGatewayCommandExecute());
        }

        public ICommand Cancel { get { return _goBackCommand; } }
        //Implement IDialogViewModel. Do nothing when enter is pressed
        public ICommand DefaultAction { get { return new RelayCommand(o => { }); } }
        public ICommand DeleteUser { get { return _deleteUserCommand; } }
        public ICommand EditUser { get { return _editUserCommand; } }
        public ICommand AddUser { get { return _addUserCommand; } }
        public ICommand DeleteGateway { get { return _deleteGatewayCommand; } }
        public ICommand EditGateway { get { return _editGatewayCommand; } }
        public ICommand AddGateway { get { return _addGatewayCommand; } }

        public GeneralSettings GeneralSettings
        {
            get { return _generalSettings; }
            private set { SetProperty(ref _generalSettings, value); }
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
                    _deleteUserCommand.EmitCanExecuteChanged();
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
                    _deleteGatewayCommand.EmitCanExecuteChanged();
                    if (value != null && value.GatewayComboBoxType == GatewayComboBoxType.AddNew)
                    {
                        this.AddGateway.Execute(null);
                    }
                }
            }
        }

        protected override void OnPresenting(object activationParameter)
        {
            this.GeneralSettings = this.ApplicationDataModel.Settings;

            this.Gateways = TransformingObservableCollection<IModelContainer<GatewayModel>, GatewayComboBoxElement>.Create(
                this.ApplicationDataModel.Gateways.Models,
                g => new GatewayComboBoxElement(GatewayComboBoxType.Gateway, g),
                gcbe =>
                {
                    if (this.SelectedGateway == gcbe)
                    {
                        this.SelectedGateway = null;
                    }
                });

            this.Users = TransformingObservableCollection<IModelContainer<CredentialsModel>, UserComboBoxElement>.Create(
                this.ApplicationDataModel.Credentials.Models,
                c => new UserComboBoxElement(UserComboBoxType.Credentials, c),
                ucbe =>
                {
                    if (this.SelectedUser == ucbe)
                    {
                        this.SelectedUser = null;
                    }
                });
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

            if(null != _telemetryClient)
                _telemetryClient.IsActive = this.GeneralSettings.SendFeedback;

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

                // edit can also indicate deletion of the selected Gateway
                var editGatewayCompleted = new ModalPresentationCompletion((s, e) =>
                {
                    GatewayPromptResult result = e.Result as GatewayPromptResult;
                    if (null != result && result.Deleted)
                    {
                        this.DeleteGateway.Execute(null);
                    }
                });
                this.NavigationService.PushAccessoryView("AddOrEditGatewayView", args, editGatewayCompleted);
            }
        }

        private void DeleteGatewayCommandExecute()
        {
            if (GatewayCommandsEnabled())
            {
                this.ApplicationDataModel.Gateways.RemoveModel(this.SelectedGateway.Gateway.Id);
            }
        }

        private void AddGatewayCommandExecute()
        {
            var args = new AddGatewayViewModelArgs();
            this.NavigationService.PushAccessoryView("AddOrEditGatewayView", args);
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
                this.NavigationService.PushAccessoryView("AddOrEditUserView", args);
            }
        }

        private void DeleteUserCommandExecute()
        {
            if (UserCommandsEnabled())
            {
                this.ApplicationDataModel.Credentials.RemoveModel(this.SelectedUser.Credentials.Id);                
            }
        }

        private void AddUserCommandExecute()
        {
            var creds = new CredentialsModel() { Username = "", Password = "" };
            var args = AddOrEditUserViewArgs.AddUser();
            this.NavigationService.PushAccessoryView("AddOrEditUserView", args);
        }

        void ITelemetryClientSite.SetTelemetryClient(ITelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }
    }
}
