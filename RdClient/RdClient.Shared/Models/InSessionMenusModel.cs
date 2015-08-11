namespace RdClient.Shared.Models
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Input.Pointer;
    using RdClient.Shared.ViewModels;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    /// <summary>
    /// Model injected in the view model of the in-session menus view.
    /// </summary>
    public sealed class InSessionMenusModel : DisposableObject, IInSessionMenus
    {
        private readonly IDeferredExecution _dispatcher;
        private readonly IRemoteSession _session;
        private readonly IFullScreenModel _fullScreenModel;
        private readonly IPointerCapture _pointerCapture;
        private readonly IDeviceCapabilities _deviceCapabilities;
        private readonly RelayCommand _enterFullScreen;
        private readonly RelayCommand _exitFullScreen;
        private readonly RelayCommand _touchMode;
        private readonly RelayCommand _pointerMode;
        private bool _disposed;

        /// <summary>
        /// Create a new InSessionMenusModel object.
        /// </summary>
        /// <param name="dispatcher">Deferred execution dispatcher dispatching events emitted by the full screen model
        /// to the UI thread.</param>
        /// <param name="session">Remote session object.</param>
        /// <param name="fullScreenModel">Full screen model object.</param>
        public InSessionMenusModel(IDeferredExecution dispatcher,
            IRemoteSession session,
            IFullScreenModel fullScreenModel,
            IPointerCapture pointerCapture,
            IDeviceCapabilities deviceCapabilities)
        {
            Contract.Assert(null != dispatcher);
            Contract.Assert(null != session);
            Contract.Assert(null != fullScreenModel);
            Contract.Assert(null != pointerCapture);
            Contract.Assert(null != deviceCapabilities);
            Contract.Ensures(null != _dispatcher);
            Contract.Ensures(null != _session);
            Contract.Ensures(null != _fullScreenModel);
            Contract.Ensures(null != _pointerCapture);
            Contract.Ensures(null != _deviceCapabilities);

            _dispatcher = dispatcher;
            _session = session;
            _fullScreenModel = fullScreenModel;
            _pointerCapture = pointerCapture;
            _deviceCapabilities = deviceCapabilities;

            _enterFullScreen = new RelayCommand(
                parameter => _fullScreenModel.EnterFullScreen(),
                parameter => !_fullScreenModel.IsFullScreenMode);
            _exitFullScreen = new RelayCommand(
                parameter => _fullScreenModel.ExitFullScreen(),
                parameter => _fullScreenModel.IsFullScreenMode);

            _touchMode = new RelayCommand(this.SetTouchMode, this.CanSetTouchMode);
            _pointerMode = new RelayCommand(this.SetPointerMode, this.CanSetPointerMode);

            _fullScreenModel.EnteredFullScreen += this.OnFullScreenChanged;
            _fullScreenModel.ExitedFullScreen += this.OnFullScreenChanged;
            _deviceCapabilities.PropertyChanged += this.OnDeviceCapabilitiesChanged;
            _disposed = false;
        }

        void IInSessionMenus.Disconnect()
        {
            _session.Disconnect();
        }

        ICommand IInSessionMenus.EnterFullScreen { get { return _enterFullScreen; } }

        ICommand IInSessionMenus.ExitFullScreen { get { return _exitFullScreen; } }

        ICommand IInSessionMenus.TouchMode { get { return _touchMode; } }

        ICommand IInSessionMenus.PointerMode { get { return _pointerMode; } }

        protected override void DisposeManagedState()
        {
            _disposed = true;
            _fullScreenModel.EnteredFullScreen -= this.OnFullScreenChanged;
            _fullScreenModel.ExitedFullScreen -= this.OnFullScreenChanged;
            _deviceCapabilities.PropertyChanged -= this.OnDeviceCapabilitiesChanged;
            base.DisposeManagedState();
        }

        private void OnFullScreenChanged(object sender, EventArgs e)
        {
            _dispatcher.Defer(() =>
            {
                if (!_disposed)
                {
                    _enterFullScreen.EmitCanExecuteChanged();
                    _exitFullScreen.EmitCanExecuteChanged();
                }
            });
        }

        private void OnDeviceCapabilitiesChanged(object sender, PropertyChangedEventArgs e)
        {
            _touchMode.EmitCanExecuteChanged();
            _pointerMode.EmitCanExecuteChanged();
        }

        private void SetTouchMode(object parameter)
        {
            _pointerCapture.InputMode = InputMode.Touch;
            _touchMode.EmitCanExecuteChanged();
            _pointerMode.EmitCanExecuteChanged();
        }

        private void SetPointerMode(object parameter)
        {
            _pointerCapture.InputMode = InputMode.Mouse;
            _touchMode.EmitCanExecuteChanged();
            _pointerMode.EmitCanExecuteChanged();
        }

        private bool CanSetTouchMode(object parameter)
        {
            return InputMode.Touch != _pointerCapture.InputMode && _deviceCapabilities.TouchPresent;
        }

        private bool CanSetPointerMode(object parameter)
        {
            return InputMode.Mouse != _pointerCapture.InputMode && _deviceCapabilities.TouchPresent;
        }
    }
}
