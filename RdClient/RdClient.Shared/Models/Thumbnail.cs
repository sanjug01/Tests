using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;


namespace RdClient.Shared.Models
{
    [DataContract(IsReference = true)]
    public class Thumbnail : ModelBase
    {
        private const uint THUMBNAIL_HEIGHT = 256;
        private ImageSource _image;
        private byte[] _imageBytes;        

        public async Task Update(uint width, uint height, byte[] imageBytes)
        {
            byte[] encodedBytes;
            using (IRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream).AsTask().ConfigureAwait(false);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, width, height, 96.0, 96.0, imageBytes);
                encoder.BitmapTransform.ScaledHeight = THUMBNAIL_HEIGHT;
                encoder.BitmapTransform.ScaledWidth = Convert.ToUInt32(width * THUMBNAIL_HEIGHT / (double) height);
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

        public ImageSource Image
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
                _imageBytes = value;
                this.UpdateImageSource();
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
