using RdClient.Shared.Helpers;
using Windows.Foundation;
namespace RdClient.Shared.Input.Mouse
{
    public interface ITouchContext
    {
        DoubleClickTimer DoubleClickTimer { get; set; }
        void MouseLeftClick(PointerEvent pointerEvent);
        void MouseMove(PointerEvent pointerEvent);
        void MouseRightClick(PointerEvent pointerEvent);
        void UpdateCursorPosition(PointerEvent pointerEvent);

        bool MoveThresholdExceeded(PointerEvent pointerEvent);
        int NumberOfContacts(PointerEvent pointerEvent);
        
        IPointerManipulator PointerManipulator { get; set; }
    }
}
