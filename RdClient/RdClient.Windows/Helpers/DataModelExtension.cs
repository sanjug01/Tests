using RdClient.Models;
using RdClient.Shared.Models;
using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace RdClient.Shared.Navigation
{
    public class DataModelExtension : INavigationExtension
    {
        public DataModelExtension()
        {

        }

        public void Presenting(IViewModel viewModel)
        {
            IViewModelWithData vmwd = viewModel as IViewModelWithData;
            if(null != vmwd)
            {
                // do something
            }
        }
    }
}
