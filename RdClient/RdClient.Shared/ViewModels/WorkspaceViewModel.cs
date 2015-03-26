﻿namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Input;

    public class WorkspaceViewModel : Helpers.MutableObject, IWorkspaceViewModel
    {
        private string _name;
        private readonly RelayCommand _editCommand;
        private readonly RelayCommand _deleteCommand;
        private readonly RelayCommand _refreshCommand;
        private readonly IModelContainer<OnPremiseWorkspaceModel> _workspace;
        private readonly ReadOnlyObservableCollection<IRemoteResourceViewModel> _remoteResourceViewModels;
        private readonly IExecutionDeferrer _dispatcher;
        private readonly INavigationService _navigationService;
        private readonly ISessionFactory _sessionFactory;
        private readonly ApplicationDataModel _dataModel;

        public WorkspaceViewModel(
            IModelContainer<OnPremiseWorkspaceModel> workspace,
            ApplicationDataModel dataModel,
            IExecutionDeferrer dispatcher,
            INavigationService navigationService,
            ISessionFactory sessionFactory)
        {
            _editCommand = new RelayCommand((o) => EditCommandExecute());
            _refreshCommand = new RelayCommand((o) => RefreshCommandExecute());
            _deleteCommand = new RelayCommand((o) => DeleteCommandExecute());
            _dispatcher = dispatcher;
            _navigationService = navigationService;
            _workspace = workspace;
            _workspace.Model.PropertyChanged += WorkspacePropertyChanged;
            _remoteResourceViewModels = TransformingObservableCollection<RemoteResourceModel, IRemoteResourceViewModel>.Create(_workspace.Model.Resources, CreateRemoteResourceViewModel);
            _dataModel = dataModel;
            _sessionFactory = sessionFactory;
            SetName();
        }

        public string Name
        {
            get { return _name; }
            private set { _dispatcher.TryDeferToUI(() => SetProperty(ref _name, value)); }
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

        public ReadOnlyObservableCollection<IRemoteResourceViewModel> RemoteResourceViewModels
        {
            get { return _remoteResourceViewModels; }
        }

        private void DeleteCommandExecute()
        {
            _workspace.Model.UnSubscribe();
        }

        private void RefreshCommandExecute()
        {
            _workspace.Model.Refresh();
        }

        private void EditCommandExecute()
        {
            _navigationService.PushModalView("AddOrEditWorkspaceView", null);
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
            }
        }

        private void SetName()
        {
            this.Name = _workspace.Model.FriendlyName ?? _workspace.Model.FeedUrl ?? "";
        }

        private IRemoteResourceViewModel CreateRemoteResourceViewModel(RemoteResourceModel remoteResource)
        {
            return new RemoteResourceViewModel(remoteResource, _dispatcher, _navigationService, _sessionFactory);
        }
    }
}
