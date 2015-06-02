using RdClient.Shared.ViewModels;
using Windows.UI.ViewManagement;

namespace RdClient.Shared.Models
{
    public interface IFullScreenModel
    {
        RelayCommand EnterFullScreenCommand { get; }
        RelayCommand ExitFullScreenCommand { get; }

        UserInteractionMode UserInteractionMode { get; }

        void ToggleFullScreen();
    }
}