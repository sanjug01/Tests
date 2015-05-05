using RdClient.Shared.Input.Pointer;
using Windows.UI.Input;

namespace RdClient.Shared.Input.Recognizers
{
    public enum ZoomScrollType
    {
        ZoomPan,
        Scroll,
        HScroll
    }

    public interface IZoomScrollEvent : IPointerEventBase
    {
        ZoomScrollType Type { get; }
        ManipulationDelta Delta { get; }
        bool IsInertial { get; }
    }
}
