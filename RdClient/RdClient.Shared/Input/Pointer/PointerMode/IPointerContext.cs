using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer.PointerMode
{
    public interface IPointerContext
    {
        IPointerControl Control { get; }
        DoubleClickTimer Timer { get; }

        bool MoveThresholdExceeded(PointerEvent pointerEvent);
        Point LastMoveVector { get; }
        double LastMoveDistance { get; }
        PointerMoveOrientation LastMoveOrientation { get; }


        bool SpreadThresholdExceeded(PointerEvent pointerEvent);
        double LastSpreadDelta { get; }
        Point LastSpreadCenter { get; }

        int NumberOfContacts(PointerEvent pointerEvent);

        void TrackEvent(PointerEvent pointerEvent);

        void Reset();
    }
}
