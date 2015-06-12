using RdClient.Shared.ViewModels;
using System;
using Windows.UI.ViewManagement;

namespace RdClient.Shared.Models
{
    public interface IFullScreenModel
    {
        event EventHandler FullScreenChange;
        event EventHandler UserInteractionModeChange;

        RelayCommand EnterFullScreenCommand { get; }
        RelayCommand ExitFullScreenCommand { get; }

        UserInteractionMode UserInteractionMode { get; }

        void ToggleFullScreen();
        bool IsFullScreenMode { get; }
    }
}