using RdClient.Shared.Helpers;
using Windows.Foundation;
namespace RdClient.Shared.Input.Mouse
{
    public interface ITouchContext
    {
        DoubleClickTimer DoubleClickTimer { get; }
        void MouseLeftClick(PointerEvent pointerEvent);
        void MouseMove(PointerEvent pointerEvent);
        void MouseScroll(PointerEvent pointerEvent);
        void MouseRightClick(PointerEvent pointerEvent);
        void UpdateCursorPosition(PointerEvent pointerEvent);

        void BeginGesture(PointerEvent pointerEvent);
        void EndGesture(PointerEvent pointerEvent);
        void ApplyGesture(PointerEvent pointerEvent);

        bool MoveThresholdExceeded(PointerEvent pointerEvent);
        int NumberOfContacts(PointerEvent pointerEvent);

       
        IPointerManipulator PointerManipulator { get; }
    }
}
