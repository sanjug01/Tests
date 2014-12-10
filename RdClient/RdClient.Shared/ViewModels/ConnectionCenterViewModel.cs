using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.Navigation.Extensions;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public class ConnectionCenterViewModel : ViewModelBase, IConnectionCenterViewModel, IApplicationBarItemsSource
    {
        private readonly RelayCommand _addDesktopCommand;

        private ObservableCollection<IDesktopViewModel> _desktopViewModels;
        private int _selectedCount;

        // app bar items
        private readonly BarItemModel _separatorItem, _rightSeparatorItem;
        private readonly BarItemModel _addItem;
        private readonly BarItemModel _editItem;
        private readonly BarItemModel _deleteItem;
        private readonly BarItemModel _testsItem;

        public ConnectionCenterViewModel()
        {
            _addDesktopCommand = new RelayCommand(AddDesktopExecute);
            EditDesktopCommand = new RelayCommand(o => this.EditDesktopCommandExecute(o), o => (1 == this.SelectedCount) );
            DeleteDesktopCommand = new RelayCommand(o => this.DeleteDesktopCommandExecute(o), o => (this.SelectedCount >= 1) );
            TestsCommand = new RelayCommand(new Action<object>(GotoTests));

            _separatorItem = new SeparatorBarItemModel();

            _addItem = new SegoeGlyphBarButtonModel(SegoeGlyph.Add, AddDesktopCommand, "Add");
            _editItem = new SegoeGlyphBarButtonModel(SegoeGlyph.Edit, EditDesktopCommand, "Edit");
            _deleteItem = new SegoeGlyphBarButtonModel(SegoeGlyph.Trash, DeleteDesktopCommand, "Delete");

            _rightSeparatorItem = new SeparatorBarItemModel(BarItemModel.ItemAlignment.Right);
            _testsItem = new SegoeGlyphBarButtonModel(SegoeGlyph.People, TestsCommand, "Tests",
                BarItemModel.ItemAlignment.Right);

            this.DesktopViewModels = null;
            this.PropertyChanged += ConnectionCenterViewModel_PropertyChanged;
            _selectedCount = 0;
        }

        IEnumerable<BarItemModel> IApplicationBarItemsSource.GetItems(IApplicationBarSite applicationBarSite)
        {
            return new BarItemModel[]
            {               
                _editItem,
                _deleteItem,
                _rightSeparatorItem,
                _testsItem,
            };
        }

        public ObservableCollection<IDesktopViewModel> DesktopViewModels
        {
            get { return _desktopViewModels; }
            private set 
            {                
                SetProperty(ref _desktopViewModels, value);
            }
        }

        public ICommand AddDesktopCommand
        {
            get { return _addDesktopCommand; }
        }

        public RelayCommand EditDesktopCommand { get; private set; }
        public RelayCommand DeleteDesktopCommand { get; private set; }
        public RelayCommand TestsCommand { get; private set; }

        public bool HasDesktops
        {
            get { return this.DataModel.LocalWorkspace.Connections.Count > 0; }
        }

        public int SelectedCount
        {
            get { return _selectedCount; }
        }

        protected override void OnPresenting(object activationParameter)
        {
            // update NavigationService for all DesktopViewModels
            foreach (DesktopViewModel vm in this.DesktopViewModels)
            {
                vm.NavigationService = this.NavigationService;
            }
        }

        private void AddDesktopExecute(object o)
        {
            NavigationService.PushModalView("AddOrEditDesktopView", new AddDesktopViewModelArgs());
        }

        private void ConnectionCenterViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataModel")
            {
                ObservableCollection<IDesktopViewModel> desktopVMs = new ObservableCollection<IDesktopViewModel>();

                foreach (RemoteResource rr in this.DataModel.LocalWorkspace.Connections)
                {
                    rr.CastAndCall<Desktop>(desktop =>
                    {
                        DesktopViewModel vm = new DesktopViewModel(desktop, NavigationService, DataModel);
                        desktopVMs.Add(vm);
                        vm.PropertyChanged += DesktopSelection_PropertyChanged;
                    });
                }

                this.DesktopViewModels = desktopVMs;
                this.DataModel.LocalWorkspace.Connections.CollectionChanged += Desktops_CollectionChanged;
                this.EmitPropertyChanged("HasDesktops");
                ((INotifyPropertyChanged)this.DesktopViewModels).PropertyChanged += DesktopViewModels_PropertyChanged;
            }
        }

        private void DesktopViewModels_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Count"))
            {
                this.EmitPropertyChanged("HasDesktops");
                this.UpdateSelection();
            }
        }

        private void UpdateSelection()
        {
            _selectedCount = 0;
            foreach (DesktopViewModel vm in this.DesktopViewModels)
            {
                if (vm.IsSelected)
                {
                    _selectedCount++;
                }
            }
            this.EmitPropertyChanged("SelectedCount");
            EditDesktopCommand.EmitCanExecuteChanged();
            DeleteDesktopCommand.EmitCanExecuteChanged();
        }

        private void DesktopSelection_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsSelected"))
            {
                this.UpdateSelection();
            }
        }

        private void Desktops_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Desktop desktop in e.NewItems)
                {
                    DesktopViewModel vm = new DesktopViewModel(desktop, NavigationService, DataModel);
                    vm.PropertyChanged += DesktopSelection_PropertyChanged;
                    this.DesktopViewModels.Add(vm);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var vmsToRemove = this.DesktopViewModels.Where(vm => e.OldItems.Contains(vm.Desktop)).ToList();
                foreach (DesktopViewModel vm in vmsToRemove)
                {
                    vm.PropertyChanged -= DesktopSelection_PropertyChanged;
                    this.DesktopViewModels.Remove(vm);
                }
            }
        }

        private void EditDesktopCommandExecute(object o)
        {
            // extract first selected desktops - should be a single one
            foreach (DesktopViewModel vm in this.DesktopViewModels)
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
            List<object> selectedDesktops = new List<object>();

            foreach (DesktopViewModel vm in this.DesktopViewModels)
            {
                if (vm.IsSelected)
                {
                    selectedDesktops.Add(vm.Desktop);
                }
            }

            if (selectedDesktops.Count > 0)
            {
                this.NavigationService.PushModalView("DeleteDesktopsView",
                    new DeleteDesktopsArgs(selectedDesktops));
            }
        }

        private void GotoTests(object o)
        {
            // using valid test data
            NavigationService.NavigateToView("TestsView", 
                new TestsViewModelArgs(
                    new Desktop(this.DataModel.LocalWorkspace) { HostName = "a3-w81" },
                    new Credentials() { Username = "tslabadmin", Domain = "", Password = "1234AbCd", HaveBeenPersisted = false }
                    ));
        }
    }
}

