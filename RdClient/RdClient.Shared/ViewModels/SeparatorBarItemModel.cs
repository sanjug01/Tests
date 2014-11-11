namespace RdClient.Shared.ViewModels
{
    public sealed class SeparatorBarItemModel : BarItemModel
    {
        public SeparatorBarItemModel(ItemAlignment alignment = ItemAlignment.Left, bool isVisible = true)
            : base(alignment, isVisible)
        {
        }
    }
}
