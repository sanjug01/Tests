using RdClient.Shared.Helpers;
using RdClient.Shared.Navigation;
using RdClient.Shared.ViewModels;

namespace RdClient.Helpers
{
    public sealed class DisconnectStringExtension : INavigationExtension
    {
        private readonly DisconnectString _disconnectStrings;

        public DisconnectStringExtension()
        {
            _disconnectStrings = new DisconnectString(new LocalizedString());
        }

        void INavigationExtension.Presenting(IViewModel viewModel)
        {
            viewModel.CastAndCall<IViewModelDisconnectString>(vmds => vmds.DisconnectString = _disconnectStrings);
        }

        void INavigationExtension.Dismissed(IViewModel viewModel)
        {
            //
            // TODO: probably change the view model to handle the absense of strings.
            //
            viewModel.CastAndCall<IViewModelDisconnectString>(vmds => vmds.DisconnectString = null);
        }
    }
}
