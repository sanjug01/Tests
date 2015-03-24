namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using System.Windows.Input;
    using System.ComponentModel;

    public class AddOrEditWorkspaceViewModel : DeferringViewModelBase
    {
        private readonly RelayCommand _saveCommand;
        private readonly RelayCommand _cancelCommand;
        private string _feedUrl;

        public AddOrEditWorkspaceViewModel()
        {
            _saveCommand = new RelayCommand(SaveCommandExecute);
            _cancelCommand = new RelayCommand(CancelCommandExecute);
        }

        public IPresentableView PresentableView { private get; set; }

        public ICommand SaveCommand { get { return _saveCommand; } }

        public ICommand CancelCommand { get { return _cancelCommand; } }

        public string FeedUrl 
        { 
            get 
            {
                return _feedUrl;
            }
            set
            {
                this.TryDeferToUI(() => SetProperty(ref _feedUrl, value));
            }
        }

        private void SaveCommandExecute(object o)
        {
            RadcClient radcClient = new RadcClient(new RadcEventSource(), new Helpers.TaskExecutor());
            OnPremiseWorkspaceModel workspace = new OnPremiseWorkspaceModel(radcClient, this.ApplicationDataModel);
            workspace.FeedUrl = this.FeedUrl;
            CredentialsModel creds = new CredentialsModel() { Username = @"rdvteam\tstestuser1", Password = @"1234AbCd" };
            workspace.CredentialsId = this.ApplicationDataModel.LocalWorkspace.Credentials.AddNewModel(creds);
            workspace.PropertyChanged += workspace_PropertyChanged;
            workspace.Subscribe();
        }

        void workspace_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            OnPremiseWorkspaceModel workspace = sender as OnPremiseWorkspaceModel;
            if (workspace != null && args.PropertyName.Equals("State"))
            {
                this.FeedUrl = workspace.State.ToString() + "...";
                if (workspace.State == WorkspaceState.Ok)
                {
                    this.ApplicationDataModel.OnPremWorkspaces.Add(workspace);
                    NavigationService.DismissModalView(PresentableView);
                    (sender as OnPremiseWorkspaceModel).PropertyChanged -= workspace_PropertyChanged;
                }
                else if (workspace.State == WorkspaceState.Error)
                {
                    this.FeedUrl = "Error: " + workspace.Error.ToString();
                    workspace.PropertyChanged -= workspace_PropertyChanged;
                    workspace.UnSubscribe();
                }
            }
        }

        private void CancelCommandExecute(object o)
        {
            NavigationService.DismissModalView(PresentableView);
        }
    }
}
