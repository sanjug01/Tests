using RdClient.Shared.Helpers;

namespace RdClient.Shared.Input.Mouse
{
    public interface IPointerEventConsumer
    {
        void ConsumeEvent(PointerEvent pointerEvent);
        void Reset();
    }
}
