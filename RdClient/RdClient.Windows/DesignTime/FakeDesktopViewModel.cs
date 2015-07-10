using RdClient.Shared.Models;
using RdClient.Shared.ViewModels;
using System.ComponentModel;
using System.Windows.Input;
using Windows.UI.Xaml.Media.Imaging;
using System;
using Windows.Foundation;

namespace RdClient.DesignTime
{
    public sealed class FakeDesktopViewModel : IDesktopViewModel, ISizeableTile
    {
        private CredentialsModel _cred = new CredentialsModel() { Password = "1234AbCd", Username = "exampleUser" };
        private DesktopModel _desktop = new DesktopModel() { HostName = "ExampleHostname" };
        private PropertyChangedEventHandler _propertyChanged;

        public FakeDesktopViewModel()
        {
            this.IsSelected = true;
            this.SelectionEnabled = true;
        }

        public DesktopModel Desktop
        {
            get { return _desktop; }
        }

        public CredentialsModel Credentials
        {
            get { return _cred; }
        }

        public BitmapImage Thumbnail
        {
            get { return null; }
        }

        public bool IsSelected { get; set; }

        public ICommand EditCommand
        {
            get { return null; }
        }

        public ICommand ConnectCommand
        {
            get { return null; }
        }

        public ICommand DeleteCommand
        {
            get { return null; }
        }

        public bool SelectionEnabled { get; set; }

        // default size for most screens: Height="164" Width="296"
        Size _size = new Size(296,164);
        Size ISizeableTile.TileSize
        {
            get { return _size; }
        }

        public Size ScreenSize { set; private get; }

        void IRemoteConnectionViewModel.Presenting(ISessionFactory sessionFactory)
        {
        }

        void IRemoteConnectionViewModel.Dismissed()
        {
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }
    }
}
