using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer
{
    // the smallest common denominator of pointer events
    public interface IPointerEventBase
    {
        PointerEventAction Action { get; }
        Point Position { get; }
    }
}
