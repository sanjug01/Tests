using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public class SettingsViewModel : ViewModelBase 
    {
        private RelayCommand _goBackCommand;

        public SettingsViewModel()
        {
            _goBackCommand = new RelayCommand(this.GoBackCommandExecute);
        }

        public ICommand GoBackCommand
        {
            get { return _goBackCommand; }
        }

        private void GoBackCommandExecute(object parameter)
        {
            this.NavigationService.NavigateToView("ConnectionCenterView", null);
        }
    }
}
