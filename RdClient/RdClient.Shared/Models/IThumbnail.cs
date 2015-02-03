using RdClient.Shared.CxWrappers;
using System;
using System.Threading.Tasks;

namespace RdClient.Shared.Models
{
    public class ThumbnailUpdatedEventArgs : EventArgs
    {
        public byte[] NewThumbnailBytes { get; private set; }
        public ThumbnailUpdatedEventArgs(byte[] thumbnailBytes)
        {
            this.NewThumbnailBytes = thumbnailBytes;
        }
    }

    public interface IThumbnail
    {
        void Update(IRdpScreenSnapshot snapshot);

        byte[] EncodedImageBytes { get; }
    }
}
