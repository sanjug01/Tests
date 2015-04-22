namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Windows.Input;
    using RdClient.Shared.Navigation;

    public class ConnectionCenterViewModel : DeferringViewModelBase,
        IConnectionCenterViewModel,
        IApplicationBarItemsSource,
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
        //
        // Mutable collection of toolbar item models. When the view model needs to modify contents of the toolbar,
        // it modifies this collection.
        //
        private readonly ObservableCollection<BarItemModel> _toolbarItemsSource;
        //
        // Read-only wrapper of the collection of toolbar item models returned by the ToolbarItems property
        // of the IConnectionCenterViewModel interface. The property is used in XAML bindings.
        //
        private readonly ReadOnlyObservableCollection<BarItemModel> _toolbarItems;

        private int _selectedCount;
        private bool _desktopsSelectable;
        private bool _showDesktops;
        private bool _showApps;
        private bool _hasDesktops;
        private bool _hasApps;
        private bool _showSectionLabels;
        //
        // Completion of accessory views passed to all accessory views as the activation parameter.
        // The views subscribe to the completion object and dismiss themselves when completion is requested
        // by the view model. The view model requests completion when it needs to dismiss the current accessory view.
        //
        private SynchronousCompletion _accessoryViewCompletion;
        private readonly IViewVisibility _accessoryViewVisibility;
        private RelayCommand _cancelAccessoryView;

        //
        // App bar items
        //
        private readonly BarItemModel _editItem;
        private readonly BarItemModel _deleteItem;
        private const string EditItemStringId = "Common_Edit_String";
        private const string DeleteItemStringId = "Common_Delete_String";
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
                    comparison = CompareStrings(dmX.HostName, dmY.HostName);

                    if (0 == comparison)
                        comparison = CompareStrings(dmX.FriendlyName, dmY.FriendlyName);
                }

                return comparison;
            }

            private static int CompareStrings(string x, string y)
            {
                int result = 0;

                if(null != x && null != y)
                {
                    result = string.CompareOrdinal(x, y);
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
            this.EditDesktopCommand = new RelayCommand(o => this.EditDesktopCommandExecute(o), o => (1 == this.SelectedCount) );
            this.DeleteDesktopCommand = new RelayCommand(o => this.DeleteDesktopCommandExecute(o), o => (this.SelectedCount >= 1) );
            this.AddWorkspaceCommand = new RelayCommand(o => AddWorkspaceExecute());

            _editItem = new SegoeGlyphBarButtonModel(SegoeGlyph.Edit, EditDesktopCommand, EditItemStringId, BarItemModel.ItemAlignment.Right);
            _deleteItem = new SegoeGlyphBarButtonModel(SegoeGlyph.Trash, DeleteDesktopCommand, DeleteItemStringId, BarItemModel.ItemAlignment.Right);

            _toolbarItemsSource = new ObservableCollection<BarItemModel>();
            _toolbarItems = new ReadOnlyObservableCollection<BarItemModel>(_toolbarItemsSource);
            //
            // Add toolbar buttons
            //
            _toolbarItemsSource.Add(new SegoeGlyphBarButtonModel(SegoeGlyph.Add, new RelayCommand(this.AddResource), "Add"));
            _toolbarItemsSource.Add(new SegoeGlyphBarButtonModel(SegoeGlyph.MultiSelection, new RelayCommand(this.ToggleDesktopSelectionCommandExecute), "Select"));
            _toolbarItemsSource.Add(new SegoeGlyphBarButtonModel(SegoeGlyph.Settings, new RelayCommand(this.GoToSettingsCommandExecute), "Settings"));
            _toolbarItemsSource.Add(new SegoeGlyphBarButtonModel(SegoeGlyph.Help, new RelayCommand(this.PushAboutDialog), "About"));
            //
            //_toolbarItemsSource.Add(new SeparatorBarItemModel());
            //
            _accessoryViewVisibility = ViewVisibility.Create(false);
            _cancelAccessoryView = new RelayCommand(this.ExecuteCancelAccessoryView);

            this.SelectedCount = 0;
        }

        public ReadOnlyObservableCollection<IDesktopViewModel> DesktopViewModels
        {
            get { return _desktopViewModels; }
            private set { SetProperty(ref _desktopViewModels, value); }
        }

        public ReadOnlyObservableCollection<IWorkspaceViewModel> WorkspaceViewModels
        {
            get { return _workspaceViewModels; }
            private set { SetProperty(ref _workspaceViewModels, value); }
        }

        public ReadOnlyObservableCollection<BarItemModel> ToolbarItems
        {
            get { return _toolbarItems; }
        }

        public RelayCommand AddDesktopCommand { get; private set; }
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
                    SetShowSectionLabels();
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
                    SetShowSectionLabels();
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

        private void SetShowSectionLabels()
        {
            this.ShowSectionLabels = this.HasDesktops && this.HasApps;
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

        IEnumerable<BarItemModel> IApplicationBarItemsSource.GetItems(IApplicationBarSite applicationBarSite)
        {
            return new BarItemModel[]
            {               
                _editItem,
                _deleteItem
            };
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
            NavigationService.PushModalView("AddOrEditDesktopView", new AddDesktopViewModelArgs());
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
                    NavigationService.PushModalView("AddOrEditDesktopView", 
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
                this.NavigationService.PushModalView("DeleteDesktopsView",
                    new DeleteDesktopsArgs(selectedDesktops));
            }
        }

        private void AddResource(object parameter)
        {
            //
            // Called by the command bound to the "add" toolbar button
            //
            _accessoryViewCompletion = this.NavigationService.CreateAccessoryStack("SelectNewResourceTypeView", null);
        }

        private void ToggleDesktopSelectionCommandExecute(object o)
        {

            this.DesktopsSelectable = !this.DesktopsSelectable;
        }

        private void GoToSettingsCommandExecute(object o)
        {
            this.NavigationService.NavigateToView("SettingsView", null);
        }

        private void PushAboutDialog(object parameter)
        {
            this.NavigationService.PushAccessoryView("AboutView", _accessoryViewCompletion);
        }

        private void AddWorkspaceExecute()
        {
            NavigationService.PushModalView("AddOrEditWorkspaceView", new AddWorkspaceViewModelArgs());
        }

        private IWorkspaceViewModel CreateWorkspaceViewModel(IModelContainer<OnPremiseWorkspaceModel> workspace)
        {
            return new WorkspaceViewModel(workspace, this.ApplicationDataModel, this, this.NavigationService, _sessionFactory);
        }

        private void ExecuteCancelAccessoryView(object parameter)
        {
            //
            // Cancel the current accessory view (there can be only one).
            //
            Contract.Assert(parameter is IHandleable);

            _accessoryViewCompletion.Complete();
            _accessoryViewCompletion.Reset();
        }
    }
}

