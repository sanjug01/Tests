using RdClient.Shared.CxWrappers;
using System;
using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer
{
    public interface IPointerPosition
    {
        Point PointerPosition { get; set; }
    }

    public interface IPointerCapture
    {
        void OnMouseModeChanged(object sender, EventArgs e);
        void OnPointerChanged(object sender, IPointerEventBase e);
        void OnMouseCursorPositionChanged(object sender, MouseCursorPositionChangedArgs args);
        void OnMouseCursorShapeChanged(object sender, MouseCursorShapeChangedArgs args);
        void OnMultiTouchEnabledChanged(object sender, MultiTouchEnabledChangedArgs args);

    }
}
