using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;

namespace RdClient.Shared.CxWrappers
{
    public class RdpScreenSnapshot : IRdpScreenSnapshot
    {
        public const BitmapPixelFormat PIXEL_FORMAT = BitmapPixelFormat.Bgra8;
        public const int BYTES_PER_PIXEL = 4;
        public uint Width { get; private set; }
        public uint Height { get; private set; }
        public byte[] Bytes { get; private set; }
        public BitmapPixelFormat PixelFormat { get; private set;}

        public RdpScreenSnapshot(int width, int height, byte[] bytes)
        {
            this.PixelFormat = PIXEL_FORMAT;
            this.Width = Convert.ToUInt32(width);
            this.Height = Convert.ToUInt32(height);
            if (bytes.Length == this.Width * this.Height * BYTES_PER_PIXEL)
            {
                this.Bytes = bytes;                
            }
            else
            {
                throw new ArgumentException("ScreenSnapshot byte array is wrong size. It should have a length equal to width * height * 4 bytes per pixel");
            }                        
        }
    }
}
