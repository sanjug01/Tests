using RdClient.Navigation;
using RdClient.Shared.CxWrappers;
using System;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public interface IAddOrEditDesktopViewModel
    {
        ICommand SaveCommand { get; }
        ICommand CancelCommand { get; }
        INavigationService NavigationService { set; }
    }
}
