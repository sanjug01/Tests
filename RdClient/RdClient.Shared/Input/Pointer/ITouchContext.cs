using RdClient.Shared.Helpers;
using Windows.Foundation;
namespace RdClient.Shared.Input.Pointer
{

    public enum GestureType
    {
        Idle, 
        Unknown,
        Scrolling,
        Zooming,
        Panning
    }

    public interface ITouchContext
    {
        DoubleClickTimerOld DoubleClickTimer { get; }
        void MouseLeftClick(PointerEventOld pointerEvent);
        void MouseMove(PointerEventOld pointerEvent);
        void MouseScroll(PointerEventOld pointerEvent);
        void MouseRightClick(PointerEventOld pointerEvent);
        void UpdateCursorPosition(PointerEventOld pointerEvent);

        void BeginGesture(PointerEventOld pointerEvent);
        void CompleteGesture(PointerEventOld pointerEvent);
        void ApplyZoom(PointerEventOld pointerEvent);
        void ApplyPan(PointerEventOld pointerEvent);

        bool MoveThresholdExceeded(PointerEventOld pointerEvent);
        int NumberOfContacts(PointerEventOld pointerEvent);
        bool IsScrolling(PointerEventOld pointerEvent);
        bool IsZooming(PointerEventOld pointerEvent);
        bool IsPanning(PointerEventOld pointerEvent);

       
        IPointerManipulator PointerManipulator { get; }
    }
}
