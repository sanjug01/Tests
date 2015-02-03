using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using RdMock;
using System.Threading.Tasks;

namespace RdClient.Shared.Test.Mock
{
    public class Thumbnail : MockBase, IThumbnail
    {
        public byte[] EncodedImageBytes { get; set; }

        public void Update(IRdpScreenSnapshot snapshot)
        {
            Invoke(new object[]{snapshot});
        }
    }
}
