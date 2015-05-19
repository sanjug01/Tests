namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.ValidationRules;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    public interface IAddOrEditDesktopViewModel
    {
        ICommand Cancel { get; }
        
        ReadOnlyObservableCollection<UserComboBoxElement> Users { get; }
        UserComboBoxElement SelectedUser { get; set; }
        ICommand EditUserCommand { get; }
        ICommand AddUserCommand { get; }

        ReadOnlyObservableCollection<GatewayComboBoxElement> Gateways { get; }
        GatewayComboBoxElement SelectedGateway { get; set; }
        ICommand EditGatewayCommand { get; }
        ICommand AddGatewayCommand { get; }

        bool IsAddingDesktop { get; }
        IValidatedProperty<string> Host { get; }
        bool IsExpandedView { get; }
        string FriendlyName { get; }
        bool IsUseAdminSession { get; set; }
        bool IsSwapMouseButtons { get; set; }
        int AudioMode { get; set; }
    }
}
