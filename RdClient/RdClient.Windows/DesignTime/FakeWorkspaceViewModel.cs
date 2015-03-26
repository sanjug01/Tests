using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RdClient.DesignTime
{
    public class FakeWorkspaceViewModel : IWorkspaceViewModel
    {
        private string _name;
        private ReadOnlyObservableCollection<IRemoteResourceViewModel> _remoteResourceViewModels;

        public FakeWorkspaceViewModel()
        {
            _name = "This is a fake workspace";
            ObservableCollection<IRemoteResourceViewModel> remoteResourceViewModelsSource = new ObservableCollection<IRemoteResourceViewModel>();
            _remoteResourceViewModels = new ReadOnlyObservableCollection<IRemoteResourceViewModel>(remoteResourceViewModelsSource);
            remoteResourceViewModelsSource.Add(new FakeRemoteResourceViewModel());
            remoteResourceViewModelsSource.Add(new FakeRemoteResourceViewModel());
            remoteResourceViewModelsSource.Add(new FakeRemoteResourceViewModel());
            remoteResourceViewModelsSource.Add(new FakeRemoteResourceViewModel());
            remoteResourceViewModelsSource.Add(new FakeRemoteResourceViewModel());
            remoteResourceViewModelsSource.Add(new FakeRemoteResourceViewModel());
            remoteResourceViewModelsSource.Add(new FakeRemoteResourceViewModel());
            remoteResourceViewModelsSource.Add(new FakeRemoteResourceViewModel());
            remoteResourceViewModelsSource.Add(new FakeRemoteResourceViewModel());
        }

        public string Name
        {
            get { return _name; }
        }

        public ICommand DeleteCommand
        {
            get { return null; }
        }

        public ICommand EditCommand
        {
            get { return null; }
        }

        public ICommand RefreshCommand
        {
            get { return null; }
        }

        public ReadOnlyObservableCollection<IRemoteResourceViewModel> RemoteResourceViewModels
        {
            get { return _remoteResourceViewModels; }
        }
    }
}
