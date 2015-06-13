namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;

    public sealed class AddDesktopViewModelArgs
    {
        private readonly ViewItemsSource _itemsSource;

        public AddDesktopViewModelArgs(ViewItemsSource itemsSource)
        {
            _itemsSource = itemsSource;
        }

        public void EmitDesktopAdded(DesktopModel newDesktop)
        {
        }
    }
}
