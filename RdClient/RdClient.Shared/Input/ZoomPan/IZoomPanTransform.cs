using RdClient.Shared.CxWrappers;
using Windows.Foundation;

namespace RdClient.Shared.Input.ZoomPan
{
    public enum TransformType
    {
        ZoomIn = 1,     // default zoom in
        ZoomOut = 2,    // default zoom out
        ZoomCustom = 3,   // custom magnification parameters
        Pan = 4
    }

    public interface IZoomPanTransform
    {
        TransformType TransformType { get; }
    }
}
