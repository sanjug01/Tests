using RdClient.Shared.CxWrappers;
using Windows.Foundation;

namespace RdClient.Shared.Input.ZoomPan
{
    public interface IZoomPanManipulator
    {
        Rect WindowRect { set; }
        Rect TransformRect { set; }

        IZoomPanTransform ZoomPanTransform { get; }
    }
}
