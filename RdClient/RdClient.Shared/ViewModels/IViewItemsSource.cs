namespace RdClient.Shared.ViewModels
{
    /// <summary>
    /// Callback interface implemented by view models to that item views may give themselves to the view models.
    /// Views should implement IItemsView interface that gives the model access to visual state of view items
    /// inaccesible through XAML bindings (like scrolling an utem into the viewport and moving input focus).
    /// </summary>
    public interface IViewItemsSource
    {
        void SetItemsView(IItemsView itemsView);
    }
}
