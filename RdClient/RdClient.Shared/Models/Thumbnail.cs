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
    public class Thumbnail : ModelBase, IThumbnail
    {
        public const uint THUMBNAIL_HEIGHT = 256;
        private BitmapImage _image;
        private byte[] _imageBytes;        

        public async Task Update(IRdpScreenSnapshot snapshot)
        {
            byte[] encodedBytes;
            using (IRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream).AsTask().ConfigureAwait(false);
                encoder.SetPixelData(snapshot.PixelFormat, BitmapAlphaMode.Ignore, snapshot.Width, snapshot.Height, 96.0, 96.0, snapshot.Bytes);
                encoder.BitmapTransform.ScaledHeight = THUMBNAIL_HEIGHT;
                encoder.BitmapTransform.ScaledWidth = Convert.ToUInt32(snapshot.Width * THUMBNAIL_HEIGHT / (double) snapshot.Height);
                encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;
                await encoder.FlushAsync().AsTask().ConfigureAwait(false);
                encodedBytes = new byte[stream.Size];
                stream.Seek(0);
                await stream.ReadAsync(encodedBytes.AsBuffer(), (uint) stream.Size, InputStreamOptions.None);
            }
            this.ImageBytes = encodedBytes;
        }

        public bool HasImage
        {
            get
            {
                return (this.Image != null);
            }
        }

        public BitmapImage Image
        {
            get
            {
               return _image;
            }
            private set
            {
                SetProperty(ref _image, value);
            }
        }

        [DataMember]
        private byte[] ImageBytes
        {
            get
            {
                return _imageBytes;
            }

            set
            {
                if (value != null)
                {
                    _imageBytes = value;
                    this.UpdateImageSource();
                }
            }
        }

        private async void UpdateImageSource()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    BitmapImage newImage = new BitmapImage();
                    using (IRandomAccessStream stream = new InMemoryRandomAccessStream())
                    {
                        await stream.WriteAsync(this.ImageBytes.AsBuffer());
                        stream.Seek(0);
                        await newImage.SetSourceAsync(stream);
                    }
                    this.Image = newImage;
                });
        }
    }
}
