namespace RdClient.Shared.Navigation
{
    using RdClient.Shared.Helpers;

    /// <summary>
    /// Implementation of IViewVisibility that can be consumed by XAML bindings.
    /// </summary>
    public sealed class ViewVisibility : MutableObject, IViewVisibility
    {
        private bool _isVisible;

        public static IViewVisibility Create(bool isVisible = false)
        {
            return new ViewVisibility(isVisible);
        }

        private ViewVisibility(bool isVisible)
        {
            _isVisible = isVisible;
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            private set { this.SetProperty(ref _isVisible, value); }
        }

        void IViewVisibility.Hide()
        {
            this.IsVisible = false;
        }

        void IViewVisibility.Show()
        {
            this.IsVisible = true;
        }
    }
}
