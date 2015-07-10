namespace RdClient.Shared.ViewModels
{
    using Windows.Foundation;

    public interface ISizeableTile
    {
        Size TileSize { get; }
        Size ScreenSize { set; }
    }
}
