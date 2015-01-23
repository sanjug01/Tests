using RdClient.Shared.CxWrappers;
using Windows.Foundation;

namespace RdClient.Shared.Input.ZoomPan
{
    public enum ZoomUpdateType
    {
        ZoomIn = 1,     // default zoom in
        ZoomOut = 2,    // default zoom out
        ZoomCustom = 3   // custom magnification parameters
    }

    public interface IZoomUpdate
    {
        ZoomUpdateType ZoomType { get; }
    }
    public interface ICustomZoomUpdate : IZoomUpdate
    {
        double CenterX { get; }
        double CenterY { get; }
        double ScaleX { get; }
        double ScaleY { get; }        
    }
}
