namespace RdClient.Shared.ViewModels
{
    using System.Collections.ObjectModel;

    public interface IWorkspaceViewModel
    {
        ReadOnlyObservableCollection<IRemoteApplicationViewModel> RemoteApplicationViewModels { get; }
    }
}
