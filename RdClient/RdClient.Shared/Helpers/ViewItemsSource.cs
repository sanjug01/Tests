namespace RdClient.Shared.Helpers
{
    using System;
    using RdClient.Shared.ViewModels;

    /// <summary>
    /// Helper implements IViewItemsSource and retains the IItemsView passed to is.
    /// </summary>
    public sealed class ViewItemsSource : IViewItemsSource
    {
        private IItemsView _itemsView;

        /// <summary>
        /// Call IItemsView.SelectItem for the view that had set itself by calling IViewItemsSource.SetItemsView.
        /// </summary>
        /// <param name="item">View model of the item that must be scrolled into the view and selected.</param>
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
