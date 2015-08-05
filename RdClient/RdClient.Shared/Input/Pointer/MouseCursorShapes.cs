using RdClient.Shared.Helpers;
using System.Collections.Generic;
using Windows.UI.Xaml.Media;

namespace RdClient.Shared.Input.Pointer
{
    public class MouseCursorShapes
    {
        private ICursorEncoder _encoder;
        private IDictionary<byte[], ImageSource> _bitmapCache;     

        public MouseCursorShapes(ICursorEncoder encoder)
        {
            _encoder = encoder;
            _bitmapCache = new SortedDictionary<byte[], ImageSource>(new ArrayComparer());
        }

        public ImageSource GetImageSource(byte[] buffer, int width, int height)
        {
            ImageSource shape;

            if(false == _bitmapCache.TryGetValue(buffer, out shape))
            {
                shape = _encoder.ByteArrayToBitmap(buffer, width, height);
                _bitmapCache.Add(buffer, shape);
            }

            return shape;
        }

    }
}
