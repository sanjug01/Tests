using RdClient.Shared.Converters;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.Navigation.Extensions;
using System;
using System.ComponentModel;
using System.Windows.Input;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace RdClient.Shared.ViewModels
{
    public sealed class RemoteResourceViewModel : Helpers.MutableObject, IRemoteResourceViewModel
    {
        private RemoteResourceModel _remoteResource;
        private string _name;
        private readonly RelayCommand _connectCommand;
        private BitmapImage _icon;
        private INavigationService _navigationService;
        private ISessionFactory _sessionFactory;
        private ApplicationDataModel _dataModel;

        public RemoteResourceViewModel(
            RemoteResourceModel remoteResource,
            ApplicationDataModel dataModel,
            IExecutionDeferrer dispatcher,
            INavigationService navigationService,
            ISessionFactory sessionFactory)
        {
            base.ExecutionDeferrer = dispatcher;
            _dataModel = dataModel;
            _remoteResource = remoteResource;
            _navigationService = navigationService;
            _sessionFactory = sessionFactory;
            _remoteResource.PropertyChanged += RemoteResourcePropertyChanged;
            _connectCommand = new RelayCommand(o => ConnectCommandExecute());
            SetName();
            SetIcon();
        }

        private void ConnectCommandExecute()
        {
            RemoteSessionSetup sessionSetup = new RemoteSessionSetup(_dataModel, _remoteResource);
            IRemoteSession session = _sessionFactory.CreateSession(sessionSetup);
            _navigationService.NavigateToView("RemoteSessionView", session);            
        }

        public string Name
        {
            get { return _name; }
            private set { SetProperty(ref _name, value); }
        }

        public ICommand ConnectCommand
        {
            get { return _connectCommand; }
        }

        public BitmapImage Icon
        {
            get { return _icon; }
            private set { SetProperty(ref _icon, value); }            
        }

        private void RemoteResourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender != _remoteResource)
            {
                throw new InvalidOperationException();
            }

            if ("FriendlyName".Equals(e.PropertyName))
            {
                SetName();
            }
            else if ("IconBytes".Equals(e.PropertyName))
            {
                SetIcon();
            }
        }

        private void SetName()
        {
            this.Name = _remoteResource.FriendlyName ?? "";
        }

        private void SetIcon()
        {
            this.ExecutionDeferrer.TryDeferToUI(() =>
            {
                byte[] iconBytes = _remoteResource.IconBytes;
                if (iconBytes == null)
                {
                    this.Icon = null;
                }
                else
                {
                    IValueConverter converter = new BytesToBitmapConverter();
                    this.Icon = converter.Convert(iconBytes, typeof(BitmapImage), null, null) as BitmapImage;
                }
            });
        }
    }
}
