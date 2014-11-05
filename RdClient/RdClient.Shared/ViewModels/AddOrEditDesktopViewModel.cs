using RdClient.Navigation;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Utils;
using RdClient.Shared.Models;
using System;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public class AddOrEditDesktopViewModel : ViewModelBase , IAddOrEditDesktopViewModel
    {
        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public INavigationService NavigationService { private get; set; }

        public AddOrEditDesktopViewModel()
        {
            SaveCommand = new RelayCommand(new Action<object>(SaveDesktop));
            CancelCommand = new RelayCommand(new Action<object>(CancelChanges));
        }

        private void SaveDesktop(object o)
        {
            // TODO
            if (NavigationService != null)
            {
                NavigationService.NavigateToView("view1", null);
            }
        }

        private void CancelChanges(object o)
        {
            // TODO
            if (NavigationService != null)
            {
                NavigationService.NavigateToView("view1", null);
            }
        }
    }
}
