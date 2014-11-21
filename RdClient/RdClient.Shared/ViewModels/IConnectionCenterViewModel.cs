using System;
namespace RdClient.Shared.ViewModels
{
    public interface IConnectionCenterViewModel
    {
        System.Windows.Input.ICommand AddDesktopCommand { get; }
        System.Collections.ObjectModel.ObservableCollection<IDesktopViewModel> DesktopViewModels { get; }
        bool HasDesktops { get; }
    }
}
