using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.ViewModels;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;

namespace RdClient.Factories
{
    public class AppInitializer
    {
        private IDataModel _dataModel;
        private INavigationService _navigationService;

        private DataModelFactory _dataModelFactory = new DataModelFactory();
        private NavigationServiceFactory _navigationServiceFactory = new NavigationServiceFactory();

        public IViewPresenter ViewPresenter { private get; set; }
        public IApplicationBarViewModel AppBarViewModel { private get; set; }
        public string LandingPage { private get; set; }

        public void Initialiaze()
        {
            Contract.Assert(this.ViewPresenter != null);
            Contract.Assert(this.AppBarViewModel != null);
            Contract.Assert(this.LandingPage != null);

            _dataModel = this.CreateDataModel();

            _navigationService = this.CreateNavigationService();

            _navigationService.Presenter = this.ViewPresenter;
            _navigationService.PushingFirstModalView += (s, e) => this.ViewPresenter.PresentingFirstModalView();
            _navigationService.DismissingLastModalView += (s, e) => this.ViewPresenter.DismissedLastModalView();

            _navigationService.Extensions.Add(this.CreateDataModelExtension(_dataModel));
            _navigationService.Extensions.Add(this.CreateDeferredExecutionExtension());
            _navigationService.Extensions.Add(this.CreateApplicationBarExtension(this.AppBarViewModel));
            _navigationService.Extensions.Add(this.CreateDisconnectStringExtension());
            _navigationService.NavigateToView(this.LandingPage, null);
        }

        public IDataModel CreateDataModel()
        {
            return _dataModelFactory.CreateDataModel();
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
