namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Helpers;
    using System;
    using System.Runtime.InteropServices.WindowsRuntime;
    using Windows.Graphics.Imaging;
    using Windows.Storage.Streams;

    sealed class ThumbnailEncoder : MutableObject, IThumbnailEncoder
    {
        private readonly uint _thumbnailHeight;
        private EventHandler<ThumbnailUpdatedEventArgs> _thumbnailUpdated;

        public static IThumbnailEncoder Create(uint thumbnailHeight)
        {
            return new ThumbnailEncoder(thumbnailHeight);
        }

        private ThumbnailEncoder(uint thumbnailHeight)
        {
            _thumbnailHeight = thumbnailHeight;
        }

        void IThumbnailEncoder.Update(IRdpScreenSnapshot snapshot)
        {
            ThumbnailUpdatedEventArgs e = new ThumbnailUpdatedEventArgs(GetSnapshotBytes(snapshot));

            using(LockUpgradeableRead())
            {
                if (null != _thumbnailUpdated)
                    _thumbnailUpdated(this, e);
            }
        }

        event EventHandler<ThumbnailUpdatedEventArgs> IThumbnailEncoder.ThumbnailUpdated
        {
            add { using (LockWrite()) _thumbnailUpdated += value; }
            remove { using (LockWrite()) _thumbnailUpdated -= value; }
        }

        private BitmapEncoder CreateBitmapEncoderWithStream(IRandomAccessStream stream)
        {
            return BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream).AsTask<BitmapEncoder>().Result;
        }

        private byte[] GetSnapshotBytes(IRdpScreenSnapshot snapshot)
        {
            byte[] bytes = null;

            using (IRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                BitmapEncoder encoder = CreateBitmapEncoderWithStream(stream);

                encoder.SetPixelData(snapshot.PixelFormat, BitmapAlphaMode.Ignore, snapshot.Width, snapshot.Height, 96.0, 96.0, snapshot.RawImage);
                encoder.BitmapTransform.ScaledHeight = _thumbnailHeight;
                encoder.BitmapTransform.ScaledWidth = snapshot.Width * _thumbnailHeight / snapshot.Height;
                encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;

                encoder.FlushAsync().AsTask().Wait();

                uint length = (uint)stream.Size;

                bytes = new byte[length];

                stream.GetInputStreamAt(0)
                    .ReadAsync(bytes.AsBuffer(), length, InputStreamOptions.None)
                    .AsTask<IBuffer, uint>()
                    .Wait();
            }

            return bytes;
        }
    }
}
