using RdClient.Shared.Helpers;
using Windows.Foundation;
namespace RdClient.Shared.Input.Mouse
{
    public interface IPointerManipulator
    {
        Point CursorPosition { get; set; }
        Size WindowSize { get; set; }

        void ChangeMousePointer(PointerEventType eventType);
    }
}
