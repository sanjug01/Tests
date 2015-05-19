using Windows.Foundation;

namespace RdClient.Shared.Models.Viewport
{
    public interface IViewportTransform
    {
        double ScaleX { get; set; }
        double ScaleY { get; set; }
        double TranslateX { get; set; }
        double TranslateY { get; set; }
        Point TransformPoint(Point point);
        Point InverseTransformPoint(Point point);

    }
}