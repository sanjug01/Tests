using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RdClient.Shared.Models;
using RdClient.Shared.ViewModels;

namespace RdClient.Shared.ViewModels
{
    public class ViewModelLocator
    {
        private IDataModel _dataModel;

        public IDataModel DataModel
        {
            set { _dataModel = value; }
        }

        public ConnectionCenterViewModel ConnectionCenterViewModel
        {
            get { return SetVmDataModelAndReturn(new ConnectionCenterViewModel()); }
        }

        public SessionViewModel SessionViewModel
        {
            get { return SetVmDataModelAndReturn(new SessionViewModel()); }
        }

        public AddOrEditDesktopViewModel AddOrEditDesktopViewModel
        {
            get { return SetVmDataModelAndReturn(new AddOrEditDesktopViewModel()); }
        }

        public DeleteDesktopsViewModel DeleteDesktopsViewModel
        {
            get { return SetVmDataModelAndReturn(new DeleteDesktopsViewModel()); }
        }

        public TestsViewModel TestsViewModel 
        {
            get { return SetVmDataModelAndReturn(new TestsViewModel()); }
        }

        private T SetVmDataModelAndReturn<T>(T vm) where T : ViewModelBase
        {
            vm.DataModel = _dataModel;
            return vm;
        }
    }
}
