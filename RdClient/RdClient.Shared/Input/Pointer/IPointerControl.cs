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
        void LeftClick(Point position);
        void RightClick(Point position);

        void Move(Point delta);

        void Scroll(double delta);
        void HScroll(double delta);

        void LeftDrag(PointerDragAction action, Point delta, Point position);
        void RightDrag(PointerDragAction action, Point delta, Point position);

        void ZoomPan(Point center, Point translation, double scale);
    }
}
