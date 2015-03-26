namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using System.Windows.Input;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    using RdClient.Shared.Data;
    using System.Linq;

    public sealed class AddOrEditWorkspaceViewModel : DeferringViewModelBase
    {
        private readonly RelayCommand _saveCommand;
        private readonly RelayCommand _cancelCommand;
        private string _feedUrl;
        private ObservableCollection<UserComboBoxElement> _credOptions;
        private UserComboBoxElement _selectedCredOption;

        public AddOrEditWorkspaceViewModel()
        {
            _saveCommand = new RelayCommand(SaveCommandExecute);
            _cancelCommand = new RelayCommand(CancelCommandExecute);
            _credOptions = new ObservableCollection<UserComboBoxElement>();
            _selectedCredOption = null;
        }

        public IPresentableView PresentableView { private get; set; }

        public ICommand SaveCommand { get { return _saveCommand; } }

        public ICommand CancelCommand { get { return _cancelCommand; } }

        public string FeedUrl 
        { 
            get { return _feedUrl; }
            set { this.TryDeferToUI(() => SetProperty(ref _feedUrl, value)); }
        }

        public ObservableCollection<UserComboBoxElement> CredentialOptions
        {
            get { return _credOptions; }
        }

        public UserComboBoxElement SelectedCredentialOption
        {
            get { return _selectedCredOption; }
            set 
            { 
                if (SetProperty(ref _selectedCredOption, value))
                {
                    if (value != null && value.UserComboBoxType == UserComboBoxType.AddNew)
                    {
                        LaunchAddUserView();
                    }
                }
            }
        }

        protected override void OnPresenting(object activationParameter)
        {
            LoadComboBoxItems();
        }

        private void LaunchAddUserView()
        {
            AddUserViewArgs args = new AddUserViewArgs(new CredentialsModel(), false);
            ModalPresentationCompletion addUserCompleted = new ModalPresentationCompletion(CredentialPromptResultHandler);
            NavigationService.PushModalView("AddUserView", args, addUserCompleted);
        }

        private void CredentialPromptResultHandler(object sender, PresentationCompletionEventArgs args)
        {
            CredentialPromptResult result = args.Result as CredentialPromptResult;

            if (result != null && !result.UserCancelled)
            {
                this.ApplicationDataModel.LocalWorkspace.Credentials.AddNewModel(result.Credentials);
                LoadComboBoxItems();
                this.SelectedCredentialOption = GetComboBoxItem(result.Credentials);
            }
            else
            {
                this.SelectedCredentialOption = null;
            }
        }

        private UserComboBoxElement GetComboBoxItem(CredentialsModel cred)
        {
            return this.CredentialOptions.Single(cbe =>
            {
                return cbe.Credentials != null && cbe.Credentials.Model == cred;
            });
        }

        private void LoadComboBoxItems()
        {
            List<UserComboBoxElement> comboBoxes = new List<UserComboBoxElement>();
            this.CredentialOptions.Add(new UserComboBoxElement(UserComboBoxType.AddNew));
            foreach (IModelContainer<CredentialsModel> cred in this.ApplicationDataModel.LocalWorkspace.Credentials.Models)
            {
                this.CredentialOptions.Add(new UserComboBoxElement(UserComboBoxType.Credentials, cred));
            }
            this.SelectedCredentialOption = null;
        }

        private void SaveCommandExecute(object o)
        {
            RadcClient radcClient = new RadcClient(new RadcEventSource(), new Helpers.TaskExecutor());
            OnPremiseWorkspaceModel workspace = new OnPremiseWorkspaceModel(radcClient, this.ApplicationDataModel);
            workspace.FeedUrl = this.FeedUrl;
            workspace.CredentialsId = this.SelectedCredentialOption.Credentials.Id;
            workspace.PropertyChanged += workspace_PropertyChanged;
            workspace.Subscribe();
        }
        private void CancelCommandExecute(object o)
        {
            NavigationService.DismissModalView(PresentableView);
        }

        private void workspace_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            OnPremiseWorkspaceModel workspace = sender as OnPremiseWorkspaceModel;
            if (workspace != null && args.PropertyName.Equals("State"))
            {
                this.FeedUrl = workspace.State.ToString() + "...";
                if (workspace.State == WorkspaceState.Ok)
                {
                    this.TryDeferToUI(() =>
                    {
                        this.ApplicationDataModel.OnPremWorkspaces.AddNewModel(workspace);
                        NavigationService.DismissModalView(PresentableView);
                        (sender as OnPremiseWorkspaceModel).PropertyChanged -= workspace_PropertyChanged;
                    });
                }
                else if (workspace.State == WorkspaceState.Error)
                {
                    this.TryDeferToUI(() =>
                    {
                        this.FeedUrl = "Error: " + workspace.Error.ToString();
                        workspace.PropertyChanged -= workspace_PropertyChanged;
                        workspace.UnSubscribe();
                    });
                }
            }
        }
    }
}
