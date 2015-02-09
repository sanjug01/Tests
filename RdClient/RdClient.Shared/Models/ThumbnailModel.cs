namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Runtime.Serialization;
    using Windows.Storage.Streams;
    using Windows.UI.Xaml.Media.Imaging;
    using System;

    [DataContract(IsReference=true)]
    public sealed class ThumbnailModel : SerializableModel
    {
        private BitmapImage _image;

        [DataMember(Name = "EncodedImageBytes", EmitDefaultValue = false, IsRequired = false)]
        private byte[] _encodedImageBytes;

        public ThumbnailModel()
        {
        }

        public BitmapImage Image
        {
            get
            {
                if (null == _image)
                    _image = DecodeImage(_encodedImageBytes);
                return _image;
            }
        }

        public byte[] EncodedImageBytes
        {
            get { return _encodedImageBytes; }
            set
            {
                if(this.SetProperty(ref _encodedImageBytes, value))
                {
                    _image = null;
                    EmitPropertyChanged("Image");
                }
            }
        }

        private static BitmapImage DecodeImage(byte[] imageBytes)
        {
            BitmapImage image = new BitmapImage();

            if (null != imageBytes)
            {
                using (IRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    stream.WriteAsync(imageBytes.AsBuffer()).AsTask<uint, uint>().Wait();
                    stream.Seek(0);
                    image.SetSourceAsync(stream).AsTask().Wait();
                }
            }

            return image;
        }
    }
}
