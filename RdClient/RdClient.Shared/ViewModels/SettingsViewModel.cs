using RdClient.Shared.Data;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
        private ReadOnlyObservableCollection<ICredentialViewModel> _credentialViewModels;
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

        public ReadOnlyObservableCollection<ICredentialViewModel> CredentialsViewModels
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

            foreach (ICredentialViewModel vm in this.CredentialsViewModels)
            {
                vm.Presented(this.NavigationService, this.ApplicationDataModel);
            }
        }

        private void SettingsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ApplicationDataModel")
            {
                //
                // Set up a transforming observable collection of credentials view models sourced at the local workspace's credentials
                // and creating view models for credentials models.
                //
                this.CredentialsViewModels = TransformingObservableCollection<IModelContainer<CredentialsModel>, ICredentialViewModel>
                    .Create(this.ApplicationDataModel.LocalWorkspace.Credentials.Models, this.CreateCredentialsViewModel);
                this.CredentialsViewModels.CastAndCall<INotifyPropertyChanged>(npc =>
                    npc.PropertyChanged += this.OnCredentialsViewModelsPropertyChanged);
                this.HasCredentials = this.CredentialsViewModels.Count > 0;
            }
        }

        private ICredentialViewModel CreateCredentialsViewModel(IModelContainer<CredentialsModel> container)
        {
            return new CredentialViewModel(container.Model);
        }

        private void OnCredentialsViewModelsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName.Equals("Count"))
            {
                this.HasCredentials = this.CredentialsViewModels.Count > 0;
            }
        }

        private void GoBackCommandExecute()
        {
            this.NavigationService.NavigateToView("ConnectionCenterView", null);
        }

        private void AddUserCommandExecute()
        {
            AddUserViewArgs args = new AddUserViewArgs(new CredentialsModel(), false);

            ModalPresentationCompletion addUserCompleted = new ModalPresentationCompletion((s, e) =>
            {
                CredentialPromptResult result = e.Result as CredentialPromptResult;

                if (result != null && !result.UserCancelled)
                {
                    this.ApplicationDataModel.LocalWorkspace.Credentials.AddNewModel(result.Credentials);
                }
            });

            NavigationService.PushModalView("AddUserView", args, addUserCompleted);
        }
    }
}
