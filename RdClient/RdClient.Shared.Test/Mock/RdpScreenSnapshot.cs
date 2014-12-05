using RdClient.Shared.CxWrappers;
using Windows.Graphics.Imaging;

namespace RdClient.Shared.Test.Mock
{
    public class RdpScreenSnapshot : IRdpScreenSnapshot
    {
        public uint Width { get; set; }

        public uint Height { get; set; }

        public byte[] Bytes { get; set; }

        public BitmapPixelFormat PixelFormat { get; set; }
    }
}
