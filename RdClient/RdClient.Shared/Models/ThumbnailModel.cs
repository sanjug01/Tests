namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Data;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Windows.Graphics.Imaging;
    using Windows.Storage.Streams;
    using System;
    using Windows.Foundation;
    using System.Runtime.InteropServices.WindowsRuntime;

    [DataContract(IsReference=true)]
    public sealed class ThumbnailModel : SerializableModel, IThumbnail
    {
        private static readonly uint ThumbnailHeight = 276;

        [DataMember(Name = "EncodedImageBytes")]
        private byte[] _encodedImageBytes;

        public ThumbnailModel()
        {
        }

        void IThumbnail.Update(IRdpScreenSnapshot snapshot)
        {
            this.EncodedImageBytes = GetSnapshotBytes(snapshot);
        }

        public byte[] EncodedImageBytes
        {
            get { return _encodedImageBytes; }
            set { this.SetProperty<byte[]>(ref _encodedImageBytes, value); }
        }

        private BitmapEncoder CreateBitmapEncoderWithStream(IRandomAccessStream stream)
        {
            Task<BitmapEncoder> t = BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream).AsTask<BitmapEncoder>();
            t.Wait();
            return t.Result;
        }

        private byte[] GetSnapshotBytes(IRdpScreenSnapshot snapshot)
        {
            byte[] bytes = null;

            using (IRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                BitmapEncoder encoder = CreateBitmapEncoderWithStream(stream);

                encoder.SetPixelData(snapshot.PixelFormat, BitmapAlphaMode.Ignore, snapshot.Width, snapshot.Height, 96.0, 96.0, snapshot.RawImage);
                encoder.BitmapTransform.ScaledHeight = ThumbnailHeight;
                encoder.BitmapTransform.ScaledWidth = Convert.ToUInt32(snapshot.Width * ThumbnailHeight / (double)snapshot.Height);
                encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;

                encoder.FlushAsync().AsTask().Wait();

                uint length = (uint)bytes.Length;

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
