using RdClient.Shared.Input.Pointer;
using System;

namespace RdClient.Shared.Input.Recognizers
{
    public interface ITapRecognizer : IPointerEventConsumer
    {
        event EventHandler<ITapEvent> Tapped;

    }
}
