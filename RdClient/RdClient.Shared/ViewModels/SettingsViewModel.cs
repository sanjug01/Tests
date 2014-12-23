using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public class SettingsViewModel : ViewModelBase, ISettingsViewModel 
    {
        private RelayCommand _goBackCommand;
        private bool _showGeneralSettings;
        private bool _showGatewaySettings;
        private bool _showUserSettings;

        public SettingsViewModel()
        {
            _goBackCommand = new RelayCommand(this.GoBackCommandExecute);            
        }

        public ICommand GoBackCommand
        {
            get { return _goBackCommand; }
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

        protected override void OnPresenting(object activationParameter)
        {
            base.OnPresenting(activationParameter);
            this.ShowGeneralSettings = true;
            this.ShowGatewaySettings = false;
            this.ShowUserSettings = false;
        }

        private void GoBackCommandExecute(object parameter)
        {
            this.NavigationService.NavigateToView("ConnectionCenterView", null);
        }
    }
}
