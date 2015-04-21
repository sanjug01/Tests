using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer
{
    // the implementation of this interface is responsible for 
    // keeping the position of the local cursor in sync between different input modes
    public interface IPointerPosition
    {
        Point PointerPosition { get; set; }
    }
}
