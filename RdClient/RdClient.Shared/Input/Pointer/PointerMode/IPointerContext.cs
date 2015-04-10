using RdClient.Shared.Helpers;
using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer.PointerMode
{
    public interface IPointerContext
    {
        IPointerControl Control { get; }
        DoubleClickTimerOld Timer { get; }

        bool MoveThresholdExceeded(PointerEventOld pointerEvent, double threshold = GlobalConstants.TouchMoveThreshold);
        Point LastMoveVector { get; }
        double LastMoveDistance { get; }
        PointerMoveOrientation LastMoveOrientation { get; }

        bool SpreadThresholdExceeded(PointerEventOld pointerEvent);

        double LastSpreadDelta { get; }
        Point LastPanDelta { get; }
        Point LastSpreadCenter { get; }

        int NumberOfContacts(PointerEventOld pointerEvent);
        int FirstContactHistoryCount();

        void TrackEvent(PointerEventOld pointerEvent);

        void Reset();
    }
}
