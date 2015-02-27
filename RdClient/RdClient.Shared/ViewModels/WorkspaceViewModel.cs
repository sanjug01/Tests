namespace RdClient.Shared.ViewModels
{
    using System.Collections.ObjectModel;

    public class WorkspaceViewModel : Helpers.MutableObject, IWorkspaceViewModel
    {
        private readonly ObservableCollection<IRemoteApplicationViewModel> _remoteAppViewModelsSource;
        private readonly ReadOnlyObservableCollection<IRemoteApplicationViewModel> _remoteAppViewModels;

        public WorkspaceViewModel()
        {
            _remoteAppViewModelsSource = new ObservableCollection<IRemoteApplicationViewModel>();
            _remoteAppViewModels = new ReadOnlyObservableCollection<IRemoteApplicationViewModel>(_remoteAppViewModelsSource);
            _remoteAppViewModelsSource.Add(new RemoteApplicationViewModel());
            _remoteAppViewModelsSource.Add(new RemoteApplicationViewModel());
            _remoteAppViewModelsSource.Add(new RemoteApplicationViewModel());
            _remoteAppViewModelsSource.Add(new RemoteApplicationViewModel());
            _remoteAppViewModelsSource.Add(new RemoteApplicationViewModel());
            _remoteAppViewModelsSource.Add(new RemoteApplicationViewModel());
            _remoteAppViewModelsSource.Add(new RemoteApplicationViewModel());
            _remoteAppViewModelsSource.Add(new RemoteApplicationViewModel());
            _remoteAppViewModelsSource.Add(new RemoteApplicationViewModel());
            _remoteAppViewModelsSource.Add(new RemoteApplicationViewModel());
        }

        public ReadOnlyObservableCollection<IRemoteApplicationViewModel> RemoteApplicationViewModels
        {
            get { return _remoteAppViewModels; }
        }
    }
}
