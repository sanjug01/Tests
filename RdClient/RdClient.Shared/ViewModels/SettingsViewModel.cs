using RdClient.Shared.Data;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public class SettingsViewModel : ViewModelBase, ISettingsViewModel 
    {
        private RelayCommand _goBackCommand;
        private RelayCommand _addUserCommand;
        private RelayCommand _addGatewayCommand;
        private bool _showGeneralSettings;
        private bool _showGatewaySettings;
        private bool _showUserSettings;
        private GeneralSettings _generalSettings;
        private ReadOnlyObservableCollection<ICredentialViewModel> _credentialViewModels;
        private ReadOnlyObservableCollection<IGatewayViewModel> _gatewayViewModels;
        private bool _hasCredentials;
        private bool _hasGateways;

        public SettingsViewModel()
        {
            _goBackCommand = new RelayCommand(o => this.GoBackCommandExecute());
            _addUserCommand = new RelayCommand(o => this.AddUserCommandExecute());
            _addGatewayCommand = new RelayCommand(o => this.AddGatewayCommandExecute());
        }

        public ICommand GoBackCommand
        {
            get { return _goBackCommand; }
        }

        public ICommand AddUserCommand
        {
            get { return _addUserCommand; }
        }

        public ICommand AddGatewayCommand
        {
            get { return _addGatewayCommand; }
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

        public ReadOnlyObservableCollection<IGatewayViewModel> GatewaysViewModels
        {
            get { return _gatewayViewModels; }
            private set { SetProperty(ref _gatewayViewModels, value); }
        }

        public bool HasCredentials
        {
            get { return _hasCredentials; }
            private set { SetProperty(ref _hasCredentials, value); }
        }

        public bool HasGateways
        {
            get { return _hasGateways; }
            private set { SetProperty(ref _hasGateways, value); }
        }

        protected override void OnPresenting(object activationParameter)
        {
            this.ShowGeneralSettings = true;
            this.ShowGatewaySettings = false;
            this.ShowUserSettings = false;
            this.GeneralSettings = this.ApplicationDataModel.Settings;

            //
            // Set up a transforming observable collection of credentials view models sourced at the local workspace's credentials
            // and creating view models for credentials models.
            //
            this.CredentialsViewModels = TransformingObservableCollection<IModelContainer<CredentialsModel>, ICredentialViewModel>
                .Create(this.ApplicationDataModel.Credentials.Models,
                this.CreateCredentialsViewModel,
                this.ReleaseCredentialsViewModel);
            this.CredentialsViewModels.CastAndCall<INotifyPropertyChanged>(npc => npc.PropertyChanged += this.OnCredentialsViewModelsPropertyChanged);
            this.HasCredentials = this.CredentialsViewModels.Count > 0;

            this.GatewaysViewModels = TransformingObservableCollection<IModelContainer<GatewayModel>, IGatewayViewModel>
                .Create(this.ApplicationDataModel.Gateways.Models,
                this.CreateGatewayViewModel,
                this.ReleaseGatewayViewModel);
            this.GatewaysViewModels.CastAndCall<INotifyPropertyChanged>(npc => npc.PropertyChanged += this.OnGatewaysViewModelsPropertyChanged);
            this.HasGateways = this.GatewaysViewModels.Count > 0;
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

            this.CredentialsViewModels.CastAndCall<INotifyPropertyChanged>(npc =>
                npc.PropertyChanged -= this.OnCredentialsViewModelsPropertyChanged);
            this.CredentialsViewModels = null;
            this.HasCredentials = false;

            base.OnDismissed();
        }

        protected override void OnNavigatingBack(IBackCommandArgs backArgs)
        {
            this.GoBackCommand.Execute(null);
            backArgs.Handled = true;
        }

        private ICredentialViewModel CreateCredentialsViewModel(IModelContainer<CredentialsModel> container)
        {
            ICredentialViewModel vm = new CredentialViewModel(container);
            vm.Presented(this.NavigationService, this.ApplicationDataModel);
            return vm;
        }

        private void ReleaseCredentialsViewModel(ICredentialViewModel vm)
        {
            vm.Dismissed();
        }

        private IGatewayViewModel CreateGatewayViewModel(IModelContainer<GatewayModel> container)
        {
            IGatewayViewModel vm = new GatewayViewModel(container);
            vm.Presented(this.NavigationService, this.ApplicationDataModel);
            return vm;
        }

        private void ReleaseGatewayViewModel(IGatewayViewModel vm)
        {
            vm.Dismissed();
        }

        private void OnCredentialsViewModelsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName.Equals("Count"))
            {
                this.HasCredentials = this.CredentialsViewModels.Count > 0;
            }
        }

        private void OnGatewaysViewModelsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Count"))
            {
                this.HasGateways = this.GatewaysViewModels.Count > 0;
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
                    this.ApplicationDataModel.Credentials.AddNewModel(result.Credentials);
                }
            });

            NavigationService.PushModalView("AddUserView", args, addUserCompleted);
        }

        private void AddGatewayCommandExecute()
        {
            AddGatewayViewModelArgs args = new AddGatewayViewModelArgs();
            NavigationService.PushModalView("AddOrEditGatewayView", args);
        }
    }
}
