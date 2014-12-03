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
        private bool _hasImage = false;
        private ImageSource _bmp = new BitmapImage();

        [DataMember]
        private IRandomAccessStream _encodedImageStream = new InMemoryRandomAccessStream();

        public async Task Update(uint width, uint height, byte[] imageBytes)
        {
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, _encodedImageStream).AsTask().ConfigureAwait(false);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, width, height, 96.0, 96.0, imageBytes);
            await encoder.FlushAsync();
            UpdateImageSource();
        }

        public bool HasImage
        {
            get
            {
                return _hasImage;
            }
            private set
            {
                SetProperty(ref _hasImage, value);
            }
        }

        public ImageSource Image
        {
            get
            {
               return _bmp;
            }
            private set
            {
                SetProperty(ref _bmp, value);
            }
        }

        private async void UpdateImageSource()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    BitmapImage newImage = new BitmapImage();
                    await newImage.SetSourceAsync(_encodedImageStream);
                    this.Image = newImage;
                    this.HasImage = true;
                });
        }
    }
}
