namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Models;
    using Windows.UI.Xaml.Media.Imaging;

    public interface IDesktopViewModel : IRemoteConnectionViewModel
    {
        DesktopModel Desktop { get; }

        CredentialsModel Credentials { get; }

        BitmapImage Thumbnail { get; }
    }
}
