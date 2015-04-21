using Windows.UI.Input;

namespace RdClient.Shared.Input.Pointer
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
