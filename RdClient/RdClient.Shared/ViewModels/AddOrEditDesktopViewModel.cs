using RdClient.Navigation;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Utils;
using RdClient.Shared.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public class AddOrEditDesktopViewModel : ViewModelBase , IAddOrEditDesktopViewModel
    {
        public AddOrEditDesktopViewModel()
        {
            predicateCanExecute = (o) => { return SaveCommandCanExecute(); };
            _saveCommand = new RelayCommand(SaveCommandExecute, predicateCanExecute);
            _cancelCommand = new RelayCommand(CancelCommandExecute);

            this.GatewayOptions = new ObservableCollection<string>();
            this.GatewayOptions.Add("No gateway(d)");
            this.GatewayOptions.Add("Add gateway(d)");
            this.SelectedGatewayOptionsIndex = 0;

            this.UserOptions = new ObservableCollection<string>();
            this.UserOptions.Add("Enter every time(d)");
            this.UserOptions.Add("Add credentials(d)");
            this.SelectedUserOptionsIndex = 0;

            IsAddingDesktop = true;
            _desktop = null;

            this.IsExpandedViewVisible = false;
            this.ResetCachedDesktopData();

            PresentableView = null;
        }

        public IPresentableView PresentableView { private get; set; }

        public INavigationService NavigationService { private get; set; }

        public ICommand SaveCommand { get { return _saveCommand; } }

        public ICommand CancelCommand { get { return _cancelCommand; } }

        public ObservableCollection<string> GatewayOptions { get; set; }

        public int SelectedGatewayOptionsIndex { get; set; }

        public ObservableCollection<string> UserOptions { get; set; }

        public int SelectedUserOptionsIndex { get; set; }

        public Desktop Desktop
        {
            get { return _desktop; }
            set
            {
                SetProperty(ref _desktop, value, "Desktop");
                ResetCachedDesktopData();
            }
        }

        public bool IsAddingDesktop {
            get { return _isAddingDesktop; }
            set { SetProperty(ref _isAddingDesktop, value, "IsAddingDesktop"); }
        }

        public string FriendlyName
        {
            get { return _friendlyName; }
            set { SetProperty(ref _friendlyName, value, "FriendlyName"); }
        }

        public string Host
        {
            get { return _host; }
            set
            {
                SetProperty(ref _host, value, "Host");
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsConnectToAdminSession
        {
            get { return _isConnectToAdminSession; }
            set { SetProperty(ref _isConnectToAdminSession, value, "IsConnectToAdminSession"); }
        }

        public bool IsExpandedViewVisible
        {
            get { return _isExpandedViewVisible; }
            set { SetProperty(ref _isExpandedViewVisible, value, "IsExpandedViewVisible"); }
        }

        public bool IsSwapMouseButtons
        {
            get { return _isSwapMouseButtons; }
            set { SetProperty(ref _isSwapMouseButtons, value, "IsSwapMouseButtons"); }
        }

        public int AudioMode
        {
            get { return _audioMode; }
            set { SetProperty(ref _audioMode, value, "AudioMode"); }
        }

        private void SaveCommandExecute(object o)
        {
            // TODO
            if (IsAddingDesktop)
            {
                // TODO : add new desktop
            }
            else
            {
                // TODO : Refresh list of desktops                
            }

            this.ResetCachedDesktopData();

            if (null != NavigationService && null != PresentableView)
            {
                NavigationService.DismissModalView(PresentableView);
            }
        }

        private void CancelCommandExecute(object o)
        {
            this.ResetCachedDesktopData();

            if (null != NavigationService && null != PresentableView)
            {
                NavigationService.DismissModalView(PresentableView);
            }
        }

        private bool SaveCommandCanExecute()
        {
            bool canExecute = false;

            if (this.Host.Trim().Length > 0)
            {
                //
                //  TODO: Add proper validation of the host
                //
                canExecute = true;
            }
            return canExecute;
        }

        private void ResetCachedDesktopData()
        {
            // TODO - need to use gateway and user settings too
            if (null != _desktop)
            {
                this.Host = _desktop.hostName;
                this.FriendlyName = "N/A";              // _desktop.FriendlyName;  -- not implemented
                this.IsSwapMouseButtons = true;         // _desktop.IsLeftHandedMouseButtons; -- not implemented
                this.IsConnectToAdminSession = true;    //  _desktop.IsConsoleMode; -- not implemented
                this.AudioMode = 1;                     //  _desktop.AudioMode; -- not implemented
            }
            else
            {
                this.Host = string.Empty;
                this.FriendlyName = string.Empty;
                this.IsSwapMouseButtons = false;
                this.IsConnectToAdminSession = false;
                this.AudioMode = 0;

                this.SelectedUserOptionsIndex = 0;
                this.SelectedGatewayOptionsIndex = 0;
            }
        }

        private bool _isAddingDesktop;
        private string _friendlyName;
        private string _host;
        private bool _isExpandedViewVisible;
        private bool _isSwapMouseButtons;
        private bool _isConnectToAdminSession;
        private int _audioMode;

        Predicate<object> predicateCanExecute; 
        private readonly RelayCommand _saveCommand;
        private readonly RelayCommand _cancelCommand;

        private Desktop _desktop;


    }
}
