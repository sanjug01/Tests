﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RdClient.Shared.ViewModels;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace RdClient.DesignTime
{
    public sealed class FakeConnectionCenterViewModel : IConnectionCenterViewModel
    {
        private readonly ObservableCollection<IDesktopViewModel> _desktopViewModelsSource;
        private readonly ReadOnlyObservableCollection<IDesktopViewModel> _desktopViewModels;

        private readonly ObservableCollection<IWorkspaceViewModel> _workspaceViewModelsSource;
        private readonly ReadOnlyObservableCollection<IWorkspaceViewModel> _workspaceViewModels;

        private readonly ObservableCollection<BarItemModel> _toolbarItemsSource;
        private readonly ReadOnlyObservableCollection<BarItemModel> _toolbarItems;

        public FakeConnectionCenterViewModel()
        {
            _desktopViewModelsSource = new ObservableCollection<IDesktopViewModel>();
            _desktopViewModels = new ReadOnlyObservableCollection<IDesktopViewModel>(_desktopViewModelsSource);
            _desktopViewModelsSource.Add(new FakeDesktopViewModel());
            _desktopViewModelsSource.Add(new FakeDesktopViewModel());
            _desktopViewModelsSource.Add(new FakeDesktopViewModel());
            _desktopViewModelsSource.Add(new FakeDesktopViewModel());
            _desktopViewModelsSource.Add(new FakeDesktopViewModel());
            _desktopViewModelsSource.Add(new FakeDesktopViewModel());
            _desktopViewModelsSource.Add(new FakeDesktopViewModel());
            _desktopViewModelsSource.Add(new FakeDesktopViewModel());
            _desktopViewModelsSource.Add(new FakeDesktopViewModel());

            _workspaceViewModelsSource = new ObservableCollection<IWorkspaceViewModel>();
            _workspaceViewModels = new ReadOnlyObservableCollection<IWorkspaceViewModel>(_workspaceViewModelsSource);
            _workspaceViewModelsSource.Add(new FakeWorkspaceViewModel());
            _workspaceViewModelsSource.Add(new FakeWorkspaceViewModel());
            _workspaceViewModelsSource.Add(new FakeWorkspaceViewModel());

            _toolbarItemsSource = new ObservableCollection<BarItemModel>();
            _toolbarItems = new ReadOnlyObservableCollection<BarItemModel>(_toolbarItemsSource);

            _toolbarItemsSource.Add(new SegoeGlyphBarButtonModel(SegoeGlyph.Home, new RelayCommand(o => { }), "Home"));
            _toolbarItemsSource.Add(new SeparatorBarItemModel());
            _toolbarItemsSource.Add(new SegoeGlyphBarButtonModel(SegoeGlyph.Home, new RelayCommand(o => { }), "Home"));
        }

        public RelayCommand AddDesktopCommand
        {
            get { return null; }
        }

        public ReadOnlyObservableCollection<IDesktopViewModel> DesktopViewModels
        {
            get { return _desktopViewModels; }
        }

        public ReadOnlyObservableCollection<IWorkspaceViewModel> WorkspaceViewModels
        {
            get { return _workspaceViewModels; }
        }

        ReadOnlyObservableCollection<BarItemModel> IConnectionCenterViewModel.ToolbarItems
        {
            get { return _toolbarItems; }
        }

        public bool HasDesktops
        {
            get { return true; }
        }


        public bool HasApps
        {
            get { return true; }
        }

        public bool ShowDesktops
        {
            get { return false; }
            set { }
        }

        public bool ShowApps
        {
            get { return true; }
            set { }
        }
    }
}
