using System.Diagnostics.Contracts;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;

namespace RdClient.Shared.Input.Pointer
{
    public class CursorEncoder : ICursorEncoder
    {
        public ImageSource ByteArrayToBitmap(byte[] buffer, int width, int height)
        {
            Contract.Assert(null != buffer);

            WriteableBitmap bitmap = new WriteableBitmap(width, height);

            using (System.IO.Stream stream = bitmap.PixelBuffer.AsStream())
            {
                byte alpha;

                stream.Position = 0;

                //
                // The format used by the WriteableBitmap is ARGB32 (premultiplied RGB).
                // So the pixels in the Pixel array of a WriteableBitmap are stored as
                // pre-multiplied colors. Each color channel is pre-multiplied by the alpha value.
                //
                for (int i = 0; i < buffer.Length; i += 4)
                {
                    alpha = buffer[i];

                    //
                    // Copy the ARGB color in reverse order, using WriteByte.
                    // WriteByte writes a byte to the current position in the stream and advances
                    // the position within the stream by one byte.
                    //
                    stream.WriteByte((byte)(buffer[i + 3]));
                    stream.WriteByte((byte)(buffer[i + 2]));
                    stream.WriteByte((byte)(buffer[i + 1]));
                    stream.WriteByte(alpha);
                };
            }

            return bitmap;
        }
    }
}
