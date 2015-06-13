namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using System;
    using System.Diagnostics.Contracts;

    public sealed class DesktopAddedEventArgs : EventArgs
    {
        private readonly DesktopModel _desktop;
        private readonly ViewItemsSource _itemsSource;

        public DesktopModel Desktop
        {
            get { return _desktop; }
        }

        public ViewItemsSource ItemsSource
        {
            get { return _itemsSource; }
        }

        public DesktopAddedEventArgs(DesktopModel desktop, ViewItemsSource itemsSource)
        {
            Contract.Assert(null != desktop);
            _desktop = desktop;
            _itemsSource = itemsSource;
        }
    }

    public sealed class AddDesktopViewModelArgs
    {
        private readonly ViewItemsSource _itemsSource;

        public event EventHandler<DesktopAddedEventArgs> DesktopAdded;

        public AddDesktopViewModelArgs(ViewItemsSource itemsSource)
        {
            _itemsSource = itemsSource;
        }

        public void EmitDesktopAdded(DesktopModel newDesktop)
        {
            if (null != this.DesktopAdded)
                this.DesktopAdded(this, new DesktopAddedEventArgs(newDesktop, _itemsSource));
        }
    }
}
