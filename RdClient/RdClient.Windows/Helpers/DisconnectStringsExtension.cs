using RdClient.Shared.Helpers;
using RdClient.Shared.Navigation;
using RdClient.Shared.ViewModels;

namespace RdClient.Helpers
{
    public sealed class DisconnectStringsExtension : INavigationExtension
    {
        private readonly DisconnectStrings _disconnectStrings;

        public DisconnectStringsExtension()
        {
            _disconnectStrings = new DisconnectStrings(new LocalizedStrings());
        }

        void INavigationExtension.Presenting(IViewModel viewModel)
        {
            viewModel.CastAndCall<IViewModelDisconnectStrings>(vmds => vmds.DisconnectStrings = _disconnectStrings);
        }

        void INavigationExtension.Dismissed(IViewModel viewModel)
        {
            //
            // TODO: probably change the view model to handle the absense of strings.
            //
            viewModel.CastAndCall<IViewModelDisconnectStrings>(vmds => vmds.DisconnectStrings = null);
        }
    }
}
