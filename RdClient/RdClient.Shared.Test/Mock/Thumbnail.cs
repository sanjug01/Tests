using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using RdMock;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace RdClient.Shared.Test.Mock
{
    public class Thumbnail : MockBase, IThumbnail
    {
        public bool HasImage { get; set; }

        public BitmapImage Image { get; set; }

        public Task Update(IRdpScreenSnapshot snapshot)
        {
            return Task.FromResult(Invoke(new object[]{snapshot}));
        }       
    }
}
