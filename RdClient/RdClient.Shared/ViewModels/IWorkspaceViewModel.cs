﻿namespace RdClient.Shared.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    public interface IWorkspaceViewModel
    {
        string Name { get; }
        ICommand DeleteCommand { get; }
        ICommand EditCommand { get; }
        ICommand RefreshCommand { get; }
        ReadOnlyObservableCollection<IRemoteResourceViewModel> RemoteResourceViewModels { get; }
    }
}
