using System.Collections.ObjectModel;

namespace RdClient.Shared.ViewModels
{
    public interface IConnectionCenterViewModel
    {
        RelayCommand AddDesktopCommand { get; }
        ReadOnlyObservableCollection<IDesktopViewModel> DesktopViewModels { get; }
        bool HasDesktops { get; }
    }
}
