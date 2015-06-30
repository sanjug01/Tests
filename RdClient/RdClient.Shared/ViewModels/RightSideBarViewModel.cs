using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Pointer;
using RdClient.Shared.Models;
using System;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace RdClient.Shared.ViewModels
{
    public class RightSideBarViewModel : MutableObject, IRightSideBarViewModel
    {
        public IRemoteSession RemoteSession
        {
            private get; set;
        }

        private IPointerCapture _pointerCapture;
        public IPointerCapture PointerCapture
        {
            private get
            {
                return _pointerCapture;
            }
            set
            {
                _pointerCapture = value;
                if(_pointerCapture != null)
                {
                    this.InternalMouseMode(null);
                }
            }
        }

        public IDeviceCapabilities DeviceCapabilities
        {
            get; set;
        }

        private IFullScreenModel _fullScreenModel;
        public IFullScreenModel FullScreenModel
        {
            get
            {
                return _fullScreenModel;
            }
            set
            {
                _fullScreenModel = value;

                if(_fullScreenModel != null)
                {
                    _fullScreenModel.FullScreenChange -= OnFullScreenChange;
                    _fullScreenModel.FullScreenChange += OnFullScreenChange;
                }

                OnFullScreenChange(this, EventArgs.Empty);
            }
        }

        private void OnFullScreenChange(object sender, EventArgs e)
        {
            if (_fullScreenModel.IsFullScreenMode)
            {
                _fullScreenButtonModel.CanExecute = false;
                _normalScreenButtonModel.CanExecute = true;
            }
            else
            {
                _fullScreenButtonModel.CanExecute = true;
                _normalScreenButtonModel.CanExecute = false;
            }
        }

        private BarButtonModel _disconnectButtonModel;
        public BarButtonModel DisconnectButtonModel
        {
            get
            {
                return _disconnectButtonModel;
            }
        }


        private BarButtonModel _fullScreenButtonModel;
        public BarButtonModel FullScreenButtonModel
        { 
            get
            {
                return _fullScreenButtonModel;
            }
        }

        private BarButtonModel _normalScreenButtonModel;
        public BarButtonModel NormalScreenButtonModel
        {
            get
            {
                return _normalScreenButtonModel;
            }
        }

        private BarButtonModel _touchButtonModel;
        public BarButtonModel TouchButtonModel
        {
            get
            {
                return _touchButtonModel;
            }
        }

        private BarButtonModel _mouseButtonModel;
        public BarButtonModel MouseButtonModel
        {
            get
            {
                return _mouseButtonModel;
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
            _disconnectButtonModel = new BarButtonModel() { Command = new RelayCommand(InternalDisconnect) };
            _fullScreenButtonModel = new BarButtonModel() { Command = new RelayCommand(InternalFullScreen) };
            _normalScreenButtonModel = new BarButtonModel() { Command = new RelayCommand(InternalNormalScreen) };        
            _touchButtonModel = new BarButtonModel() { Command = new RelayCommand(InternalTouchMode) };
            _mouseButtonModel = new BarButtonModel() { Command = new RelayCommand(InternalMouseMode) };

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
            this.PointerCapture.ChangeInputMode(InputMode.Mouse);
            _mouseButtonModel.CanExecute = false;
            _touchButtonModel.CanExecute = true && this.DeviceCapabilities.TouchPresent;
            this.Visibility = Visibility.Collapsed;
        }

        private void InternalTouchMode(object parameter)
        {
            this.PointerCapture.ChangeInputMode(InputMode.Touch);
            _mouseButtonModel.CanExecute = true && this.DeviceCapabilities.TouchPresent;
            _touchButtonModel.CanExecute = false;
            this.Visibility = Visibility.Collapsed;
        }

        private void InternalFullScreen(object parameter)
        {
            this.Visibility = Visibility.Collapsed;
            this.FullScreenModel.EnterFullScreen();
        }

        private void InternalNormalScreen(object parameter)
        {
            this.Visibility = Visibility.Collapsed;
            this.FullScreenModel.ExitFullScreen();
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
