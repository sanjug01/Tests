using RdClient.Shared.Models.Viewport;
using Windows.UI.Xaml;

namespace RdClient.Shared.Models
{
    public interface IScrollBarModel
    {
        double MaximumHorizontal { get; }
        double MaximumVertical { get; }
        double MinimumHorizontal { get; }
        double MinimumVertical { get; }
        double ValueHorziontal { get; set; }
        double ValueVertical { get; set; }
        IViewport Viewport { set; }
        double ViewportHeight { get; }
        double ViewportWidth { get; }
        double HorizontalScrollBarWidth { set; }
        double VerticalScrollBarWidth { set; }
        Visibility VisibilityHorizontal { get; }
        Visibility VisibilityVertical { get; }
        Visibility VisibilityCorner { get; }
    }
}