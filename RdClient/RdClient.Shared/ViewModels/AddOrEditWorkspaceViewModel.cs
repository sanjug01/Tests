namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using System.Windows.Input;

    public class AddOrEditWorkspaceViewModel : ViewModelBase
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
                SetProperty(ref _feedUrl, value);
            }
        }

        private void SaveCommandExecute(object o)
        {
            RadcClient radcClient = new RadcClient(new RadcEventSource(), new Helpers.TaskExecutor());
            OnPremiseWorkspaceModel workspace = new OnPremiseWorkspaceModel(radcClient, this.ApplicationDataModel);
            workspace.FeedUrl = this.FeedUrl;
            CredentialsModel creds = new CredentialsModel() { Username = @"rdvteam\tstestuser1", Password = @"1234AbCd" };
            workspace.CredentialsId = this.ApplicationDataModel.LocalWorkspace.Credentials.AddNewModel(creds);
            workspace.PropertyChanged += (sender, args) =>
                {                    
                    if (args.PropertyName.Equals("State"))
                    {
                        if (workspace.State == WorkspaceState.AddingResources)
                        {
                            this.ApplicationDataModel.OnPremWorkspaces.Add(workspace);
                            NavigationService.DismissModalView(PresentableView);
                        }
                        else if (workspace.State == WorkspaceState.Error)
                        {
                            radcClient.StartRemoveFeed(workspace.FeedUrl);
                        }
                    }
                };
            workspace.Subscribe();
        }

        private void CancelCommandExecute(object o)
        {
            NavigationService.DismissModalView(PresentableView);
        }
    }
}
