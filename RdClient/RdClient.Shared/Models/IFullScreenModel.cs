using RdClient.Shared.ViewModels;

namespace RdClient.Shared.Models
{
    public interface IFullScreenModel
    {
        RelayCommand EnterFullScreenCommand { get; }
        RelayCommand ExitFullScreenCommand { get; }

        void ToggleFullScreen();
    }
}