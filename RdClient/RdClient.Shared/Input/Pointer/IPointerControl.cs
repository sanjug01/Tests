using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer
{
    public enum PointerDragAction
    {
        Begin,
        Update,
        End
    }

    // An IPointerControl is an abstraction used by the input state machine to actuate mouse actions.
    // usually the implementation of an IPointerControl will make sure the local mouse cursor is in the
    // right place and will forward the actions to the IRemoteSessionControl
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
