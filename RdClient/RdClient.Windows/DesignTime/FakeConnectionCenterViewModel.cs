using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RdClient.Shared.ViewModels;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace RdClient.DesignTime
{
    public class FakeConnectionCenterViewModel : IConnectionCenterViewModel
    {
        private readonly ObservableCollection<IDesktopViewModel> _desktopViewModelsSource;
        private readonly ReadOnlyObservableCollection<IDesktopViewModel> _desktopViewModels;

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
        }

        public RelayCommand AddDesktopCommand
        {
            get { return null; }
        }

        public ReadOnlyObservableCollection<IDesktopViewModel> DesktopViewModels
        {
            get { return _desktopViewModels; }
        }

        public bool HasDesktops
        {
            get { return true; }
        }
    }
}
