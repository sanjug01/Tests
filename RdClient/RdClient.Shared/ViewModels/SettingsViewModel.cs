using RdClient.Shared.Data;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

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
            _addUserCommand = new RelayCommand(o => this.AddUserCommandExecute());
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
            this.GeneralSettings = this.ApplicationDataModel.Settings;
            foreach (CredentialViewModel vm in this.CredentialsViewModels)
            {
                vm.Presented(this.NavigationService, this.ApplicationDataModel);
            }
        }

        private void SettingsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ApplicationDataModel")
            {
                ObservableCollection<ICredentialViewModel> credVMs = new ObservableCollection<ICredentialViewModel>();

                foreach (IModelContainer<CredentialsModel> cred in this.ApplicationDataModel.LocalWorkspace.Credentials.Models)
                {
                    CredentialViewModel vm = new CredentialViewModel(cred.Model);
                    credVMs.Add(vm);
                }
                this.CredentialsViewModels = credVMs;
                this.HasCredentials = this.CredentialsViewModels.Count > 0;

                INotifyCollectionChanged ncc = this.ApplicationDataModel.LocalWorkspace.Credentials.Models;
                ncc.CollectionChanged += Credentials_CollectionChanged;
            }
        }

        private void Credentials_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (IModelContainer<CredentialsModel> cred in e.NewItems)
                {
                    CredentialViewModel vm = new CredentialViewModel(cred.Model);
                    vm.Presented(this.NavigationService, this.ApplicationDataModel);
                    this.CredentialsViewModels.Add(vm);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach(IModelContainer<CredentialsModel> container in e.OldItems)
                {
                    ICredentialViewModel vm = this.CredentialsViewModels.First(cvm => object.ReferenceEquals(cvm.Credentials, container.Model));
                    this.CredentialsViewModels.Remove(vm);
                }
            }

            this.HasCredentials = this.CredentialsViewModels.Count > 0;
        }

        private void GoBackCommandExecute()
        {
            this.NavigationService.NavigateToView("ConnectionCenterView", null);
        }

        private void AddUserCommandExecute()
        {
            AddUserViewArgs args = new AddUserViewArgs(new CredentialsModel(), false);

            ModalPresentationCompletion addUserCompleted = new ModalPresentationCompletion();

            addUserCompleted.Completed += (s, e) =>
            {
                CredentialPromptResult result = e.Result as CredentialPromptResult;

                if (result != null && !result.UserCancelled)
                {
                    this.ApplicationDataModel.LocalWorkspace.Credentials.AddNewModel(result.Credentials);
                }
            };
            NavigationService.PushModalView("AddUserView", args, addUserCompleted);
        }
    }
}
