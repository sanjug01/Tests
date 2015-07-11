namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Navigation;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using Windows.Foundation;

    public interface IConnectionCenterViewModel
    {
        RelayCommand AddDesktopCommand { get; }
        ReadOnlyObservableCollection<IDesktopViewModel> DesktopViewModels { get; }
        IViewItemsSource DesktopViewModelsSource { get; }
        ReadOnlyObservableCollection<IWorkspaceViewModel> WorkspaceViewModels { get; }

        bool HasDesktops { get; }
        bool HasApps { get; }
        bool ShowDesktops { get; set; }
        bool ShowApps { get; set; }

        Size DesktopTileSize { get; set; }

        IViewVisibility AccessoryViewVisibility { get; }
        ICommand CancelAccessoryView { get; }
    }
}
