namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Models;
    using System.Windows.Input;
    using Windows.UI.Xaml.Media.Imaging;

    public interface IRemoteResourceViewModel
    {
        string Name { get; }
        ICommand ConnectCommand { get; }
        BitmapImage Icon { get; }
    }
}
