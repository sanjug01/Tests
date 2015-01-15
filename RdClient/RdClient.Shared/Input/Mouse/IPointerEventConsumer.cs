using RdClient.Shared.Helpers;

namespace RdClient.Shared.Input.Mouse
{
    public interface IPointerEventConsumer
    {
        void Reset();
        void ConsumeEvent(PointerEvent pointerEvent);
    }
}
