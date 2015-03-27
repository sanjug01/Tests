namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

    public interface IWorkspaceViewModel
    {
        string Name { get; }
        string State { get; }
        string Error { get; }
        ICommand DeleteCommand { get; }
        ICommand EditCommand { get; }
        ICommand RefreshCommand { get; }
        List<IRemoteResourceViewModel> RemoteResourceViewModels { get; }
    }
}
