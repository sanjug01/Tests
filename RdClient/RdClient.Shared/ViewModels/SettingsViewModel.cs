using RdClient.Shared.Data;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System;
using System.Collections.Generic;

namespace RdClient.Shared.ViewModels
{
    public sealed class SettingsViewModel : ViewModelBase, ISettingsViewModel, IDialogViewModel 
    {
        private readonly RelayCommand _goBackCommand;
        private readonly RelayCommand _deleteUserCommand;
        private readonly RelayCommand _editUserCommand;
        private readonly RelayCommand _addUserCommand;
        private readonly RelayCommand _deleteGatewayCommand;
        private readonly RelayCommand _editGatewayCommand;
        private readonly RelayCommand _addGatewayCommand;
        private GeneralSettings _generalSettings;
        private IList<UserComboBoxElement> _users;
        private UserComboBoxElement _selectedUser;
        private IList<GatewayComboBoxElement> _gateways;
        private GatewayComboBoxElement _selectedGateway;

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
        public ICommand DeleteUserCommand { get { return _deleteUserCommand; } }
        public ICommand EditUserCommand { get { return _editUserCommand; } }
        public ICommand AddUserCommand { get { return _addUserCommand; } }
        public ICommand DeleteGatewayCommand { get { return _deleteGatewayCommand; } }
        public ICommand EditGatewayCommand { get { return _editGatewayCommand; } }
        public ICommand AddGatewayCommand { get { return _addGatewayCommand; } }

        public GeneralSettings GeneralSettings
        {
            get { return _generalSettings; }
            private set { SetProperty(ref _generalSettings, value); }
        }

        public IList<UserComboBoxElement> Users
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
                        this.AddUserCommand.Execute(null);
                    }
                }
            }
        }

        public IList<GatewayComboBoxElement> Gateways
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
                        this.AddGatewayCommand.Execute(null);
                    }
                }
            }
        }

        protected override void OnPresenting(object activationParameter)
        {
            this.GeneralSettings = this.ApplicationDataModel.Settings;
            LoadUsers();
            LoadGateways();
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

        private void LoadGateways()
        {
            IList<GatewayComboBoxElement> gateways = new List<GatewayComboBoxElement>();
            // gateways.Add(new GatewayComboBoxElement(GatewayComboBoxType.AddNew));
            foreach (IModelContainer<GatewayModel> gatewayModel in this.ApplicationDataModel.Gateways.Models)
            {
                gateways.Add(new GatewayComboBoxElement(GatewayComboBoxType.Gateway, gatewayModel));
            }
            this.SelectedGateway = null;
            this.Gateways = gateways;
        }

        private bool GatewayCommandsEnabled()
        {            
            return this.SelectedGateway?.Gateway?.Model != null;
        }

        private void EditGatewayCommandExecute()
        {
            if (GatewayCommandsEnabled())
            {
                var args = new EditGatewayViewModelArgs(this.SelectedGateway.Gateway.Model);

                // edit can also indicate deletion of the selected Gateway
                var editGatewayCompleted = new ModalPresentationCompletion((s, e) =>
                {
                    GatewayPromptResult result = e.Result as GatewayPromptResult;
                    if (result.Deleted)
                    {
                        this.DeleteGatewayCommand.Execute(null);
                    }
                    else
                    {
                        LoadGateways();
                        LoadUsers();
                    }
                });
                this.NavigationService.PushAccessoryView("AddOrEditGatewayView", args, editGatewayCompleted);
            }
        }

        private void DeleteGatewayCommandExecute()
        {
            if (GatewayCommandsEnabled())
            {
                this.NavigationService.PushAccessoryView("DeleteGatewayView", this.SelectedGateway.Gateway, new ModalPresentationCompletion((s, e) => LoadGateways()));
            }
        }

        private void AddGatewayCommandExecute()
        {
            var args = new AddGatewayViewModelArgs();
            var completed = new ModalPresentationCompletion((s, e) =>
            {
                LoadGateways();
                LoadUsers();
            });
            this.NavigationService.PushAccessoryView("AddOrEditGatewayView", args, completed);
        }

        private void LoadUsers()
        {
            IList<UserComboBoxElement> users = new List<UserComboBoxElement>();
            // users.Add(new UserComboBoxElement(UserComboBoxType.AddNew));
            foreach (IModelContainer<CredentialsModel> credModel in this.ApplicationDataModel.Credentials.Models)
            {
                users.Add(new UserComboBoxElement(UserComboBoxType.Credentials, credModel));
            }
            this.SelectedUser = null;
            this.Users = users;
        }

        private bool UserCommandsEnabled()
        {
            return this.SelectedUser?.Credentials?.Model != null;
        }

        private void EditUserCommandExecute()
        {
            if (UserCommandsEnabled())
            {
                var args = new AddUserViewArgs(this.SelectedUser.Credentials.Model, false, CredentialPromptMode.EditCredentials);
                ModalPresentationCompletion editUserCompleted = new ModalPresentationCompletion((s, e) =>
                {
                    CredentialPromptResult result = e.Result as CredentialPromptResult;
                    if (result != null && result.Deleted)
                    {
                        this.DeleteUserCommand.Execute(null);
                    }
                    else
                    {
                        LoadUsers();
                    }
                });
                this.NavigationService.PushAccessoryView("AddUserView", args, editUserCompleted);
            }
        }

        private void DeleteUserCommandExecute()
        {
            if (UserCommandsEnabled())
            {
                this.NavigationService.PushAccessoryView("DeleteUserView", this.SelectedUser.Credentials, new ModalPresentationCompletion((s, e) => LoadUsers()));
            }
        }

        private void AddUserCommandExecute()
        {
            var creds = new CredentialsModel() { Username = "", Password = "" };
            var args = new AddUserViewArgs(creds, false, CredentialPromptMode.EnterCredentials);
            ModalPresentationCompletion addUserCompleted = new ModalPresentationCompletion((s, e) =>
            {
                CredentialPromptResult result = e.Result as CredentialPromptResult;
                if (result != null && !result.UserCancelled)
                {
                    this.ApplicationDataModel.Credentials.AddNewModel(result.Credentials);                    
                }
                LoadUsers();
            });
            this.NavigationService.PushAccessoryView("AddUserView", args, addUserCompleted);
        }
    }
}
