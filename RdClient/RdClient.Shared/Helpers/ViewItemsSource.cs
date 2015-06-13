namespace RdClient.Shared.Helpers
{
    using System;
    using RdClient.Shared.ViewModels;

    public sealed class ViewItemsSource : IViewItemsSource
    {
        private IItemsView _itemsView;

        public void SelectItem(object item)
        {
            if (null != _itemsView)
                _itemsView.SelectItem(this, item);
        }

        void IViewItemsSource.SetItemsView(IItemsView itemsView)
        {
            _itemsView = itemsView;
        }
    }
}
