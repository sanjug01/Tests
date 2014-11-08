using RdClient.Navigation;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Utils;
using RdClient.Shared.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public class AddOrEditDesktopViewModel : ViewModelBase
    {
        public AddOrEditDesktopViewModel()
        {
            _saveCommand = new RelayCommand(SaveCommandExecute);
            _cancelCommand = new RelayCommand(CancelCommandExecute);

            IsAddingDesktop = true;
            _desktop = null;
            
            this.ResetCachedDesktopData();

            PresentableView = null;
        }

        public IPresentableView PresentableView { private get; set; }

        public ICommand SaveCommand { get { return _saveCommand; } }

        public ICommand CancelCommand { get { return _cancelCommand; } }
        
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

        public string Host
        {
            get { return _host; }
            set
            {
                SetProperty(ref _host, value, "Host");
            }
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

        private void ResetCachedDesktopData()
        {
            if (null != _desktop)
            {
                this.Host = _desktop.hostName;
            }
            else
            {
                this.Host = string.Empty;
            }
        }

        protected override void OnPresenting(object activationParameter)
        {
            if (null == activationParameter)
            {
                // add
                IsAddingDesktop = true;
                Desktop = null;
            }
            else
            {
                //edit
                IsAddingDesktop = false;
                Desktop = activationParameter as Desktop;
            }
        }

        private bool _isAddingDesktop;
        private string _host;

        private readonly RelayCommand _saveCommand;
        private readonly RelayCommand _cancelCommand;

        private Desktop _desktop;


    }
}
