using RdClient.Shared.Helpers;
using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer.PointerMode
{
    public interface IPointerContext
    {
        IPointerControl Control { get; }
        DoubleClickTimer Timer { get; }

        bool MoveThresholdExceeded(PointerEvent pointerEvent, double threshold = GlobalConstants.TouchMoveThreshold);
        Point LastMoveVector { get; }
        double LastMoveDistance { get; }
        PointerMoveOrientation LastMoveOrientation { get; }

        bool SpreadThresholdExceeded(PointerEvent pointerEvent);

        double LastSpreadDelta { get; }
        Point LastPanDelta { get; }
        Point LastSpreadCenter { get; }

        int NumberOfContacts(PointerEvent pointerEvent);
        int FirstContactHistoryCount();

        void TrackEvent(PointerEvent pointerEvent);

        void Reset();
    }
}
