using RdClient.Shared.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public interface ISettingsViewModel
    {
        ICommand Cancel { get; }
        ICommand AddUserCommand { get; }
        ICommand AddGatewayCommand { get; }

        bool HasCredentials { get; }
        bool HasGateways { get; }
        GeneralSettings GeneralSettings { get; }
        ReadOnlyObservableCollection<ICredentialViewModel> CredentialsViewModels { get; }
        ReadOnlyObservableCollection<IGatewayViewModel> GatewaysViewModels { get; } 
    }
}
