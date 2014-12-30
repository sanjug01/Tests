using RdClient.Shared.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public class SettingsViewModel : ViewModelBase, ISettingsViewModel 
    {
        private RelayCommand _goBackCommand;
        private RelayCommand _addUserCommand;
        private RelayCommand _editUserCommand;
        private RelayCommand _deleteUserCommand;
        private bool _showGeneralSettings;
        private bool _showGatewaySettings;
        private bool _showUserSettings;
        private GeneralSettings _generalSettings;
        private ObservableCollection<Credentials> _users;

        public SettingsViewModel()
        {
            _goBackCommand = new RelayCommand(this.GoBackCommandExecute);
            _editUserCommand = new RelayCommand(this.EditUserCommandExectute);
            _deleteUserCommand = new RelayCommand(this.DeleteUserCommandExectute);
            _addUserCommand = new RelayCommand(this.AddUserCommandExectute);
        }

        public ICommand GoBackCommand
        {
            get { return _goBackCommand; }
        }

        public ICommand AddUserCommand
        {
            get { return _addUserCommand; }
        }

        public bool ShowGeneralSettings
        {
            get { return _showGeneralSettings; }
            set { SetProperty(ref _showGeneralSettings, value); }
        }

        public bool ShowGatewaySettings
        {
            get { return _showGatewaySettings; }
            set { SetProperty(ref _showGatewaySettings, value); }
        }

        public bool ShowUserSettings
        {
            get { return _showUserSettings; }
            set { SetProperty(ref _showUserSettings, value); }
        }

        public GeneralSettings GeneralSettings
        {
            get { return _generalSettings; }
            private set { SetProperty(ref _generalSettings, value); }
        }

        public ObservableCollection<Credentials> Users
        {
            get { return _users; }
            private set { SetProperty(ref _users, value); }
        }

        protected override void OnPresenting(object activationParameter)
        {
            base.OnPresenting(activationParameter);
            this.ShowGeneralSettings = true;
            this.ShowGatewaySettings = false;
            this.ShowUserSettings = false;
            this.GeneralSettings = this.DataModel.Settings;
            this.Users = this.DataModel.LocalWorkspace.Credentials;
        }

        private void GoBackCommandExecute(object parameter)
        {
            this.NavigationService.NavigateToView("ConnectionCenterView", null);
        }

        private void AddUserCommandExectute(object parameter)
        {
            AddUserViewArgs args = new AddUserViewArgs(this.AddUserCallback, false);
            NavigationService.PushModalView("AddUserView", args);
        }

        private void AddUserCallback(Credentials creds, bool store)
        {
            this.DataModel.LocalWorkspace.Credentials.Add(creds);
        }

        private void EditUserCommandExectute(object parameter)
        {
            throw new NotImplementedException();
        }

        private void DeleteUserCommandExectute(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
