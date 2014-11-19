using RdClient.Shared.Helpers;
using RdClient.Shared.Navigation;
using RdClient.Shared.ViewModels;
using Windows.UI.Core;

namespace RdClient.Helpers
{
    public class DisconnectStringsExtension : INavigationExtension
    {
        private DisconnectStrings _disconnectStrings;

        public DisconnectStringsExtension()
        {
            _disconnectStrings = new DisconnectStrings(new LocalizedStrings());
        }

        public void Presenting(IViewModel viewModel)
        {
            IViewModelDisconnectStrings vmds = viewModel as IViewModelDisconnectStrings;
            if(null != vmds)
            {
                vmds.DisconnectStrings = _disconnectStrings;
            }
        }
    }
}
