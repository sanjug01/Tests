using Windows.Foundation;

namespace RdClient.Shared.Models.Viewport
{
    public interface IViewportPanel
    {
        double Width { get; set; }
        double Height { get; set; }
        IViewportTransform Transform { get; }
    }
}