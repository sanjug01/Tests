using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using RdMock;
using System.Threading.Tasks;

namespace RdClient.Shared.Test.Mock
{
    public class Thumbnail : MockBase, IThumbnail
    {
        public byte[] ImageBytes { get; set; }

        public Task Update(IRdpScreenSnapshot snapshot)
        {
            return Task.FromResult(Invoke(new object[]{snapshot}));
        }
    }
}
