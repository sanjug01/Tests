using RdClient.Shared.CxWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
