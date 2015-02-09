using RdClient.Shared.CxWrappers;
using System;
using System.Threading.Tasks;

namespace RdClient.Shared.Models
{
    public class ThumbnailUpdatedEventArgs : EventArgs
    {
        public byte[] EncodedImageBytes { get; private set; }

        public ThumbnailUpdatedEventArgs(byte[] thumbnailBytes)
        {
            this.EncodedImageBytes = thumbnailBytes;
        }
    }

    public interface IThumbnailEncoder
    {
        void Update(IRdpScreenSnapshot snapshot);

        event EventHandler<ThumbnailUpdatedEventArgs> ThumbnailUpdated;
    }
}
