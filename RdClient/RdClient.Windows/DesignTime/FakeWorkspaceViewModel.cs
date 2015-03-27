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
        public FakeWorkspaceViewModel()
        {
            this.Name = "This is a fake workspace";
            this.State = "State.A_Ok";
            this.Error = "Error.SuperCriticalFailure";
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
            this.RemoteResourceViewModels = remoteResourceViewModelsSource;
        }

        public string Name { get; set; }

        public string State { get; set; }

        public string Error { get; set; }

        public List<IRemoteResourceViewModel> RemoteResourceViewModels { get; set; }

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
    }
}
