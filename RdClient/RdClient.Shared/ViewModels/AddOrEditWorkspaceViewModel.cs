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
    using System.Diagnostics.Contracts;
    using System;
    using RdClient.Shared.ValidationRules;

    public class FeedUrlValidationRule : IValidationRule
    {
        private OnPremiseWorkspaceModel _workspace;
        private IModelCollection<OnPremiseWorkspaceModel> _workspaceCollection;

        public FeedUrlValidationRule(OnPremiseWorkspaceModel workspace, IModelCollection<OnPremiseWorkspaceModel> workspaceCollection)
        {
            _workspace = workspace;
            _workspaceCollection = workspaceCollection;
        }

        public bool Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string feedUrl = value as string ?? "";
            bool alreadyAWorkspace = _workspaceCollection.Models.Any(w => w.Model != _workspace && string.Compare(w.Model.FeedUrl, feedUrl, StringComparison.OrdinalIgnoreCase) == 0);
            return !alreadyAWorkspace && Uri.IsWellFormedUriString(feedUrl, UriKind.Absolute);
        }
    }

    public sealed class AddWorkspaceViewModelArgs
    {
        public AddWorkspaceViewModelArgs() {}
    }

    public sealed class EditWorkspaceViewModelArgs
    {
        private readonly OnPremiseWorkspaceModel _workspace;

        public OnPremiseWorkspaceModel Workspace { get { return _workspace; } }

        public EditWorkspaceViewModelArgs(OnPremiseWorkspaceModel workspace)
        {
            _workspace = workspace;
        }
    }

    public sealed class AddOrEditWorkspaceViewModel : DeferringViewModelBase
    {
        private RelayCommand _saveCommand;
        private RelayCommand _cancelCommand;
        private string _feedUrl;
        private ObservableCollection<UserComboBoxElement> _credOptions;
        private UserComboBoxElement _selectedCredOption;
        private OnPremiseWorkspaceModel _workspace;
        private bool _adding;
        private FeedUrlValidationRule _feedValidationRule;

        public AddOrEditWorkspaceViewModel()
        {
            _credOptions = new ObservableCollection<UserComboBoxElement>();
            _selectedCredOption = null;
            _saveCommand = new RelayCommand(SaveCommandExecute);
            _cancelCommand = new RelayCommand(CancelCommandExecute);
        }

        public ICommand SaveCommand { get { return _saveCommand; } }

        public ICommand CancelCommand { get { return _cancelCommand; } }

        public string FeedUrl 
        { 
            get { return _feedUrl; }
            set 
            {
                this.TryDeferToUI(() =>
                {
                    if (SetProperty(ref _feedUrl, value))
                    {
                        _saveCommand.EmitCanExecuteChanged();
                    }
                }); 
            }
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
                    else
                    {
                        _saveCommand.EmitCanExecuteChanged();
                    }
                }
            }
        }

        public OnPremiseWorkspaceModel Workspace
        {
            get { return _workspace; }
            set { SetProperty(ref _workspace, value); }
        }

        public bool Adding
        {
            get { return _adding; }
            set { SetProperty(ref _adding, value); }
        }

        protected override void OnPresenting(object activationParameter)
        {
            AddWorkspaceViewModelArgs addArgs = activationParameter as AddWorkspaceViewModelArgs;
            EditWorkspaceViewModelArgs editArgs = activationParameter as EditWorkspaceViewModelArgs;
            LoadComboBoxItems();
            if (editArgs != null)
            {
                this.Workspace = editArgs.Workspace;
                this.FeedUrl = this.Workspace.FeedUrl;
                this.SelectedCredentialOption = GetComboBoxItem(this.Workspace.Credential);
                this.Adding = false;
            }
            else if (addArgs != null)
            {
                this.Workspace = new OnPremiseWorkspaceModel();
                this.Workspace.Initialize(new RadcClient(new RadcEventSource(), new Helpers.TaskExecutor()), this.ApplicationDataModel);
                this.Adding = true;
            }
            else
            {
                throw new ArgumentException("provide an AddWorkspaceViewModelArgs or EditWorkspaceViewModelArgs");
            }
            _feedValidationRule = new FeedUrlValidationRule(this.Workspace, this.ApplicationDataModel.OnPremWorkspaces);
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
                this.ApplicationDataModel.Credentials.AddNewModel(result.Credentials);
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
            return this.CredentialOptions.FirstOrDefault(cbe =>
            {
                return cbe.UserComboBoxType == UserComboBoxType.Credentials && cbe.Credentials?.Model == cred;
            });          
        }

        private void LoadComboBoxItems()
        {
            List<UserComboBoxElement> comboBoxes = new List<UserComboBoxElement>();
            this.CredentialOptions.Add(new UserComboBoxElement(UserComboBoxType.AddNew));
            foreach (IModelContainer<CredentialsModel> cred in this.ApplicationDataModel.Credentials.Models)
            {
                this.CredentialOptions.Add(new UserComboBoxElement(UserComboBoxType.Credentials, cred));
            }
            this.SelectedCredentialOption = null;
        }

        private void SaveCommandExecute(object o)
        {
            if (this.SelectedCredentialOption != null
                && this.SelectedCredentialOption.Credentials != null
                && _feedValidationRule.Validate(this.FeedUrl, null))
            {
                OnPremiseWorkspaceModel workspace = this.Workspace;
                if (this.Adding)
                {
                    this.ApplicationDataModel.OnPremWorkspaces.AddNewModel(workspace);
                }
                workspace.FeedUrl = this.FeedUrl;
                workspace.CredentialsId = this.SelectedCredentialOption.Credentials.Id;
                workspace.UnSubscribe(result =>
                {
                    workspace.Subscribe();
                });
                this.DismissModal(null);
            }
        }
        private void CancelCommandExecute(object o)
        {
            this.DismissModal(null);
        }
    }
}
