using RdClient.Shared.Models;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Linq;

namespace RdClient.Shared.ViewModels
{
    public class SettingsViewModel : ViewModelBase, ISettingsViewModel 
    {
        private RelayCommand _goBackCommand;
        private RelayCommand _addUserCommand;
        private bool _showGeneralSettings;
        private bool _showGatewaySettings;
        private bool _showUserSettings;
        private GeneralSettings _generalSettings;
        private ObservableCollection<ICredentialViewModel> _credentialViewModels;
        private bool _hasCredentials;

        public SettingsViewModel()
        {
            _goBackCommand = new RelayCommand(o => this.GoBackCommandExecute());
            _addUserCommand = new RelayCommand(o => this.AddUserCommandExectute());
            this.PropertyChanged += SettingsViewModel_PropertyChanged;
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

        public ObservableCollection<ICredentialViewModel> CredentialsViewModels
        {
            get { return _credentialViewModels; }
            private set { SetProperty(ref _credentialViewModels, value); }
        }

        public bool HasCredentials
        {
            get { return _hasCredentials; }
            private set { SetProperty(ref _hasCredentials, value); }
        }

        protected override void OnPresenting(object activationParameter)
        {
            this.ShowGeneralSettings = true;
            this.ShowGatewaySettings = false;
            this.ShowUserSettings = false;
            this.GeneralSettings = this.DataModel.Settings;
            foreach (CredentialViewModel vm in this.CredentialsViewModels)
            {
                vm.Presented(this.NavigationService, this.DataModel);
            }
        }

        private void SettingsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataModel")
            {
                ObservableCollection<ICredentialViewModel> credVMs = new ObservableCollection<ICredentialViewModel>();
                foreach (Credentials cred in this.DataModel.LocalWorkspace.Credentials)
                {
                    CredentialViewModel vm = new CredentialViewModel(cred);
                    credVMs.Add(vm);
                }
                this.CredentialsViewModels = credVMs;
                this.HasCredentials = this.CredentialsViewModels.Count > 0;
                this.DataModel.LocalWorkspace.Credentials.CollectionChanged += Credentials_CollectionChanged;
            }
        }

        private void Credentials_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Credentials cred in e.NewItems)
                {
                    CredentialViewModel vm = new CredentialViewModel(cred);
                    vm.Presented(this.NavigationService, this.DataModel);
                    this.CredentialsViewModels.Add(vm);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var vmsToRemove = this.CredentialsViewModels.Where(vm => e.OldItems.Contains(vm.Credential)).ToList();
                foreach (CredentialViewModel vm in vmsToRemove)
                {
                    this.CredentialsViewModels.Remove(vm);
                }
            }
            this.HasCredentials = this.CredentialsViewModels.Count > 0;
        }

        private void GoBackCommandExecute()
        {
            this.NavigationService.NavigateToView("ConnectionCenterView", null);
        }

        private void AddUserCommandExectute()
        {
            AddUserViewArgs args = new AddUserViewArgs(this.AddUserCallback, false);
            NavigationService.PushModalView("AddUserView", args);
        }

        private void AddUserCallback(Credentials creds, bool store)
        {
            this.DataModel.LocalWorkspace.Credentials.Add(creds);
        }
    }
}
