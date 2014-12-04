using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.ViewModels;
using RdClient.Shared.Factories;
using System.Threading.Tasks;

namespace RdClient.Factories
{
    public class ObjectFactory : IObjectFactory
    {
        private DataModelFactory _dataModelFactory = new DataModelFactory();
        private NavigationServiceFactory _navigationServiceFactory = new NavigationServiceFactory();

        public async Task<IDataModel> CreateDataModel()
        {
            return await _dataModelFactory.CreateDataModel();
        }

        public INavigationService CreateNavigationService()
        {
            return _navigationServiceFactory.CreateNavigationService();
        }

        public INavigationExtension CreateDataModelExtension(IDataModel dataModel)
        {
            return _navigationServiceFactory.CreateDataModelExtension(dataModel);
        }

        public INavigationExtension CreateDisconnectStringExtension()
        {
            return _navigationServiceFactory.CreateDisconnectStringExtension();
        }

        public INavigationExtension CreateDeferredExecutionExtension()
        {
            return _navigationServiceFactory.CreateDeferredExecutionExtension();
        }

        public INavigationExtension CreateApplicationBarExtension(IApplicationBarViewModel applicationBarViewModel)
        {
            return _navigationServiceFactory.CreateApplicationBarExtension(applicationBarViewModel);
        }
    }
}
