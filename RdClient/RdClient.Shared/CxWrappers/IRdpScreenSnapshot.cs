using Windows.Graphics.Imaging;

namespace RdClient.Shared.CxWrappers
{
    public interface IRdpScreenSnapshot
    {
        uint Width { get; }
        uint Height { get; }
        byte[] Bytes { get; }
        BitmapPixelFormat PixelFormat { get; }
    }
}
