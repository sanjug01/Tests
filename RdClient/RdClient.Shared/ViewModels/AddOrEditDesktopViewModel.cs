using RdClient.Shared.Data;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.ValidationRules;
using RdClient.Shared.ViewModels.EditCredentialsTasks;
using System;
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
        private readonly DesktopModel _desktop;

        public DesktopModel Desktop { get {return _desktop; } }

        public EditDesktopViewModelArgs(DesktopModel desktop)
        {
            _desktop = desktop;
        }
    }

    public class AddOrEditDesktopViewModel : ViewModelBase, IDialogViewModel
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
        private DesktopModel _desktop;
        private int _selectedUserOptionsIndex;
        private int _selectedGatewayOptionsIndex;

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
            this.GatewayOptions = new ObservableCollection<GatewayComboBoxElement>();
            //this.SelectedUserOptionsIndex = 0;
            this.IsExpandedView = false;
        }

        public ObservableCollection<UserComboBoxElement> UserOptions { get; private set; }

        public ObservableCollection<GatewayComboBoxElement> GatewayOptions { get; private set; }

        public IPresentableView PresentableView { private get; set; }
        public ICommand DefaultAction { get { return _saveCommand; } }
        public ICommand Cancel { get { return _cancelCommand; } }
        public ICommand ShowDetailsCommand { get { return _showDetailsCommand; } }
        public ICommand HideDetailsCommand { get { return _hideDetailsCommand; } }

        public bool IsAddingDesktop
        {
            get { return _isAddingDesktop; }
            private set
            {
                this.SetProperty(ref _isAddingDesktop, value, "IsAddingDesktop");
            }
        }

        public int SelectedUserOptionsIndex 
        { 
            get { return _selectedUserOptionsIndex; }

            set
            {
                if (SetProperty(ref _selectedUserOptionsIndex, value))
                {
                    if (value >= 0 && UserComboBoxType.AddNew == this.UserOptions[value].UserComboBoxType)
                    {
                        this.LaunchAddUserView();
                    }
                }
            }
        }

        public int SelectedGatewayOptionsIndex
        {
            get { return _selectedGatewayOptionsIndex; }

            set
            {
                if (SetProperty(ref _selectedGatewayOptionsIndex, value))
                {
                    if (value >= 0 && GatewayComboBoxType.AddNew == this.GatewayOptions[value].GatewayComboBoxType)
                    {
                        this.LaunchAddGatewayView();
                    }
                }
            }
        }

        public DesktopModel Desktop
        {
            get { return _desktop; }
            private set { this.SetProperty(ref _desktop, value); }
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
            if (this.Validate())
            {
                this.Desktop.HostName = this.Host;

                this.Desktop.HostName = this.Host;
                this.Desktop.FriendlyName = this.FriendlyName;
                this.Desktop.IsAdminSession = this.IsUseAdminSession;
                this.Desktop.IsSwapMouseButtons = this.IsSwapMouseButtons;
                this.Desktop.AudioMode = (AudioMode) this.AudioMode;

                if (null != this.UserOptions[this.SelectedUserOptionsIndex].Credentials)
                {
                    this.Desktop.CredentialsId = this.UserOptions[this.SelectedUserOptionsIndex].Credentials.Id;
                }
                else
                {
                    this.Desktop.CredentialsId = Guid.Empty;
                }

                if (null != this.GatewayOptions[this.SelectedGatewayOptionsIndex].Gateway)
                {
                    this.Desktop.GatewayId = this.GatewayOptions[this.SelectedGatewayOptionsIndex].Gateway.Id;
                }
                else
                {
                    this.Desktop.GatewayId = Guid.Empty;
                }

                if (this.IsAddingDesktop)
                {
                    this.ApplicationDataModel.LocalWorkspace.Connections.AddNewModel(this.Desktop);
                }

                this.DismissModal(null);
            }
        }

        private void CancelCommandExecute(object o)
        {
            this.DismissModal(null);
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
            if (!(this.IsHostValid = rule.Validate(this.Host).IsValid))
            {
                isValid = isValid && this.IsHostValid;
            }

            return isValid;
        }

        private void Update()
        {
            // load users list
            this.LoadUsers();
            this.SelectUserId(this.Desktop.CredentialsId);

            // load gateways list
            this.LoadGateways();
            this.SelectGatewayId(this.Desktop.GatewayId);
        }


        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(null != activationParameter);

            AddDesktopViewModelArgs addArgs = activationParameter as AddDesktopViewModelArgs;
            EditDesktopViewModelArgs editArgs = activationParameter as EditDesktopViewModelArgs;

            if (editArgs != null)
            {
                this.Desktop = editArgs.Desktop;
                this.Host = this.Desktop.HostName;
                this.FriendlyName = this.Desktop.FriendlyName;
                this.AudioMode = (int) this.Desktop.AudioMode;
                this.IsSwapMouseButtons = this.Desktop.IsSwapMouseButtons;
                this.IsUseAdminSession = this.Desktop.IsAdminSession;

                this.IsAddingDesktop = false;
            }
            else if(addArgs != null)
            {
                this.Desktop = new DesktopModel();
                this.FriendlyName = string.Empty;
                this.AudioMode = (int)RdClient.Shared.Models.AudioMode.Local;
                this.IsSwapMouseButtons = false;
                this.IsUseAdminSession = false;

                this.IsAddingDesktop = true;
            }
            this.IsExpandedView = false;

            Update();
        }

        private void GatewayPromptResultHandler(object sender, PresentationCompletionEventArgs args)
        {
            GatewayPromptResult result = args.Result as GatewayPromptResult;

            if (result != null && !result.UserCancelled)
            {
                this.LoadGateways();
                this.SelectGatewayId(result.GatewayId);
            }
        }

        private void LaunchAddGatewayView()
        {
            AddGatewayViewModelArgs args = new AddGatewayViewModelArgs();
            ModalPresentationCompletion addGatewayCompleted = new ModalPresentationCompletion(GatewayPromptResultHandler);
            NavigationService.PushAccessoryView("AddOrEditGatewayView", args, addGatewayCompleted);
        }

        private void LaunchAddUserView()
        {
            AddUserViewArgs args = new AddUserViewArgs(new CredentialsModel(), false);
            ModalPresentationCompletion addUserCompleted = new ModalPresentationCompletion(CredentialPromptResultHandler);
            NavigationService.PushAccessoryView("AddUserView", args, addUserCompleted);
        }

        private void CredentialPromptResultHandler(object sender, PresentationCompletionEventArgs args)
        {
            CredentialPromptResult result = args.Result as CredentialPromptResult;

            if (result != null && !result.UserCancelled)
            {
                Guid credId = this.ApplicationDataModel.Credentials.AddNewModel(result.Credentials);
                LoadUsers();
                this.SelectUserId(credId);
            }
            else
            {
                this.Update();
            }
        }

        private void LoadUsers()
        {
            // load users list
            this.UserOptions.Clear();
            this.UserOptions.Add(new UserComboBoxElement(UserComboBoxType.AskEveryTime));
            this.UserOptions.Add(new UserComboBoxElement(UserComboBoxType.AddNew));

            foreach (IModelContainer<CredentialsModel> credentials in this.ApplicationDataModel.Credentials.Models)
            {
                this.UserOptions.Add(new UserComboBoxElement(UserComboBoxType.Credentials, credentials));
            }
        }

        private void LoadGateways()
        {
            // load gateways list
            this.GatewayOptions.Clear();
            this.GatewayOptions.Add(new GatewayComboBoxElement(GatewayComboBoxType.None));
            this.GatewayOptions.Add(new GatewayComboBoxElement(GatewayComboBoxType.AddNew));

            foreach (IModelContainer<GatewayModel> gateway in this.ApplicationDataModel.Gateways.Models)
            {
                this.GatewayOptions.Add(new GatewayComboBoxElement(GatewayComboBoxType.Gateway, gateway));
            }
        }

        /// <summary>
        /// change user selection without saving to the desktop instance.
        /// </summary>
        /// <param name="userId"> user id for the selected user </param>
        private void SelectUserId(Guid userId)
        {
            int idx = 0;
            if (Guid.Empty != userId)
            {
                for (idx = 0; idx < this.UserOptions.Count; idx++)
                {
                    if (this.UserOptions[idx].UserComboBoxType == UserComboBoxType.Credentials &&
                        this.UserOptions[idx].Credentials.Id == userId)
                        break;
                }

                if (idx == this.UserOptions.Count)
                {
                    idx = 0;
                }
            }

            this.SelectedUserOptionsIndex = idx;
        }

        /// <summary>
        /// change gateway selection without saving to the desktop instance.
        /// </summary>
        /// <param name="gatewayId"> gateway id for the selected gateway </param>
        private void SelectGatewayId(Guid gatewayId)
        {
            int idx = 0;
            if (Guid.Empty != gatewayId)
            {
                for (idx = 0; idx < this.GatewayOptions.Count; idx++)
                {
                    if (this.GatewayOptions[idx].GatewayComboBoxType == GatewayComboBoxType.Gateway &&
                        this.GatewayOptions[idx].Gateway.Id == gatewayId)
                        break;
                }

                if (idx == this.GatewayOptions.Count)
                {
                    idx = 0;
                }
            }

            this.SelectedGatewayOptionsIndex = idx;
        }

    }
}
