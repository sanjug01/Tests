using RdClient.Shared.Models;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public interface ISettingsViewModel
    {
        ICommand GoBackCommand { get; }
        bool ShowGatewaySettings { get; set; }
        bool ShowGeneralSettings { get; set; }
        bool ShowUserSettings { get; set; }
        GeneralSettings GeneralSettings { get; }
    }
}
