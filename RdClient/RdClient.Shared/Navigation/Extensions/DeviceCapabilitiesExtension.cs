namespace RdClient.Shared.Navigation.Extensions
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;

    public sealed class DeviceCapabilitiesExtension : MutableObject, INavigationExtension
    {
        private IDeviceCapabilities _deviceCapabilities;

        public IDeviceCapabilities DeviceCapabilities
        {
            get { return _deviceCapabilities; }
            set { this.SetProperty(ref _deviceCapabilities, value); }
        }

        void INavigationExtension.Presenting(IViewModel viewModel)
        {
            viewModel.CastAndCall<IDeviceCapabilitiesSite>(site => site.SetDeviceCapabilities(_deviceCapabilities));
        }

        void INavigationExtension.Dismissed(IViewModel viewModel)
        {
            viewModel.CastAndCall<IDeviceCapabilitiesSite>(site => site.SetDeviceCapabilities(null));
        }
    }
}
