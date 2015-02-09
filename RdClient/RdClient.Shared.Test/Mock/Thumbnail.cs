using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using RdMock;
using System.Threading.Tasks;

namespace RdClient.Shared.Test.Mock
{
    public class Thumbnail : MockBase, IThumbnailEncoder
    {
        public byte[] EncodedImageBytes { get; set; }

        void IThumbnailEncoder.Update(IRdpScreenSnapshot snapshot)
        {
            Invoke(new object[] { snapshot });
        }

        event System.EventHandler<ThumbnailUpdatedEventArgs> IThumbnailEncoder.ThumbnailUpdated
        {
            add { throw new System.NotImplementedException(); }
            remove { throw new System.NotImplementedException(); }
        }
    }
}
