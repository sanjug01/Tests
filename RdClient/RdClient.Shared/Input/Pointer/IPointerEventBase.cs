using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer
{
    public interface IPointerEventBase
    {
        PointerEventAction Action { get; }

        Point Position { get; }
    }
}
