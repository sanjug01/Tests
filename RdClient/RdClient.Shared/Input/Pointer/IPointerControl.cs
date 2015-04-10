using Windows.Foundation;

namespace RdClient.Shared.Input
{
    public enum PointerDragAction
    {
        Begin,
        Update,
        End
    }

    public interface IPointerControl
    {
        void LeftClick(IPointerRoutedEventProperties pointerEvent);
        void RightClick(IPointerRoutedEventProperties pointerEvent);

        void Move(Point delta);

        void Scroll(double delta);
        void HScroll(double delta);

        void LeftDrag(PointerDragAction action, Point delta);
        void RightDrag(PointerDragAction action, Point delta);

        void ZoomPan(Point center, Point translation, double scale);
    }
}
