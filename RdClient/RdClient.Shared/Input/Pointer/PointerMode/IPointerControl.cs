
namespace RdClient.Shared.Input.Pointer.PointerMode
{
    public interface IPointerControl
    {
        IPointerManipulator Manipulator { get; }

        void MouseLeftClick(PointerEvent pointerEvent);
        void MouseMove(PointerEvent pointerEvent);
        void MouseScroll(PointerEvent pointerEvent);
        void MouseRightClick(PointerEvent pointerEvent);
        void UpdateCursorPosition(PointerEvent pointerEvent);

        void ZoomAndPan(PointerEvent pointerEvent);
    }
}
