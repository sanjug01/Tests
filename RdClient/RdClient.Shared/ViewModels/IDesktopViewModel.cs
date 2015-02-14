namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Models;
    using System.ComponentModel;
    using System.Windows.Input;
    using Windows.UI.Xaml.Media.Imaging;

    public interface IDesktopViewModel : INotifyPropertyChanged
    {
        DesktopModel Desktop { get; }

        CredentialsModel Credentials { get; }

        BitmapImage Thumbnail { get; }

        bool IsSelected { get; set; }

        bool SelectionEnabled { get; set; }

        ICommand EditCommand { get; }

        ICommand ConnectCommand { get; }

        ICommand DeleteCommand { get; }

        void Dismissed();
    }
}
