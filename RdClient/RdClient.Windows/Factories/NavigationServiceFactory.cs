using RdClient.Helpers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.Navigation.Extensions;
using RdClient.Shared.ViewModels;
using Windows.UI.Core;

namespace RdClient.Factories
{
    class NavigationServiceFactory
    {
        public INavigationService CreateNavigationService()
        {
            IPresentableViewFactory viewFactory = new PresentableViewFactory<PresentableViewConstructor>();
            viewFactory.AddViewClass("ConnectionCenterView", typeof(Views.ConnectionCenterView));
            viewFactory.AddViewClass("SessionView", typeof(Views.SessionView));
            viewFactory.AddViewClass("AddOrEditDesktopView", typeof(Views.AddOrEditDesktopView));
            viewFactory.AddViewClass("AddUserView", typeof(Views.AddUserView));
            viewFactory.AddViewClass("ErrorMessageView", typeof(Views.ErrorMessageView));
            viewFactory.AddViewClass("DeleteDesktopsView", typeof(Views.DeleteDesktopsView));
            viewFactory.AddViewClass("CertificateValidationView", typeof(Views.CertificateValidationView));
            viewFactory.AddViewClass("SettingsView", typeof(Views.SettingsView));
            viewFactory.AddViewClass("DeleteUserView", typeof(Views.DeleteUserView));

            DispatchedNavigationService navigationService = new DispatchedNavigationService();
            navigationService.ViewFactory = viewFactory;

            return navigationService;
        }

        public INavigationExtension CreateDataModelExtension(RdDataModel dataModel)
        {
            DataModelExtension dataModelExtension = new DataModelExtension();
            dataModelExtension.DataModel = dataModel;

            return dataModelExtension;
        }

        public INavigationExtension CreateDeferredExecutionExtension(IDeferredExecution deferredExecution)
        {
            return new DeferredExecutionExtension() { DeferredExecution = deferredExecution };
        }

        public INavigationExtension CreateApplicationBarExtension(IApplicationBarViewModel applicationBarViewModel)
        {
            ApplicationBarExtension applicationBarExtension = new ApplicationBarExtension();
            applicationBarExtension.ViewModel = applicationBarViewModel;

            return applicationBarExtension;
        }
    }
}
