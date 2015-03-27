namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Input;
    using System.Linq;

    public class WorkspaceViewModel : Helpers.MutableObject, IWorkspaceViewModel
    {
        private string _name;
        private List<IRemoteResourceViewModel> _remoteResourceViewModels;
        private readonly RelayCommand _editCommand;
        private readonly RelayCommand _deleteCommand;
        private readonly RelayCommand _refreshCommand;
        private readonly IModelContainer<OnPremiseWorkspaceModel> _workspace;        
        private readonly IExecutionDeferrer _dispatcher;
        private readonly INavigationService _navigationService;
        private readonly ISessionFactory _sessionFactory;
        private readonly ApplicationDataModel _dataModel;
        private string _stateString;
        private string _errorString;

        public WorkspaceViewModel(
            IModelContainer<OnPremiseWorkspaceModel> workspace,
            ApplicationDataModel dataModel,
            IExecutionDeferrer dispatcher,
            INavigationService navigationService,
            ISessionFactory sessionFactory)
        {
            base.ExecutionDeferrer = dispatcher;
            _editCommand = new RelayCommand((o) => EditCommandExecute());
            _refreshCommand = new RelayCommand((o) => RefreshCommandExecute());
            _deleteCommand = new RelayCommand((o) => DeleteCommandExecute());
            _dispatcher = dispatcher;
            _navigationService = navigationService;
            _workspace = workspace;
            _workspace.Model.PropertyChanged += WorkspacePropertyChanged;
            _remoteResourceViewModels = null;
            _dataModel = dataModel;
            _sessionFactory = sessionFactory;
            SetName();
            SetState();
            SetError();
            LoadResources();
        }

        public string Name
        {
            get { return _name; }
            private set { _dispatcher.TryDeferToUI(() => SetProperty(ref _name, value)); }
        }
        public string State
        {
            get { return _stateString; }
            private set { _dispatcher.TryDeferToUI(() => SetProperty(ref _stateString, value)); }
        }

        public string Error
        {
            get { return _errorString; }
            private set { _dispatcher.TryDeferToUI(() => SetProperty(ref _errorString, value)); }
        }

        public List<IRemoteResourceViewModel> RemoteResourceViewModels
        {
            get { return _remoteResourceViewModels; }
            private set { _dispatcher.TryDeferToUI(() => SetProperty(ref _remoteResourceViewModels, value)); }
        }


        public ICommand DeleteCommand
        {
            get { return _deleteCommand; }
        }

        public ICommand EditCommand
        {
            get { return _editCommand; }
        }

        public ICommand RefreshCommand
        {
            get { return _refreshCommand; }
        }

        private void DeleteCommandExecute()
        {
            _workspace.Model.UnSubscribe();
            _dataModel.OnPremWorkspaces.RemoveModel(_workspace.Id);
        }

        private void RefreshCommandExecute()
        {
            _workspace.Model.Refresh();
        }

        private void EditCommandExecute()
        {
            _navigationService.PushModalView("AddOrEditWorkspaceView", new EditWorkspaceViewModelArgs(_workspace.Model));
        }

        private void WorkspacePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (sender != _workspace.Model)
            {
                throw new InvalidOperationException("Should only subscribe to property changed events for this workspace");
            }
            switch (args.PropertyName)
            {
                case "FriendlyName":
                case "FeedUrl":
                    SetName();
                    break;
                case "Resources":
                    LoadResources();
                    break;
                case "State":
                    SetState();
                    break;
                case "Error":
                    SetError();
                    break;
            }
        }

        private void SetError()
        {
            this.Error = _workspace.Model.Error.ToString();
        }

        private void SetState()
        {
            this.State = _workspace.Model.State.ToString();
        }

        private void LoadResources()
        {
            List<IRemoteResourceViewModel> resourceVMs = new List<IRemoteResourceViewModel>();
            foreach (RemoteResourceModel remoteResourceModel in _workspace.Model.Resources ?? Enumerable.Empty<RemoteResourceModel>())
            {
                resourceVMs.Add(CreateRemoteResourceViewModel(remoteResourceModel));
            }
            this.RemoteResourceViewModels = resourceVMs;
        }

        private void SetName()
        {
            this.Name = _workspace.Model.FriendlyName ?? _workspace.Model.FeedUrl ?? "";
        }

        private IRemoteResourceViewModel CreateRemoteResourceViewModel(RemoteResourceModel remoteResource)
        {
            return new RemoteResourceViewModel(remoteResource, _dataModel, _dispatcher, _navigationService, _sessionFactory);
        }
    }
}
