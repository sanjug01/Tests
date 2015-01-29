using RdClient.Shared.CxWrappers;
using Windows.Foundation;

namespace RdClient.Shared.Input.ZoomPan
{
    public interface IPanTransform : IZoomPanTransform
    {
        double X { get; }
        double Y { get; }
    }
}
