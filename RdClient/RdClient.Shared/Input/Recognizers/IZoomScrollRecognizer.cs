using RdClient.Shared.Input.Pointer;
using System;

namespace RdClient.Shared.Input.Recognizers
{
    public interface IZoomScrollRecognizer : IPointerEventConsumer
    {
        event EventHandler<IZoomScrollEvent> ZoomScrollEvent;
    }
}
