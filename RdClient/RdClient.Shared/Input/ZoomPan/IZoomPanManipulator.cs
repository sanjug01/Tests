using RdClient.Shared.CxWrappers;
using Windows.Foundation;

namespace RdClient.Shared.Input.ZoomPan
{
    public interface IZoomPanManipulator
    {
        Rect WindowRect { set; }

        IZoomPanTransform ZoomPanTransform { get; }
        double ScaleCenterX { get; }
        double ScaleCenterY { get; }
        double ScaleXFrom  { get; }
        double ScaleXTo  { get; }
        double ScaleYFrom { get; }
        double ScaleYTo { get; }
        double TranslateXFrom { get; }
        double TranslateXTo { get; }
        double TranslateYFrom { get; }
        double TranslateYTo { get; }
    }
}
