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
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    public class ConnectionCenterViewModel : DeferringViewModelBase,
        IConnectionCenterViewModel,
        ISessionFactorySite
    {
        //
        // Sorted collection of desktop models; sorting order is defined by the Order property of this object.
        //
        private IOrderedObservableCollection<IModelContainer<RemoteConnectionModel>> _orderedConnections;
        //
        // Desktop view models created for elements of _orderedConnections.Models.
        //
        private ReadOnlyObservableCollection<IDesktopViewModel> _desktopViewModels;
        private ReadOnlyObservableCollection<IWorkspaceViewModel> _workspaceViewModels;

        private readonly ViewItemsSource _desktopViewModelsSource;
        private readonly ICommand _showSettings;
        private readonly ICommand _showAbout;
        private readonly ICommand _showHelp;

        private int _selectedCount;
        private bool _desktopsSelectable;
        private bool _showDesktops;
        private bool _showApps;
        private bool _hasDesktops;
        private bool _hasApps;
        private bool _showSectionLabels;

        private readonly IViewVisibility _accessoryViewVisibility;
        private RelayCommand _cancelAccessoryView;

        //
        // Session factory object set by the navigation service extension through
        // the ISessionFactorySite interface.
        //
        private ISessionFactory _sessionFactory;
        //
        // Alphabetical comparer of desktop model host names; the class is used to automatically
        // sort the observable collection of local desktops.
        //
        private sealed class DesktopModelAlphabeticalComparer : IComparer<IModelContainer<RemoteConnectionModel>>
        {
            int IComparer<IModelContainer<RemoteConnectionModel>>.Compare(IModelContainer<RemoteConnectionModel> x, IModelContainer<RemoteConnectionModel> y)
            {
                DesktopModel dmX = x.Model as DesktopModel;
                DesktopModel dmY = y.Model as DesktopModel;
                int comparison = 0;

                if(null != dmX && null != dmY)
                {
                    string
                        nameX = GetModelName(dmX),
                        nameY = GetModelName(dmY);

                    comparison = CompareStrings(nameX, nameY);

                    if (0 == comparison)
                    {
                        //
                        // If strings are the same, compare the hashes.
                        //
                        int hashX = dmX.GetHashCode(),
                            hashY = dmY.GetHashCode();

                        comparison = hashX < hashY ? -1 : hashX == hashY ? 0 : 1;
                    }
                }

                return comparison;
            }

            private static string GetModelName(DesktopModel model)
            {
                Contract.Requires(null != model);
                return string.IsNullOrEmpty(model.FriendlyName) ? model.HostName : model.FriendlyName;
            }

            private static int CompareStrings(string x, string y)
            {
                int result = 0;

                if(null != x && null != y)
                {
                    result = string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
                }
                else if(null != x)
                {
                    result = 1;
                }
                else if(null != y)
                {
                    result = -1;
                }

                return result;
            }
        }

        public ConnectionCenterViewModel()
        {
            this.AddDesktopCommand = new RelayCommand(AddDesktopExecute);
            _desktopViewModelsSource = new ViewItemsSource();
            _showSettings = new RelayCommand(this.GoToSettingsCommandExecute);
            _showAbout = new RelayCommand(this.ShowAboutCommandExecute);
            _showHelp = new RelayCommand(this.ShowHelpCommandExecute);
            this.EditDesktopCommand = new RelayCommand(o => this.EditDesktopCommandExecute(o), o => (1 == this.SelectedCount) );
            this.DeleteDesktopCommand = new RelayCommand(o => this.DeleteDesktopCommandExecute(o), o => (this.SelectedCount >= 1) );
            this.AddWorkspaceCommand = new RelayCommand(o => AddWorkspaceExecute());

            _accessoryViewVisibility = ViewVisibility.Create(false);
            _cancelAccessoryView = new RelayCommand(o => this.ExecuteCancelAccessoryView());

            this.SelectedCount = 0;
        }

        public ReadOnlyObservableCollection<IDesktopViewModel> DesktopViewModels
        {
            get { return _desktopViewModels; }
            private set { SetProperty(ref _desktopViewModels, value); }
        }

        public IViewItemsSource DesktopViewModelsSource
        {
            get { return _desktopViewModelsSource; }
        }

        public ReadOnlyObservableCollection<IWorkspaceViewModel> WorkspaceViewModels
        {
            get { return _workspaceViewModels; }
            private set { SetProperty(ref _workspaceViewModels, value); }
        }

        public RelayCommand AddDesktopCommand { get; private set; }
        public ICommand ShowSettings { get { return _showSettings; } }
        public ICommand ShowAbout { get { return _showAbout; } }
        public ICommand ShowHelp { get { return _showHelp; } }
        public RelayCommand EditDesktopCommand { get; private set; }
        public RelayCommand DeleteDesktopCommand { get; private set; }
        public RelayCommand AddWorkspaceCommand { get; private set; }

        public bool HasDesktops
        {
            get 
            { 
                return _hasDesktops; 
            }
            private set 
            {
                if (SetProperty(ref _hasDesktops, value))
                {
                    this.ShowDesktops = value;
                    this.ShowApps = !value;
                    UpdateShowSectionLabels();
                }
            }
        }

        public bool HasApps
        {
            get
            {
                return _hasApps;
            }
            private set
            {
                if (SetProperty(ref _hasApps, value))
                {
                    this.ShowApps = value;
                    this.ShowDesktops = !value;
                    UpdateShowSectionLabels();
                }
            }
        }

        public bool ShowDesktops
        {
            get 
            { 
                return _showDesktops; 
            }
            set 
            {
                if ((value == this.HasDesktops) || (!value && this.HasApps))
                {
                    SetProperty(ref _showDesktops, value);
                }
            }
        }

        public bool ShowApps
        {
            get
            {
                return _showApps;
            }
            set
            {
                if ((value == this.HasApps) || (!value && this.HasDesktops))
                {
                    SetProperty(ref _showApps, value);
                }
            }
        }

        public bool ShowSectionLabels
        {
            get
            {
                return _showSectionLabels;
            }
            private set
            {
                SetProperty(ref _showSectionLabels, value);
            }
        }

        public IViewVisibility AccessoryViewVisibility
        {
            get { return _accessoryViewVisibility; }
        }

        public ICommand CancelAccessoryView
        {
            get { return _cancelAccessoryView; }
        }

        public int SelectedCount
        {
            get { return _selectedCount; }
            private set { SetProperty(ref _selectedCount, value); }
        }

        public bool DesktopsSelectable
        {
            get
            {
                return _desktopsSelectable;
            }
            set
            {
                SetProperty(ref _desktopsSelectable, value);

                foreach (DesktopViewModel vm in _desktopViewModels)
                {
                    vm.SelectionEnabled = value;
                }                
            }
        }

        void ISessionFactorySite.SetSessionFactory(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(null != _sessionFactory);

            if (null == _desktopViewModels)
            {
                //
                // 1. Include only containers with DesktopModel model objects.
                //
                ReadOnlyObservableCollection<IModelContainer<RemoteConnectionModel>> onlyDesktops =
                    FilteringObservableCollection<IModelContainer<RemoteConnectionModel>>.Create(
                        this.ApplicationDataModel.LocalWorkspace.Connections.Models,
                        container => container.Model is DesktopModel);
                //
                // 2. Sort the desktop models by name.
                //
                _orderedConnections = OrderedObservableCollection<IModelContainer<RemoteConnectionModel>>.Create(onlyDesktops);
                _orderedConnections.Order = new DesktopModelAlphabeticalComparer();
                //
                // 3. Transform containers with desktop models into desktop view models.
                //
                this.DesktopViewModels = TransformingObservableCollection<IModelContainer<RemoteConnectionModel>, IDesktopViewModel>
                                            .Create(_orderedConnections.Models, this.CreateDesktopViewModel, this.RemovedDesktopViewModel);

                INotifyPropertyChanged npc = _desktopViewModels;
                npc.PropertyChanged += OnDesktopViewModelPropertyChanged;
            }
            else
            {
                //
                // Attach the session factory to all desktop view models
                //
                foreach (IRemoteConnectionViewModel dvm in _desktopViewModels)
                {
                    dvm.Presenting(_sessionFactory);
                }
            }                        
            
            if (null == _workspaceViewModels)
            {
                //
                // 3. Transform containers with desktop models into desktop view models.
                //
                this.WorkspaceViewModels = TransformingObservableCollection<IModelContainer<OnPremiseWorkspaceModel>, IWorkspaceViewModel>
                                            .Create(this.ApplicationDataModel.OnPremWorkspaces.Models, this.CreateWorkspaceViewModel);

                INotifyPropertyChanged npc = _workspaceViewModels;
                npc.PropertyChanged += OnWorkspaceViewModelsPropertyChanged;
            }
            else
            {
            }  

            this.HasDesktops = _desktopViewModels.Count > 0;
            this.HasApps = _workspaceViewModels.Count > 0;
        }

        private void OnWorkspaceViewModelsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Count"))
            {
                this.HasApps = _workspaceViewModels.Count > 0;
            }            
        }

        protected override void OnDismissed()
        {
            foreach(IRemoteConnectionViewModel dvm in _desktopViewModels)
            {
                dvm.Dismissed();
            }

            this.DesktopViewModels = null;
            this.WorkspaceViewModels = null;

            base.OnDismissed();
        }

        private IDesktopViewModel CreateDesktopViewModel(IModelContainer<RemoteConnectionModel> container)
        {
            Contract.Assert(container.Model is DesktopModel, "Data model for a desktop tile is not DesktopModel");
            Contract.Assert(null != _sessionFactory);

            IDesktopViewModel dvm = DesktopViewModel.Create(container, this.ApplicationDataModel, this.NavigationService);

            dvm.SelectionEnabled = this.DesktopsSelectable;
            dvm.PropertyChanged += DesktopSelection_PropertyChanged;
            dvm.Presenting(_sessionFactory);

            return dvm;
        }

        private void RemovedDesktopViewModel(IDesktopViewModel desktopViewModel)
        {
            desktopViewModel.PropertyChanged -= this.DesktopSelection_PropertyChanged;
            desktopViewModel.Dismissed();
        }

        private void AddDesktopExecute(object o)
        {
            AddDesktopViewModelArgs args = new AddDesktopViewModelArgs(_desktopViewModelsSource);

            args.DesktopAdded += (sender, e) =>
            {
                //
                // Find the new view model in the observable collection of desktop view models,
                // that is based on the desktop model reported in DesktopAdded, and tell the view
                // to select the item that shows the new desktop.
                //
                foreach(DesktopViewModel dvm in this.DesktopViewModels)
                {
                    if(object.ReferenceEquals(dvm.Desktop, e.Desktop))
                    {
                        e.ItemsSource.SelectItem(dvm);
                        break;
                    }
                }
            };

            NavigationService.PushAccessoryView("AddOrEditDesktopView", args);
        }

        private void OnDesktopViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Count"))
            {
                this.HasDesktops = _desktopViewModels.Count > 0;
                this.UpdateSelection();
            }
        }

        private void UpdateSelection()
        {
            int newSelectedCount = 0;            
            foreach (DesktopViewModel vm in _desktopViewModels)
            {
                if (vm.IsSelected)
                {
                    newSelectedCount++;
                }
            }
            this.SelectedCount = newSelectedCount;            
            EditDesktopCommand.EmitCanExecuteChanged();
            DeleteDesktopCommand.EmitCanExecuteChanged();
        }

        private void UpdateShowSectionLabels()
        {
            //this.ShowSectionLabels = this.HasDesktops || this.HasApps;
        }

        private void DesktopSelection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsSelected"))
            {
                this.UpdateSelection();
            }
        }

        private void EditDesktopCommandExecute(object o)
        {
            // extract first selected desktops - should be a single one
            foreach (DesktopViewModel vm in _desktopViewModels)
            {
                if (vm.IsSelected)
                {
                    NavigationService.PushAccessoryView("AddOrEditDesktopView", 
                        new EditDesktopViewModelArgs(vm.Desktop));
                    break;
                }
            }
        }

        private void DeleteDesktopCommandExecute(object o)
        {
            // extract list of selected desktops
            IList<IModelContainer<DesktopModel>> selectedDesktops = null;

            foreach (DesktopViewModel vm in _desktopViewModels)
            {
                if (vm.IsSelected)
                {
                    if (null == selectedDesktops)
                        selectedDesktops = new List<IModelContainer<DesktopModel>>();
                    selectedDesktops.Add(TemporaryModelContainer<DesktopModel>.WrapModel(vm.DesktopId, vm.Desktop));
                }
            }

            if (null != selectedDesktops)
            {
                this.NavigationService.PushAccessoryView("DeleteDesktopsView",
                    new DeleteDesktopsArgs(selectedDesktops));
            }
        }

        private void AddResource(object parameter)
        {
            //
            // Called by the command bound to the "add" toolbar button
            //
            this.NavigationService.PushAccessoryView("SelectNewResourceTypeView", null);
        }

        private void ToggleDesktopSelectionCommandExecute(object o)
        {

            this.DesktopsSelectable = !this.DesktopsSelectable;
        }

        private void GoToSettingsCommandExecute(object o)
        {
            this.NavigationService.PushAccessoryView("SettingsView", null);
        }

        private void ShowAboutCommandExecute(object o)
        {
            this.NavigationService.PushAccessoryView("AboutView", null);
        }

        private async void ShowHelpCommandExecute(object o)
        {            
            await Windows.System.Launcher.LaunchUriAsync(new Uri(GlobalConstants.HelpDocumentUrl));
        }

        private void PushAdditionalCommandsDialog(object parameter)
        {
            this.NavigationService.PushAccessoryView("AdditionalToolbarCommandsView", null);
        }

        private void AddWorkspaceExecute()
        {
            NavigationService.PushModalView("AddOrEditWorkspaceView", new AddWorkspaceViewModelArgs());
        }

        private IWorkspaceViewModel CreateWorkspaceViewModel(IModelContainer<OnPremiseWorkspaceModel> workspace)
        {
            return new WorkspaceViewModel(workspace, this.ApplicationDataModel, this, this.NavigationService, _sessionFactory);
        }

        private void ExecuteCancelAccessoryView()
        {
            NavigationService.DismissAccessoryViewsCommand.Execute(null);
        }
    }
}

