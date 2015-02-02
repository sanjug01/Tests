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
    }

    public class AddOrEditDesktopViewModel : ViewModelBase
    {
        private string _host;
        private bool _isHostValid;
        private bool _isAddingDesktop;
        private bool _isExpandedView;
        private string _friendlyName;
        private bool _isSwapMouseButtons;
        private bool _isUseAdminSession;
        private int _audioMode;

        private readonly RelayCommand _saveCommand;
        private readonly RelayCommand _cancelCommand;
        private readonly RelayCommand _showDetailsCommand;
        private readonly RelayCommand _hideDetailsCommand;
        private Desktop _desktop;
        private int _selectedUserOptionsIndex;

        public AddOrEditDesktopViewModel()
        {
            _saveCommand = new RelayCommand(SaveCommandExecute,
                o =>
                {
                    return (string.IsNullOrEmpty(this.Host) == false);
                });
            _cancelCommand = new RelayCommand(CancelCommandExecute);
            _showDetailsCommand = new RelayCommand((o) => { this.IsExpandedView = true; });
            _hideDetailsCommand = new RelayCommand((o) => { this.IsExpandedView = false; });

            IsHostValid = true;

            this.UserOptions = new ObservableCollection<UserComboBoxElement>();
            this.SelectedUserOptionsIndex = 0;
            this.IsExpandedView = false;
        }

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
                    AddUserViewArgs args = new AddUserViewArgs(new Credentials(), false);
                    ModalPresentationCompletion addUserCompleted = new ModalPresentationCompletion();
                    addUserCompleted.Completed += (s, e) =>
                    {
                        CredentialPromptResult result = e.Result as CredentialPromptResult;
                        if (result != null && !result.UserCancelled)
                        {
                            this.Desktop.CredentialId = result.Credential.Id;
                            this.DataModel.LocalWorkspace.Credentials.Add(result.Credential);
                            
                        }
                        Update();
                    };
                    NavigationService.PushModalView("AddUserView", args, addUserCompleted);
                }
            }
        }

        public IPresentableView PresentableView { private get; set; }

        public ICommand SaveCommand { get { return _saveCommand; } }

        public ICommand CancelCommand { get { return _cancelCommand; } }

        public ICommand ShowDetailsCommand { get { return _showDetailsCommand; } }
        public ICommand HideDetailsCommand { get { return _hideDetailsCommand; } }

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
                SetProperty(ref _host, value); 
                _saveCommand.EmitCanExecuteChanged();
            }
        }

        public bool IsHostValid
        {
            get { return _isHostValid; }
            private set { SetProperty(ref _isHostValid, value); }
        }

        public bool IsExpandedView
        {
            get { return _isExpandedView; }
            set { SetProperty(ref _isExpandedView, value); }
        }

        public string FriendlyName
        {
            get { return _friendlyName; }
            set { SetProperty(ref _friendlyName, value); }
        }

        public bool IsUseAdminSession
        {
            get { return _isUseAdminSession; }
            set { SetProperty(ref _isUseAdminSession, value); }
        }

        public bool IsSwapMouseButtons
        {
            get { return _isSwapMouseButtons; }
            set { SetProperty(ref _isSwapMouseButtons, value); }
        }

        public int AudioMode
        {
            get { return _audioMode; }
            set { SetProperty(ref _audioMode, value); }
        }

        private void SaveCommandExecute(object o)
        {
            if (!this.Validate())
            {
                // save cannot complete if validation fails.
                return;
            }

            this.Desktop.HostName = this.Host;
            this.Desktop.FriendlyName = this.FriendlyName;
            this.Desktop.IsUseAdminSession = this.IsUseAdminSession;
            this.Desktop.IsSwapMouseButtons = this.IsSwapMouseButtons;
            this.Desktop.AudioMode = (Desktop.AudioModes) this.AudioMode;

            if (null != this.UserOptions[this.SelectedUserOptionsIndex].Credentials)
            {
                this.Desktop.CredentialId = this.UserOptions[this.SelectedUserOptionsIndex].Credentials.Id;
            }

            bool found = this.DataModel.LocalWorkspace.Connections.ContainsItemWithId(this.Desktop.Id);

            if (found == false)
            {
                this.DataModel.LocalWorkspace.Connections.Add(this.Desktop);
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
            foreach (Credentials credentials in this.DataModel.LocalWorkspace.Credentials)
            {
                this.UserOptions.Add(new UserComboBoxElement(UserComboBoxType.Credentials, credentials));
            }

            int idx = 0;
            for (idx = 0; idx < this.UserOptions.Count; idx++)
            {
                if (this.Desktop.HasCredential &&
                    this.UserOptions[idx].UserComboBoxType == UserComboBoxType.Credentials &&
                    this.UserOptions[idx].Credentials.Id == this.Desktop.CredentialId)
                    break;
            }

            if (idx == this.UserOptions.Count)
            {
                idx = 0;
            }

            this.SelectedUserOptionsIndex = idx;            
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
                this.FriendlyName = this.Desktop.FriendlyName;
                this.AudioMode = (int) this.Desktop.AudioMode;
                this.IsSwapMouseButtons = this.Desktop.IsSwapMouseButtons;
                this.IsUseAdminSession = this.Desktop.IsUseAdminSession;

                this.IsAddingDesktop = false;
            }
            else if(addArgs != null)
            {
                this.Desktop = new Desktop(this.DataModel.LocalWorkspace);
                this.FriendlyName = string.Empty;
                this.AudioMode = (int) Desktop.AudioModes.Local;
                this.IsSwapMouseButtons = false;
                this.IsUseAdminSession = false;

                this.IsAddingDesktop = true;
            }
            this.IsExpandedView = false;

            Update();
        }
    }
}
