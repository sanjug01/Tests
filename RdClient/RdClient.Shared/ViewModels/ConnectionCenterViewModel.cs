﻿using RdClient.Shared.Data;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.Navigation.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;

namespace RdClient.Shared.ViewModels
{
    public class ConnectionCenterViewModel : DeferringViewModelBase, IConnectionCenterViewModel, IApplicationBarItemsSource
    {
        //private ObservableCollection<IDesktopViewModel> _desktopViewModels;
        private IOrderedObservableCollection<IModelContainer<RemoteConnectionModel>> _orderedConnections;
        private ReadOnlyObservableCollection<IDesktopViewModel> _desktopViewModels;
        private int _selectedCount;
        private bool _desktopsSelectable;

        // app bar items
        private readonly BarItemModel _editItem;
        private readonly BarItemModel _deleteItem;
        private const string EditItemStringId = "Common_Edit_String";
        private const string DeleteItemStringId = "Common_Delete_String";

        private class DesktopModelAlphabeticalComparer : IComparer<IModelContainer<RemoteConnectionModel>>
        {
            int IComparer<IModelContainer<RemoteConnectionModel>>.Compare(IModelContainer<RemoteConnectionModel> x, IModelContainer<RemoteConnectionModel> y)
            {
                DesktopModel dmX = x.Model as DesktopModel;
                DesktopModel dmY = y.Model as DesktopModel;
                int comparison = 0;

                if(null != dmX && null != dmY)
                {
                    comparison = string.CompareOrdinal(dmX.HostName, dmY.HostName);
                }

                return comparison;
            }
        }

        public ConnectionCenterViewModel()
        {
            this.AddDesktopCommand = new RelayCommand(AddDesktopExecute);
            this.EditDesktopCommand = new RelayCommand(o => this.EditDesktopCommandExecute(o), o => (1 == this.SelectedCount) );
            this.DeleteDesktopCommand = new RelayCommand(o => this.DeleteDesktopCommandExecute(o), o => (this.SelectedCount >= 1) );
            this.ToggleDesktopSelectionCommand = new RelayCommand(this.ToggleDesktopSelectionCommandExecute);
            this.GoToSettingsCommand = new RelayCommand(this.GoToSettingsCommandExecute);

            _editItem = new SegoeGlyphBarButtonModel(SegoeGlyph.Edit, EditDesktopCommand, EditItemStringId, BarItemModel.ItemAlignment.Right);
            _deleteItem = new SegoeGlyphBarButtonModel(SegoeGlyph.Trash, DeleteDesktopCommand, DeleteItemStringId, BarItemModel.ItemAlignment.Right);

            this.SelectedCount = 0;
        }

        public ReadOnlyObservableCollection<IDesktopViewModel> DesktopViewModels
        {
            get { return _desktopViewModels; }

            private set 
            {                
                SetProperty(ref _desktopViewModels, value);
            }
        }

        public RelayCommand AddDesktopCommand { get; private set; }
        public RelayCommand EditDesktopCommand { get; private set; }
        public RelayCommand DeleteDesktopCommand { get; private set; }
        public RelayCommand ToggleDesktopSelectionCommand { get; private set; }
        public RelayCommand GoToSettingsCommand { get; private set; }

        public bool HasDesktops
        {
            get { return this.DesktopViewModels.Count > 0; }
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
                foreach (DesktopViewModel vm in this.DesktopViewModels)
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

        protected override void OnPresenting(object activationParameter)
        {
            if (null == _desktopViewModels)
            {
                _orderedConnections = OrderedObservableCollection<IModelContainer<RemoteConnectionModel>>
                                            .Create(this.ApplicationDataModel.LocalWorkspace.Connections.Models);
                _orderedConnections.Order = new DesktopModelAlphabeticalComparer();
                this.DesktopViewModels = TransformingObservableCollection<IModelContainer<RemoteConnectionModel>, IDesktopViewModel>
                                            .Create(_orderedConnections.Models, this.CreateDesktopViewModel, this.RemovedDesktopViewModel);

                INotifyPropertyChanged npc = this.DesktopViewModels;
                npc.PropertyChanged += DesktopViewModels_PropertyChanged;
            }
            //
            // update NavigationService for all DesktopViewModels
            //
            foreach (DesktopViewModel vm in _desktopViewModels)
            {
                vm.NavigationService = this.NavigationService;
                vm.Presented();
            }
        }

        private DesktopViewModel CreateDesktopViewModel(IModelContainer<RemoteConnectionModel> container)
        {
            Contract.Assert(container.Model is DesktopModel, "Data model for a desktop tile is not DesktopModel");

            DesktopViewModel dvm = new DesktopViewModel((DesktopModel)container.Model, container.Id, this.ApplicationDataModel, this)
            {
                SelectionEnabled = this.DesktopsSelectable
            };

            dvm.PropertyChanged += DesktopSelection_PropertyChanged;

            return dvm;
        }

        private void RemovedDesktopViewModel(IDesktopViewModel desktopViewModel)
        {
            desktopViewModel.PropertyChanged -= this.DesktopSelection_PropertyChanged;
        }

        private void AddDesktopExecute(object o)
        {
            NavigationService.PushModalView("AddOrEditDesktopView", new AddDesktopViewModelArgs());
        }

        private void DesktopViewModels_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Count"))
            {
                this.EmitPropertyChanged("HasDesktops");
                this.UpdateSelection();
            }
        }

        private void UpdateSelection()
        {
            int newSelectedCount = 0;            
            foreach (DesktopViewModel vm in this.DesktopViewModels)
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
            IList<IModelContainer<DesktopModel>> selectedDesktops = null;

            foreach (DesktopViewModel vm in this.DesktopViewModels)
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

        private void ToggleDesktopSelectionCommandExecute(object o)
        {

            this.DesktopsSelectable = !this.DesktopsSelectable;
        }

        private void GoToSettingsCommandExecute(object o)
        {
            this.NavigationService.NavigateToView("SettingsView", null);
        }
    }
}

