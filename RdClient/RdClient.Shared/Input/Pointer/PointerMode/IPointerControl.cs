
namespace RdClient.Shared.Input.Pointer.PointerMode
{
    public interface IPointerControl
    {
        IPointerManipulator Manipulator { get; }

        void MouseLeftClick(PointerEventOld pointerEvent);
        void MouseMove(PointerEventOld pointerEvent);
        void MouseScroll(PointerEventOld pointerEvent);
        void MouseRightClick(PointerEventOld pointerEvent);
        void UpdateCursorPosition(PointerEventOld pointerEvent);

        void ZoomAndPan(PointerEventOld pointerEvent);
    }
}
