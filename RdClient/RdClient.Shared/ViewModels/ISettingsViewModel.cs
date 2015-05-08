namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Models;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    public interface ISettingsViewModel
    {
        ICommand Cancel { get; }

        GeneralSettings GeneralSettings { get; }

        ReadOnlyObservableCollection<UserComboBoxElement> Users { get; }
        UserComboBoxElement SelectedUser { get; set; }
        ICommand DeleteUserCommand { get; }
        ICommand EditUserCommand { get; }
        ICommand AddUserCommand { get; }

        ReadOnlyObservableCollection<GatewayComboBoxElement> Gateways { get; }
        GatewayComboBoxElement SelectedGateway { get; set; }
        ICommand DeleteGatewayCommand { get; }
        ICommand EditGatewayCommand { get; }
        ICommand AddGatewayCommand { get; }
    }
}
