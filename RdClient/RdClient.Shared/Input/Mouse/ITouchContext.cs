using RdClient.Shared.Helpers;
using Windows.Foundation;
namespace RdClient.Shared.Input.Mouse
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
        DoubleClickTimer DoubleClickTimer { get; }
        void MouseLeftClick(PointerEvent pointerEvent);
        void MouseMove(PointerEvent pointerEvent);
        void MouseScroll(PointerEvent pointerEvent);
        void MouseRightClick(PointerEvent pointerEvent);
        void UpdateCursorPosition(PointerEvent pointerEvent);

        void BeginGesture(PointerEvent pointerEvent);
        void CompleteGesture(PointerEvent pointerEvent);
        void ApplyZoom(PointerEvent pointerEvent);
        void ApplyPan(PointerEvent pointerEvent);

        bool MoveThresholdExceeded(PointerEvent pointerEvent);
        int NumberOfContacts(PointerEvent pointerEvent);
        bool IsScrolling(PointerEvent pointerEvent);
        bool IsZooming(PointerEvent pointerEvent);
        bool IsPanning(PointerEvent pointerEvent);

       
        IPointerManipulator PointerManipulator { get; }
    }
}
