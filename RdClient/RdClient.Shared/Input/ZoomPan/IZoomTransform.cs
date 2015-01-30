using RdClient.Shared.CxWrappers;
using Windows.Foundation;

namespace RdClient.Shared.Input.ZoomPan
{

    public interface ICustomZoomTransform : IZoomPanTransform
    {
        double CenterX { get; }
        double CenterY { get; }
        double ScaleX { get; }
        double ScaleY { get; }        
    }
}
