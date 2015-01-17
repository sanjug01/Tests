using RdClient.Shared.Helpers;
namespace RdClient.Shared.Input.Mouse
{
    public interface ITouchContext
    {
        DoubleClickTimer DoubleClickTimer { get; set; }
        void MouseLeftClick();
        void MouseMove(PointerEvent pointerEvent);
        void MouseRightClick();
        bool MoveThresholdExceeded(PointerEvent pointerEvent);
        int NumberOfContacts(PointerEvent pointerEvent);
        IPointerManipulator PointerManipulator { get; set; }
        void UpdateCursorPosition(PointerEvent pointerEvent);
    }
}
