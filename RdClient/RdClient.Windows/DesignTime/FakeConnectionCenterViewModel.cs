namespace RdClient.DesignTime
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ViewModels;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    public sealed class FakeConnectionCenterViewModel : DisposableObject, IConnectionCenterViewModel
    {
        private readonly ObservableCollection<IDesktopViewModel> _desktopViewModelsSource;
        private readonly ReadOnlyObservableCollection<IDesktopViewModel> _desktopViewModels;

        private readonly ObservableCollection<IWorkspaceViewModel> _workspaceViewModelsSource;
        private readonly ReadOnlyObservableCollection<IWorkspaceViewModel> _workspaceViewModels;

        private readonly ObservableCollection<BarItemModel> _toolbarItemsSource;
        private readonly ReadOnlyObservableCollection<BarItemModel> _toolbarItems;
        private readonly IViewVisibility _accessoryViewVisibility;
        private readonly RelayCommand _cancelAccessoryView;

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

            _toolbarItemsSource.Add(new SeparatorBarItemModel());

            _accessoryViewVisibility = ViewVisibility.Create(false);
            _cancelAccessoryView = new RelayCommand(o => { });
        }

        public RelayCommand AddDesktopCommand
        {
            get { return null; }
        }

        public ReadOnlyObservableCollection<IDesktopViewModel> DesktopViewModels
        {
            get { return _desktopViewModels; }
        }

        public IViewItemsSource DesktopViewModelsSource
        {
            get { return null; }
        }

        public ReadOnlyObservableCollection<IWorkspaceViewModel> WorkspaceViewModels
        {
            get { return _workspaceViewModels; }
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

        public IViewVisibility AccessoryViewVisibility
        {
            get { return _accessoryViewVisibility; }
        }

        public ICommand CancelAccessoryView
        {
            get { return _cancelAccessoryView; }
        }
    }
}
