namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Models;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    public interface IAddOrEditDesktopViewModel
    {
        ICommand Cancel { get; }
        
        ObservableCollection<UserComboBoxElement> UserOptions { get; }
        UserComboBoxElement SelectedUser { get; set; }
        ICommand EditUserCommand { get; }
        ICommand AddUserCommand { get; }

        ObservableCollection<GatewayComboBoxElement> GatewayOptions { get; }
        GatewayComboBoxElement SelectedGateway { get; set; }
        ICommand EditGatewayCommand { get; }
        ICommand AddGatewayCommand { get; }

        bool IsAddingDesktop { get; }
        string Host { get; }
        bool IsHostValid { get; }
        bool IsExpandedView { get; }
        string FriendlyName { get; }
        bool IsUseAdminSession { get; set; }
        bool IsSwapMouseButtons { get; set; }
        int AudioMode { get; set; }
    }
}
