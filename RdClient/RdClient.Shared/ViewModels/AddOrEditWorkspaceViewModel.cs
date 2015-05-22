namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ValidationRules;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;

    public class FeedUrlValidationRule : IValidationRule<string>
    {
        private OnPremiseWorkspaceModel _workspace;
        private IModelCollection<OnPremiseWorkspaceModel> _workspaceCollection;

        public FeedUrlValidationRule(OnPremiseWorkspaceModel workspace, IModelCollection<OnPremiseWorkspaceModel> workspaceCollection)
        {
            _workspace = workspace;
            _workspaceCollection = workspaceCollection;
        }

        public IValidationResult Validate(string value)
        {
            string feedUrl = value ?? "";            
            if (string.IsNullOrEmpty(feedUrl))
            {
                return ValidationResult.Empty();
            }
            else
            {
                bool alreadyAWorkspace = _workspaceCollection.Models.Any(w => w.Model != _workspace && string.Compare(w.Model.FeedUrl, feedUrl, StringComparison.OrdinalIgnoreCase) == 0);
                if (!alreadyAWorkspace && Uri.IsWellFormedUriString(feedUrl, UriKind.Absolute))
                {
                    return ValidationResult.Valid();
                }
                else
                {
                    return ValidationResult.Invalid();
                }
            }                        
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

    public sealed class AddOrEditWorkspaceViewModel : DeferringViewModelBase, IDialogViewModel, IUsersCollector
    {
        private readonly RelayCommand _saveCommand;
        private readonly RelayCommand _cancelCommand;
        private readonly RelayCommand _addUserCommand;
        private string _feedUrl;
        private ReadOnlyObservableCollection<UserComboBoxElement> _users;
        private UserComboBoxElement _selectedUser;
        private OnPremiseWorkspaceModel _workspace;
        private bool _adding;
        private FeedUrlValidationRule _feedValidationRule;

        public AddOrEditWorkspaceViewModel()
        {
            _selectedUser = null;
            _saveCommand = new RelayCommand(SaveCommandExecute);
            _cancelCommand = new RelayCommand(CancelCommandExecute);
            _addUserCommand = new RelayCommand(LaunchAddUserView);
        }

        public ICommand DefaultAction { get { return _saveCommand; } }

        public ICommand Cancel { get { return _cancelCommand; } }

        public ICommand AddUser { get { return _addUserCommand; } }
        
        // edit not supported, but may be in the future
        public ICommand EditUser { get { throw new NotImplementedException(); } }

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

        public ReadOnlyObservableCollection<UserComboBoxElement> Users
        {
            get { return _users; }
            private set { SetProperty(ref _users, value); }
        }

        public UserComboBoxElement SelectedUser
        {
            get { return _selectedUser; }
            set { SetProperty(ref _selectedUser, value); }
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

            // initialize users colection
            this.Users = TransformingObservableCollection<IModelContainer<CredentialsModel>, UserComboBoxElement>.Create(
                this.ApplicationDataModel.Credentials.Models,
                c => new UserComboBoxElement(UserComboBoxType.Credentials, c),
                ucbe =>
                {
                    if (this.SelectedUser == ucbe)
                    {
                        this.SelectedUser = null;
                    }
                });

            if (editArgs != null)
            {
                this.Workspace = editArgs.Workspace;
                this.FeedUrl = this.Workspace.FeedUrl;
                this.SelectedUser = GetComboBoxItem(this.Workspace.Credential);
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

        private void LaunchAddUserView(object o)
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
                this.SelectedUser = GetComboBoxItem(result.Credentials);
            }
            else
            {
                this.SelectedUser = null;
            }
        }

        private UserComboBoxElement GetComboBoxItem(CredentialsModel cred)
        {
            return this.Users.FirstOrDefault(cbe =>
            {
                return cbe.UserComboBoxType == UserComboBoxType.Credentials && cbe.Credentials?.Model == cred;
            });          
        }

        private void SaveCommandExecute(object o)
        {
            if (this.SelectedUser != null
                && this.SelectedUser.Credentials != null
                && _feedValidationRule.Validate(this.FeedUrl).Status == ValidationResultStatus.Valid)
            {
                OnPremiseWorkspaceModel workspace = this.Workspace;
                if (this.Adding)
                {
                    this.ApplicationDataModel.OnPremWorkspaces.AddNewModel(workspace);
                }
                workspace.FeedUrl = this.FeedUrl;
                workspace.CredentialsId = this.SelectedUser.Credentials.Id;
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
