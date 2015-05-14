namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Navigation;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    public interface IConnectionCenterViewModel
    {
        RelayCommand AddDesktopCommand { get; }
        ReadOnlyObservableCollection<IDesktopViewModel> DesktopViewModels { get; }
        ReadOnlyObservableCollection<IWorkspaceViewModel> WorkspaceViewModels { get; }

        bool HasDesktops { get; }
        bool HasApps { get; }
        bool ShowDesktops { get; set; }
        bool ShowApps { get; set; }

        IViewVisibility AccessoryViewVisibility { get; }
        ICommand CancelAccessoryView { get; }
    }
}
