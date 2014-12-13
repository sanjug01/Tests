using RdClient.Shared.CxWrappers;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;

namespace RdClient.Shared.Models
{
    [DataContract(IsReference = true)]
    public sealed class Thumbnail : ModelBase, IThumbnail
    {
        public const uint THUMBNAIL_HEIGHT = 276;

        private byte[] _imageBytes;        

        public async Task Update(IRdpScreenSnapshot snapshot)
        {
            byte[] encodedBytes;
            using (IRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream).AsTask().ConfigureAwait(false);
                encoder.SetPixelData(snapshot.PixelFormat, BitmapAlphaMode.Ignore, snapshot.Width, snapshot.Height, 96.0, 96.0, snapshot.RawImage);
                encoder.BitmapTransform.ScaledHeight = THUMBNAIL_HEIGHT;
                encoder.BitmapTransform.ScaledWidth = Convert.ToUInt32(snapshot.Width * THUMBNAIL_HEIGHT / (double) snapshot.Height);
                encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;
                await encoder.FlushAsync().AsTask().ConfigureAwait(false);
                encodedBytes = new byte[stream.Size];
                stream.Seek(0);
                await stream.ReadAsync(encodedBytes.AsBuffer(), (uint) stream.Size, InputStreamOptions.None);
            }
            this.EncodedImageBytes = encodedBytes;
        }

        [DataMember]
        public byte[] EncodedImageBytes
        {
            get
            {
                return _imageBytes;
            }

            private set
            {
                SetProperty(ref _imageBytes, value);
            }
        }
    }
}
