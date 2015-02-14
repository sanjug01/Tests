namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using System;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Runtime.Serialization;
    using System.Threading;
    using Windows.Storage.Streams;
    using Windows.UI.Xaml.Media.Imaging;

    [DataContract(IsReference=true)]
    public sealed class ThumbnailModel : SerializableModel, IDisposable
    {
        private ReaderWriterLockSlim _monitor;
        private BitmapImage _image;
        private EventHandler _imageUpdated;

        [DataMember(Name = "EncodedImageBytes", EmitDefaultValue = false, IsRequired = false)]
        private byte[] _encodedImageBytes;

        public ThumbnailModel()
        {
            _monitor = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        ~ThumbnailModel()
        {
            Dispose(false);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            _monitor = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        public event EventHandler ImageUpdated
        {
            add
            {
                using (ReadWriteMonitor.Write(_monitor))
                    _imageUpdated += value;
            }

            remove
            {
                using (ReadWriteMonitor.Write(_monitor))
                    _imageUpdated -= value;
            }
        }

        public void UpdateEncodedBytes(byte[] encodedBytes)
        {
            //
            // The method is always called on a worker thread.
            //
            using(ReadWriteMonitor.Write(_monitor))
            {
                _image = null;
                _encodedImageBytes = encodedBytes;
                //
                // The emitted event must be consumed by UI components and handling must be dispatched
                // to the UI thread.
                //
                if (null != _imageUpdated)
                    _imageUpdated(this, EventArgs.Empty);
            }
        }

        public void Clear()
        {
            if (null != _encodedImageBytes)
            {
                _image = null;
                _encodedImageBytes = null;
                SetModified();
            }
        }

        public BitmapImage Image
        {
            get
            {
                if (null == _image)
                {
                    using(ReadWriteMonitor.Read(_monitor))
                        _image = DecodeImage(_encodedImageBytes);
                    SetModified();
                }

                return _image;
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
                _monitor.Dispose();
        }
    }
}
