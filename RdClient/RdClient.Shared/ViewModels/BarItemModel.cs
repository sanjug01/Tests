namespace RdClient.Shared.ViewModels
{
    /// <summary>
    /// Base class for bar item model classes that represent models of controls shown in the application bar.
    /// </summary>
    public abstract class BarItemModel : Helpers.MutableObject
    {
        private readonly ItemAlignment _alignment;
        private bool _isVisible;

        /// <summary>
        /// Alignment of the item in the bar.
        /// </summary>
        public enum ItemAlignment
        {
            Left,
            Right
        }

        public ItemAlignment Alignment { get { return _alignment; } }

        public bool IsVisible
        {
            get { return _isVisible; }
            set { this.SetProperty<bool>(ref _isVisible, value); }
        }

        protected BarItemModel(ItemAlignment alignment = ItemAlignment.Left, bool isVisible = true)
        {
            _alignment = alignment;
            _isVisible = isVisible;
        }
    }
}
