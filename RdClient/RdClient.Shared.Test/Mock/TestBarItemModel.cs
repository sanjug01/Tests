namespace RdClient.Shared.Test.Mock
{
    using RdClient.Shared.ViewModels;

    sealed class TestBarItemModel : BarItemModel
    {
        public TestBarItemModel(ItemAlignment alignment = ItemAlignment.Left, bool isVisible = true) : base(alignment, isVisible)
        {
        }
    }
}
