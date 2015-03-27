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
        private List<IRemoteResourceViewModel> _remoteResourceViewModels;

        public FakeWorkspaceViewModel()
        {
            _name = "This is a fake workspace";
            List<IRemoteResourceViewModel> remoteResourceViewModelsSource = new List<IRemoteResourceViewModel>();            
            remoteResourceViewModelsSource.Add(new FakeRemoteResourceViewModel());
            remoteResourceViewModelsSource.Add(new FakeRemoteResourceViewModel());
            remoteResourceViewModelsSource.Add(new FakeRemoteResourceViewModel());
            remoteResourceViewModelsSource.Add(new FakeRemoteResourceViewModel());
            remoteResourceViewModelsSource.Add(new FakeRemoteResourceViewModel());
            remoteResourceViewModelsSource.Add(new FakeRemoteResourceViewModel());
            remoteResourceViewModelsSource.Add(new FakeRemoteResourceViewModel());
            remoteResourceViewModelsSource.Add(new FakeRemoteResourceViewModel());
            remoteResourceViewModelsSource.Add(new FakeRemoteResourceViewModel());
            _remoteResourceViewModels = remoteResourceViewModelsSource;
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

        public List<IRemoteResourceViewModel> RemoteResourceViewModels
        {
            get { return _remoteResourceViewModels; }
        }
    }
}
