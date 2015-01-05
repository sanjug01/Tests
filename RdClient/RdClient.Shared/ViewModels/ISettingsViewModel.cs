using RdClient.Shared.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public interface ISettingsViewModel
    {
        ICommand GoBackCommand { get; }
        ICommand AddUserCommand { get; }
        bool ShowGatewaySettings { get; set; }
        bool ShowGeneralSettings { get; set; }
        bool ShowUserSettings { get; set; }
        bool HasCredentials { get; }
        GeneralSettings GeneralSettings { get; }
        ObservableCollection<ICredentialViewModel> CredentialsViewModels { get; } 
    }
}
