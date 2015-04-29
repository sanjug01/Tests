using RdClient.Shared.Input.Pointer;

namespace RdClient.Shared.Input.Recognizers
{
    public enum TapEventType
    {
        Tap,
        HoldingStarted,
        TapHoldingStarted,
        TapMovingStarted,
        HoldingCompleted,
        HoldingCancelled,
        DoubleTap,
    }


    public interface ITapEvent : IPointerEventBase
    {
        TapEventType Type { get; }
    }
}
