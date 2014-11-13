using RdClient.Navigation;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Utils;
using RdClient.Shared.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public delegate void SaveDesktopDelegate(Desktop desktop);
    public class AddOrEditDesktopViewModelArgs
    {
        private readonly SaveDesktopDelegate _saveDelegate;

        public AddOrEditDesktopViewModelArgs(Desktop desktop, Credentials credentials, bool isAddingDesktop = true, SaveDesktopDelegate saveDelegate=null)
        {
            this.Desktop = desktop;
            this.Credentials = credentials;
            this.IsAddingDesktop = isAddingDesktop;
            _saveDelegate = saveDelegate;
        }

        public Desktop Desktop { get; private set; }
        public Credentials Credentials { get; private set; }
        public bool IsAddingDesktop { get; private set; }
        
        public SaveDesktopDelegate SaveDelegate { get { return _saveDelegate; } }

    }
    public class AddOrEditDesktopViewModel : ViewModelBase
    {
        private bool _isAddingDesktop;
        private string _host;

        private readonly RelayCommand _saveCommand;
        private readonly RelayCommand _cancelCommand;

        private Desktop _desktop;
        private Credentials _credentials;

        private int _selectedUserOptionsIndex;
        private SaveDesktopDelegate _saveDelegate;

        public AddOrEditDesktopViewModel()
        {
            _saveCommand = new RelayCommand(SaveCommandExecute);
            _cancelCommand = new RelayCommand(CancelCommandExecute);

            IsAddingDesktop = true;
            _desktop = null;
            _credentials = null;

            this.UserOptions = new ObservableCollection<string>();
            this.UserOptions.Add("Enter every time(d)");
            this.UserOptions.Add("Add credentials(d)");
            this.SelectedUserOptionsIndex = 0;

            this.ResetCachedDesktopData();

            PresentableView = null;
            _saveDelegate = null;
        }

        public ObservableCollection<string> UserOptions { get; set; }
        public int SelectedUserOptionsIndex 
        { 
            get { return _selectedUserOptionsIndex; }
            set
            {
                SetProperty(ref _selectedUserOptionsIndex, value, "SelectedUserOptionsIndex");
            }
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

        public Credentials Credentials
        {
            get { return _credentials; }
            set
            {
                SetProperty(ref _credentials, value, "Credentials");

                // update combo box and combo box selection, if necesary
                if(null != _credentials)
                {
                    int idx = this.UserOptions.IndexOf(Credentials.Username);
                    if (0 <= idx)
                    {
                        this.SelectedUserOptionsIndex = idx;
                    }
                    else
                    { 
                        this.UserOptions.Insert(0, Credentials.Username);
                        this.SelectedUserOptionsIndex = 0;
                    }

                }
            }
        }

        public bool IsAddingDesktop {
            get { return _isAddingDesktop; }
            private set { SetProperty(ref _isAddingDesktop, value, "IsAddingDesktop"); }
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
            // saving through _saveDelegate for both edit or add
            if (IsAddingDesktop)
            {
                _desktop = new Desktop() { HostName = this.Host };
                if (null != _saveDelegate)
                {
                    _saveDelegate(_desktop);
                }
            }
            else
            {
                _desktop.HostName = this.Host;
                if (null != _saveDelegate)
                {
                    _saveDelegate(_desktop);
                }
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
                this.Host = _desktop.HostName;
            }
            else
            {
                this.Host = string.Empty;
            }

        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Requires(null != activationParameter as AddOrEditDesktopViewModelArgs);
            AddOrEditDesktopViewModelArgs args = activationParameter as AddOrEditDesktopViewModelArgs;

            this.IsAddingDesktop = args.IsAddingDesktop;
            this.Desktop = args.Desktop;
            this.Credentials = args.Credentials;
            this._saveDelegate = args.SaveDelegate;
        }
    }
}
