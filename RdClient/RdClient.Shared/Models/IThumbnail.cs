using RdClient.Shared.CxWrappers;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace RdClient.Shared.Models
{
    public interface IThumbnail
    {
        bool HasImage { get; }

        BitmapImage Image { get; }

        Task Update(IRdpScreenSnapshot snapshot);
    }
}
