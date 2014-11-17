using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RdClient.Shared.Models;
using RdClient.Shared.ViewModels;

namespace RdClient.Shared.Models
{
    class ViewModelLocator
    {
        private IDataModel _dataModel;

        public IDataModel DataModel
        {
            get { return _dataModel; }
            set { _dataModel = value; }
        }

        public async Task<ConnectionCenterViewModel> GetConnectionCenterViewModel()
        {
            return await SetVmDataModelAndReturn(new ConnectionCenterViewModel());
        }

        private async Task<T> SetVmDataModelAndReturn<T>(T vm) where T : ViewModelBase
        {
            vm.DataModel = this.DataModel;
            if (!this.DataModel.Loaded)
            {
                await this.DataModel.LoadFromStorage();
            }
            return vm;
        }
    }
}
