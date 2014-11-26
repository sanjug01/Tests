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
        private WriteableBitmap _bmp;
        private byte[] _encodedBytes;

        public void Update(uint width, uint height, byte[] imageBytes)
        {
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    CreateBmpIfRequired(width, height);
                    using (Stream stream = _bmp.PixelBuffer.AsStream())
                    {
                        await stream.WriteAsync(imageBytes, 0, imageBytes.Length);
                        OnPropertyChanged("Image");
                    }
                });
        }

        //private CoreDispatcher _dispatcher;

        //public Thumbnail()
        //    : base()
        //{
        //    _dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
        //}

        //private async Task UpdateInternal(uint width, uint height, byte[] imageBytes)
        //{
        //    if (_dispatcher.HasThreadAccess)
        //    {
        //        await UpdateInternal(width, height, imageBytes);
        //    }
        //    else
        //    {
        //        await _dispatcher.RunAsync(CoreDispatcherPriority.Normal,
        //            async () =>
        //            {
        //                await UpdateInternal(width, height, imageBytes);
        //            });
        //    }
        //}

        public bool HasImage
        {
            get
            {
                return _bmp != null;
            }
        }

        public ImageSource Image
        {
            get
            {
                return _bmp;
            }
        }

        /*
         * For serialization
         */
        [DataMember]
        public byte[] JpegBytes
        {
            get
            {
                return _encodedBytes;
            }
            set
            {
                this.decodeFromJpeg(value);
            }
        }

        private void CreateBmpIfRequired(uint newWidth, uint newHeight)
        {
            if (_bmp == null || newWidth != _bmp.PixelWidth || newHeight != _bmp.PixelHeight)
            {                
                _bmp = new WriteableBitmap((int)newWidth, (int)newHeight);
            }
        }


        private async Task<byte[]> encodeToJpeg()
        {
            byte[] encodedBytes = new byte[0];
            if (_bmp != null)
            {
                using (IRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream).AsTask().ConfigureAwait(false);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)_bmp.PixelWidth, (uint)_bmp.PixelHeight, 96.0, 96.0, _bmp.PixelBuffer.ToArray());
                    await encoder.FlushAsync();
                    byte[] bytes = new byte[stream.Size];
                    IBuffer buffer = new Windows.Storage.Streams.Buffer((uint)stream.Size);
                    await stream.ReadAsync(buffer, (uint)stream.Size, InputStreamOptions.None);
                    encodedBytes = buffer.ToArray();
                }
            }
            return encodedBytes;
        }

        private async Task decodeFromJpeg(byte[] encodedBytes)
        {
            if (encodedBytes != null && encodedBytes.Length > 0)
            {
                using (IRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    await stream.WriteAsync(encodedBytes.AsBuffer());
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                    PixelDataProvider pixelProvider = await decoder.GetPixelDataAsync();                    
                    CreateBmpIfRequired(decoder.PixelWidth, decoder.PixelHeight);                    
                    using (Stream bmpPixelStream = _bmp.PixelBuffer.AsStream())
                    {
                        byte[] decodedPixels = pixelProvider.DetachPixelData();
                        await bmpPixelStream.WriteAsync(decodedPixels, 0, decodedPixels.Length);
                    }
                    OnPropertyChanged("Image");
                }
            }            
        }

        public override async Task PrepForSerialization()
        {
            await base.PrepForSerialization();
            _encodedBytes = await this.encodeToJpeg();
        }
    }
}
