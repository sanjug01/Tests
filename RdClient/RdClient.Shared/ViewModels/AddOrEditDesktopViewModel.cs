using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.ValidationRules;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public class AddDesktopViewModelArgs
    {
        public AddDesktopViewModelArgs()
        {
        }    
    }
    public class EditDesktopViewModelArgs
    {
        private readonly Desktop _desktop;
        public Desktop Desktop { get {return _desktop; } }

        public EditDesktopViewModelArgs(Desktop desktop)
        {
            _desktop = desktop;
        }
    }

    public enum UserComboBoxType
    {
        Credentials,
        AskEveryTime,
        AddNew
    }

    public class UserComboBoxElement
    {
        private readonly Credentials _credentials;
        public Credentials Credentials { get { return _credentials; } }

        private readonly UserComboBoxType _userComboBoxType;
        public UserComboBoxType UserComboBoxType { get { return _userComboBoxType; } }

        public UserComboBoxElement(UserComboBoxType userComboBoxType, Credentials  credentials = null)
        {            
            _userComboBoxType  = userComboBoxType;
            _credentials = credentials;
        }

        public override string ToString() 
        {
            string description;

            switch (this.UserComboBoxType)
            {
                case UserComboBoxType.AddNew:
                    description = "Add New (d)";
                    break;
                case UserComboBoxType.AskEveryTime:
                    description = "Ask Every Time (d)";
                    break;
                case UserComboBoxType.Credentials:
                    description = this.Credentials.Username;
                    break;
                default:
                    description = "";
                    break;
            }

            return description;
        }
    }

    public class AddOrEditDesktopViewModel : ViewModelBase
    {
        private string _host;
        private bool _isHostValid;
        private bool _isAddingDesktop;

        private readonly RelayCommand _saveCommand;
        private readonly RelayCommand _cancelCommand;
        private Desktop _desktop;
        private int _selectedUserOptionsIndex;

        public bool IsAddingDesktop
        {
            get { return _isAddingDesktop; }
            private set 
            {
                this.SetProperty(ref _isAddingDesktop, value, "IsAddingDesktop");
            }
        }

        public ObservableCollection<UserComboBoxElement> UserOptions { get; set; }
        public int SelectedUserOptionsIndex 
        { 
            get { return _selectedUserOptionsIndex; }
            set
            {
                SetProperty(ref _selectedUserOptionsIndex, value, "SelectedUserOptionsIndex");

                int idx = (int) value;
                if(this.UserOptions.Count > 0 && this.UserOptions[idx].UserComboBoxType == UserComboBoxType.AddNew)
                {
                    AddUserViewArgs args = new AddUserViewArgs((credentials, store) =>
                        {
                            this.Desktop.CredentialId = credentials.Id;
                            this.DataModel.Credentials.Add(credentials);
                            Update();
                        }
                        , false);
                    NavigationService.PushModalView("AddUserView", args);
                }
            }
        }

        public IPresentableView PresentableView { private get; set; }

        public ICommand SaveCommand { get { return _saveCommand; } }

        public ICommand CancelCommand { get { return _cancelCommand; } }

        public Desktop Desktop
        {
            get { return _desktop; }
            private set { this.SetProperty<Desktop>(ref _desktop, value); }
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

        public AddOrEditDesktopViewModel()
        {
            _saveCommand = new RelayCommand(SaveCommandExecute,
                o =>
                {
                    return (string.IsNullOrEmpty(this.Host) == false);
                });
            _cancelCommand = new RelayCommand(CancelCommandExecute);

            IsHostValid = true;

            this.UserOptions = new ObservableCollection<UserComboBoxElement>();
            this.SelectedUserOptionsIndex = 0;
        }

        private void SaveCommandExecute(object o)
        {
            if (!this.Validate())
            {
                // save cannot complete if validation fails.
                return;
            }

            this.Desktop.HostName = this.Host;
            if (null != this.UserOptions[this.SelectedUserOptionsIndex].Credentials)
            {
                this.Desktop.CredentialId = this.UserOptions[this.SelectedUserOptionsIndex].Credentials.Id;
            }

            bool found = false;
            foreach (Desktop desktop in this.DataModel.Desktops)
            {
                if (desktop.Id == this.Desktop.Id)
                {
                    found = true;
                    break;
                }
            }

            if (found == false)
            {
                this.DataModel.Desktops.Add(this.Desktop);
            }

            NavigationService.DismissModalView(PresentableView);
        }

        private void CancelCommandExecute(object o)
        {
            NavigationService.DismissModalView(PresentableView);
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

        private void Update()
        {
            this.UserOptions.Clear();
            this.UserOptions.Add(new UserComboBoxElement(UserComboBoxType.AskEveryTime));
            this.UserOptions.Add(new UserComboBoxElement(UserComboBoxType.AddNew));
            foreach (Credentials credentials in this.DataModel.Credentials)
            {
                this.UserOptions.Add(new UserComboBoxElement(UserComboBoxType.Credentials, credentials));
            }

            if (this.Desktop.HasCredential)
            {
                int idx = 0;
                for (idx = 0; idx < this.UserOptions.Count; idx++)
                {
                    if (this.UserOptions[idx].UserComboBoxType == UserComboBoxType.Credentials &&
                        this.UserOptions[idx].Credentials.Id == this.Desktop.CredentialId)
                        break;
                }

                if (idx == this.UserOptions.Count)
                {
                    idx = 0;
                }

                this.SelectedUserOptionsIndex = idx;
            }
        }


        protected override void OnPresenting(object activationParameter)
        {
            Contract.Requires(null != activationParameter);

            AddDesktopViewModelArgs addArgs = activationParameter as AddDesktopViewModelArgs;
            EditDesktopViewModelArgs editArgs = activationParameter as EditDesktopViewModelArgs;
            if (editArgs != null)
            {
                this.Desktop = editArgs.Desktop;
                this.Host = this.Desktop.HostName;
                this.IsAddingDesktop = false;
            }
            else if(addArgs != null)
            {
                this.Desktop = new Desktop();
                this.IsAddingDesktop = true;
            }

            Update();
        }
    }
}
