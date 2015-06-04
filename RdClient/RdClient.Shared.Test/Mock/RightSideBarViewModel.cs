using RdClient.Shared.Input.Pointer;
using RdClient.Shared.Models;
using RdClient.Shared.ViewModels;
using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using System.ComponentModel;

namespace RdClient.Shared.Test.Mock
{
    public class RightSideBarViewModel : IRightSideBarViewModel
    {
        public IDeviceCapabilities DeviceCapabilities { get; set; }

        public ICommand Disconnect { get; set; }

        public ICommand FullScreen { get; set; }

        public IFullScreenModel FullScreenModel { get; set; }

        public bool IsFullScreenChecked { get; set; }

        public bool IsMouseModeChecked { get; set; }

        public ICommand MouseMode { get; set; }

        public IPointerCapture PointerCapture { get; set; }

        public IRemoteSession RemoteSession { get; set; }

        public ICommand ToggleVisiblity { get; set; }

        public Visibility Visibility { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
