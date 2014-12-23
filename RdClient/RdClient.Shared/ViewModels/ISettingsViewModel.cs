using System;
namespace RdClient.Shared.ViewModels
{
    public interface ISettingsViewModel
    {
        System.Windows.Input.ICommand GoBackCommand { get; }
        bool ShowGatewaySettings { get; set; }
        bool ShowGeneralSettings { get; set; }
        bool ShowUserSettings { get; set; }
    }
}
