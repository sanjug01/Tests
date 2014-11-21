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
        ObservableCollection<IDesktopViewModel> _destopViewModels;

        public FakeConnectionCenterViewModel()
        {
            _destopViewModels = new ObservableCollection<IDesktopViewModel>();
            _destopViewModels.Add(new FakeDesktopViewModel());
            _destopViewModels.Add(new FakeDesktopViewModel());
            _destopViewModels.Add(new FakeDesktopViewModel());
            _destopViewModels.Add(new FakeDesktopViewModel());
            _destopViewModels.Add(new FakeDesktopViewModel());
            _destopViewModels.Add(new FakeDesktopViewModel());
            _destopViewModels.Add(new FakeDesktopViewModel());
            _destopViewModels.Add(new FakeDesktopViewModel());
            _destopViewModels.Add(new FakeDesktopViewModel());
        }

        public ICommand AddDesktopCommand
        {
            get { return null; }
        }

        public ObservableCollection<IDesktopViewModel> DesktopViewModels
        {
            get { return _destopViewModels; }
        }

        public bool HasDesktops
        {
            get { return _destopViewModels.Count > 0; }
        }
    }
}
