namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Input.Pointer;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation.Extensions;
    using System.ComponentModel;
    using Windows.UI.Xaml;

    public interface IRightSideBarViewModel : INotifyPropertyChanged, ITelemetryClientSite
    {
        IFullScreenModel FullScreenModel { get; set; }
        IDeviceCapabilities DeviceCapabilities { get; set; }


        IPointerCapture PointerCapture { set; }
        IRemoteSession RemoteSession { set; }
        Visibility Visibility { get; set; }
    }
}