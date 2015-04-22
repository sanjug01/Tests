namespace RdClient.Navigation.Extensions
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using Windows.Devices.Input;

    /// <summary>
    /// IDeviceCapabilities based on WinRT device capabilities classes.
    /// </summary>
    sealed class DeviceCapabilities : MutableObject, IDeviceCapabilities
    {
        private readonly TouchCapabilities _touchCapabilities;

        public DeviceCapabilities()
        {
            _touchCapabilities = new TouchCapabilities();
        }

        uint IDeviceCapabilities.TouchPoints
        {
            get { return _touchCapabilities.Contacts; }
        }

        bool IDeviceCapabilities.TouchPresent
        {
            get { return _touchCapabilities.TouchPresent > 0; }
        }
    }
}
