using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation.Extensions;
using System;

namespace RdClient.Shared.Input.Pointer
{
    public interface IPointerCapture
    {
        void OnPointerChangedOld(object sender, PointerEventArgs args);
        void OnPointerChanged(object sender, IPointerEventBase e);
        void OnMouseCursorPositionChanged(object sender, MouseCursorPositionChangedArgs args);
        void OnMouseCursorShapeChanged(object sender, MouseCursorShapeChangedArgs args);

    }
}
