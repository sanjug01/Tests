using System.Windows.Input;
using RdClient.Shared.Input.Pointer;
using RdClient.Shared.Models;
using Windows.UI.Xaml;
using System.ComponentModel;

namespace RdClient.Shared.ViewModels
{
    public interface IRightSideBarViewModel : INotifyPropertyChanged
    {
        IFullScreenModel FullScreenModel { get; set; }
        IDeviceCapabilities DeviceCapabilities { get; set; }


        IPointerCapture PointerCapture { set; }
        IRemoteSession RemoteSession { set; }
        Visibility Visibility { get; set; }
    }
}