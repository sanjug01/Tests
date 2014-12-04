using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.ViewModels;
using System.Threading.Tasks;

namespace RdClient.Shared.Factories
{
    public interface IObjectFactory
    {
        Task<IDataModel> CreateDataModel();
        INavigationService CreateNavigationService();
        INavigationExtension CreateDataModelExtension(IDataModel dataModel);
        INavigationExtension CreateDisconnectStringExtension();
        INavigationExtension CreateDeferredExecutionExtension();
        INavigationExtension CreateApplicationBarExtension(IApplicationBarViewModel applicationBarViewModel);
    }
}
