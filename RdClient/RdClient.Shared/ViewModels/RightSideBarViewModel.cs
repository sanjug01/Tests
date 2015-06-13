using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Pointer;
using RdClient.Shared.Models;
using System;
using System.Windows.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace RdClient.Shared.ViewModels
{
    public class RightSideBarViewModel : MutableObject, IRightSideBarViewModel
    {
        private IDeviceCapabilities _deviceCapabilities;
        public IDeviceCapabilities DeviceCapabilities
        {
            private get
            {
                return _deviceCapabilities;
            }
            set
            {
                _deviceCapabilities = value;

                if(_deviceCapabilities != null)
                {
                    _deviceCapabilities.PropertyChanged += OnUserInteractionModeChange;

                    OnUserInteractionModeChange(this, EventArgs.Empty);
                }
            }
        }

        public IRemoteSession RemoteSession
        {
            private get; set;
        }

        public IFullScreenModel FullScreenModel
        {
            get; set;
        }

        private void OnUserInteractionModeChange(object sender, EventArgs e)
        {
            if(this.DeviceCapabilities.CanShowInputPanel)
            {
                this.FullScreenButtonVisibility = Visibility.Collapsed;
            }
            else
            {
                this.FullScreenButtonVisibility = Visibility.Visible;
            }

            if(this.DeviceCapabilities.TouchPresent)
            {
                this.MouseModeButtonVisibility = Visibility.Visible;
            }
            else
            {
                this.MouseModeButtonVisibility = Visibility.Collapsed;
            }
        }

        private Visibility _fullScreenButtonVisibility;
        public Visibility FullScreenButtonVisibility
        {
            get
            {
                return _fullScreenButtonVisibility;
            }
            set
            {
                SetProperty(ref _fullScreenButtonVisibility, value);
            }
        }

        private Visibility _mouseModeButtonVisibility;
        public Visibility MouseModeButtonVisibility
        {
            get
            {
                return _mouseModeButtonVisibility;
            }
            set
            {
                SetProperty(ref _mouseModeButtonVisibility, value);
            }
        }

        public IPointerCapture PointerCapture
        {
            private get; set;
        }

        private readonly ICommand _disconnectCommand;
        public ICommand Disconnect
        {
            get { return _disconnectCommand; }
        }

        private readonly ICommand _fullScreenCommand;
        public ICommand FullScreen
        {
            get { return _fullScreenCommand; }
        }

        private bool _isFullScreenChecked;
        public bool IsFullScreenChecked
        {
            get
            {
                return _isFullScreenChecked;
            }
            set
            {
                SetProperty(ref _isFullScreenChecked, value);
            }
        }

        private readonly ICommand _mouseModeCommand;
        public ICommand MouseMode
        {
            get { return _mouseModeCommand; }
        }

        private bool _isMouseModeChecked;
        public bool IsMouseModeChecked
        {
            get
            {
                return _isMouseModeChecked;
            }
            set
            {
                SetProperty(ref _isMouseModeChecked, value);
            }
        }

        private ICommand _toggleVisibility;
        public ICommand ToggleVisiblity
        {
            get { return _toggleVisibility; }
        }    

        private Visibility _visibility;
        public Visibility Visibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                SetProperty(ref _visibility, value);
            }
        }

        public RightSideBarViewModel()
        {
            _disconnectCommand = new RelayCommand(InternalDisconnect);
            _mouseModeCommand = new RelayCommand(InternalMouseMode);
            _fullScreenCommand = new RelayCommand(InternalFullScreen);
            _toggleVisibility = new RelayCommand(InternalToggleVisibility);
        }

        private void InternalDisconnect(object parameter)
        {
            this.Visibility = Visibility.Collapsed;

            if (this.RemoteSession != null)
            {
                this.RemoteSession.Disconnect();
            }
        }

        private void InternalMouseMode(object parameter)
        {
            this.Visibility = Visibility.Collapsed;

            if (this.PointerCapture != null)
            {
                this.PointerCapture.OnMouseModeChanged(this, EventArgs.Empty);
            }
        }


        private void InternalFullScreen(object parameter)
        {
            this.Visibility = Visibility.Collapsed;
            this.FullScreenModel.ToggleFullScreen();
        }

        private void InternalToggleVisibility(object parameter)
        {
            if(this.Visibility == Visibility.Visible)
            {
                this.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.Visibility = Visibility.Visible;
            }
        }
    }
}
