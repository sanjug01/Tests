using System;
using Windows.UI.ViewManagement;

namespace RdClient.Shared.Helpers
{
    public interface IFullScreen
    {
        bool IsFullScreenMode { get; }
        UserInteractionMode UserInteractionMode { get; }

        event EventHandler IsFullScreenModeChange;
        event EventHandler UserInteractionModeChange;

        void EnterFullScreen();
        void ExitFullScreen();
    }
}