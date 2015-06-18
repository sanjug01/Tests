using System;
using System.Windows.Input;
using Windows.UI.ViewManagement;

namespace RdClient.Shared.Models
{
    public interface IFullScreenModel
    {
        event EventHandler FullScreenChange;
        event EventHandler UserInteractionModeChange;

        /// <summary>
        /// Event emitted when the model tries to enter the full screen mode (EnterFullScreenCommand is executed)
        /// </summary>
        event EventHandler EnteringFullScreen;
        /// <summary>
        /// Event emitted when the model tries to exit the full screen mode (ExitFullScreenCommand is executed)
        /// </summary>
        event EventHandler ExitingFullScreen;

        ICommand EnterFullScreenCommand { get; }
        ICommand ExitFullScreenCommand { get; }

        UserInteractionMode UserInteractionMode { get; }

        void ToggleFullScreen();
        bool IsFullScreenMode { get; }
    }
}