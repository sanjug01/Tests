using Windows.UI.Xaml.Media;

namespace RdClient.Shared.Input.Pointer
{
    public interface ICursorEncoder
    {
        ImageSource ByteArrayToBitmap(byte[] buffer, int width, int height);
    }
}