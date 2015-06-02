namespace RdClient.Navigation.Extensions
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using Windows.Devices.Input;
    using Windows.UI.Core;
    using Windows.UI.ViewManagement;



    /// <summary>
    /// IDeviceCapabilities based on WinRT device capabilities classes.
    /// </summary>
    sealed class DeviceCapabilities : MutableObject, IDeviceCapabilities
    {
        private readonly CoreWindow _window;
        private readonly TouchCapabilities _touchCapabilities;
        private bool _canShowInputPanel;
        private UserInteractionMode _userInteractionMode;

        public DeviceCapabilities()
        {
            //
            // Subscribe for size changes of the core window; when the input mode changes (touch to mouse, mouse to touch)
            // size change is reported.
            //
            _window = CoreWindow.GetForCurrentThread();
            _window.SizeChanged += this.OnWindowSizeChanged;

            _touchCapabilities = new TouchCapabilities();

            _userInteractionMode = UIViewSettings.GetForCurrentView().UserInteractionMode;
            _canShowInputPanel = CheckIfCanShowInputPanel();
        }

        uint IDeviceCapabilities.TouchPoints
        {
            get { return _touchCapabilities.Contacts; }
        }

        bool IDeviceCapabilities.TouchPresent
        {
            get { return _touchCapabilities.TouchPresent > 0; }
        }

        bool IDeviceCapabilities.CanShowInputPanel
        {
            get { return _canShowInputPanel; }
        }

        private bool CanShowInputPanel
        {
            set { SetProperty(ref _canShowInputPanel, value); }
        }

        private void OnWindowSizeChanged(CoreWindow sender, WindowSizeChangedEventArgs e)
        {
            UserInteractionMode mode = UIViewSettings.GetForCurrentView().UserInteractionMode;

            if(mode != _userInteractionMode)
            {
                _userInteractionMode = mode;
                this.CanShowInputPanel = CheckIfCanShowInputPanel();
            }
        }

        private bool CheckIfCanShowInputPanel()
        {
            //
            // TODO:    after getting all information from OSG, use the current state to figure out
            //          if the input panel may be shown now.
            //
            return UserInteractionMode.Touch == _userInteractionMode && _touchCapabilities.TouchPresent > 0;
        }
    }
}
