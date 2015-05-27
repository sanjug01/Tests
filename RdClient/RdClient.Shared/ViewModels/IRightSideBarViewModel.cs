using System.Windows.Input;
using RdClient.Shared.Input.Pointer;
using RdClient.Shared.Models;
using Windows.UI.Xaml;

namespace RdClient.Shared.ViewModels
{
    public interface IRightSideBarViewModel
    {
        ICommand Disconnect { get; }
        ICommand FullScreen { get; }
        IFullScreenModel FullScreenModel { get; set; }
        bool IsFullScreenChecked { get; set; }
        bool IsMouseModeChecked { get; set; }
        ICommand MouseMode { get; }
        IPointerCapture PointerCapture { set; }
        IRemoteSession RemoteSession { set; }
        ICommand ToggleVisiblity { get; }
        Visibility Visibility { get; set; }
    }
}