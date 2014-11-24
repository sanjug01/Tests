using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.ValidationRules;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public class AddOrEditDesktopViewModelArgs
    {
        public AddOrEditDesktopViewModelArgs(Desktop desktop, Credentials credentials, bool isAddingDesktop = true)
        {
            this.Desktop = desktop;
            this.Credentials = credentials;
            this.IsAddingDesktop = isAddingDesktop;
        }

        public Desktop Desktop { get; private set; }
        public Credentials Credentials { get; private set; }
        public bool IsAddingDesktop { get; private set; }        
    }
    public class AddOrEditDesktopViewModel : ViewModelBase
    {
        private bool _isAddingDesktop;
        private string _host;
        private bool _isHostValid;
        private bool _isExpandedView;

        private readonly RelayCommand _saveCommand;
        private readonly RelayCommand _cancelCommand;

        private Desktop _desktop;
        private Credentials _credentials;

        private int _selectedUserOptionsIndex;

        public AddOrEditDesktopViewModel()
        {
            _saveCommand = new RelayCommand(SaveCommandExecute, 
                o => this.SaveCommandCanExecute());
            _cancelCommand = new RelayCommand(CancelCommandExecute);

            IsAddingDesktop = true;
            IsHostValid = true;

            _desktop = null;
            _credentials = null;

            this.UserOptions = new ObservableCollection<string>();
            this.UserOptions.Add("Enter every time(d)");
            this.UserOptions.Add("Add credentials(d)");
            this.SelectedUserOptionsIndex = 0;

            this.ResetCachedDesktopData();

            PresentableView = null;
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
            private set 
            {
                // credentials will  be set only on presenting, from activationParameter
                // which will also update the credentials's combobox
                SetProperty(ref _credentials, value, "Credentials");
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
                _saveCommand.EmitCanExecuteChanged();
            }
        }

        public bool IsHostValid
        {
            get { return _isHostValid; }
            private set { SetProperty(ref _isHostValid, value, "IsHostValid"); }
        }


        private void SaveCommandExecute(object o)
        {
            if (!this.Validate())
            {
                // save cannot complete if validation fails.
                return;
            }

            // saving through _saveDelegate for both edit or add
            if (IsAddingDesktop)
            {
                _desktop = new Desktop() { HostName = this.Host };
                this.DataModel.Desktops.Add(_desktop);
            }
            else
            {
                _desktop.HostName = this.Host;
            }

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
                //  Could do extra validation of the hostname
                //
                canExecute = true;
            }
            return canExecute;
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

        /// <summary>
        /// This method performs all validation tests.
        ///     Currently the validation is performed only on Save command
        /// </summary>
        /// <returns>true, if all validations pass</returns>
        private bool Validate()
        {
            HostNameValidationRule rule = new HostNameValidationRule();
            bool isValid = true;
            if (!(this.IsHostValid = rule.Validate(this.Host, System.Globalization.CultureInfo.CurrentCulture)))
            {
                isValid = isValid && this.IsHostValid;
            }

            return isValid;
        }


        protected override void OnPresenting(object activationParameter)
        {
            Contract.Requires(null != activationParameter as AddOrEditDesktopViewModelArgs);
            AddOrEditDesktopViewModelArgs args = activationParameter as AddOrEditDesktopViewModelArgs;

            this.IsAddingDesktop = args.IsAddingDesktop;
            this.Desktop = args.Desktop;
            this.Credentials = args.Credentials;

            // update combo box and combo box selection, if necessary
            if (null != this.Credentials)
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
}
