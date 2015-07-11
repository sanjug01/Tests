namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Models;
    using Windows.UI.Xaml.Media.Imaging;
    using Windows.Foundation;

    public interface IDesktopViewModel : IRemoteConnectionViewModel
    {
        DesktopModel Desktop { get; }

        CredentialsModel Credentials { get; }

        Size TileSize { get; set; }
    }
}
