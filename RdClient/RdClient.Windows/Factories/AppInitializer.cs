namespace RdClient.Factories
{
    using RdClient.Data;
    using RdClient.Helpers;
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.LifeTimeManagement;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using RdClient.Shared.ViewModels;
    using System.Diagnostics.Contracts;
    using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

    public class AppInitializer
    {
        private INavigationService _navigationService;
        private DeferredCommand _applicationDataSaver;

        private readonly int SaveDataDelayMilliseconds = 100;
        private readonly NavigationServiceFactory _navigationServiceFactory = new NavigationServiceFactory();

        public IViewPresenter ViewPresenter { private get; set; }
        public IApplicationBarViewModel AppBarViewModel { private get; set; }
        public string LandingPage { private get; set; }
        public ILifeTimeManager LifeTimeManager { private get; set; }
        public Button BackButton { private get; set; }

        public void Initialiaze()
        {
            Contract.Assert(this.ViewPresenter != null);
            Contract.Assert(this.AppBarViewModel != null);
            Contract.Assert(!string.IsNullOrEmpty(this.LandingPage));
            Contract.Assert(null != this.LifeTimeManager);

            ITimerFactory timerFactory = new WinrtThreadPoolTimerFactory();
            IDeferredExecution deferredExecution = new CoreDispatcherDeferredExecution() { Priority = CoreDispatcherPriority.Normal };

            ApplicationDataModel appDataModel = new ApplicationDataModel()
            {
                RootFolder = new ApplicationDataLocalStorageFolder() { FolderName = "RemoteDesktopData" },
                ModelSerializer = new SerializableModelSerializer()
            };

            ISessionFactory sessionFactory = new SessionFactory();
            sessionFactory.DeferedExecution = deferredExecution;

            _navigationService = this.CreateNavigationService();

            _navigationService.Presenter = this.ViewPresenter;
            _navigationService.PushingFirstModalView += (s, e) => this.ViewPresenter.PresentingFirstModalView();
            _navigationService.DismissingLastModalView += (s, e) => this.ViewPresenter.DismissedLastModalView();

            _navigationService.Extensions.Add(this.CreateDataModelExtension(appDataModel));
            _navigationService.Extensions.Add(this.CreateDeferredExecutionExtension(deferredExecution));
            _navigationService.Extensions.Add(this.CreateApplicationBarExtension(this.AppBarViewModel));
            _navigationService.Extensions.Add(new TimerFactoryExtension(timerFactory));
            _navigationService.Extensions.Add(new SessionFactoryExtension() { SessionFactory = sessionFactory });

            _applicationDataSaver = new DeferredCommand(appDataModel.Save, deferredExecution, timerFactory, SaveDataDelayMilliseconds);

            _navigationService.NavigateToView(this.LandingPage, null);

            //This will be replaced with a hookup to the back button pressed API once we move to Windows 10
            this.BackButton.Command = _navigationService.BackCommand;
        }

        public INavigationService CreateNavigationService()
        {
            return _navigationServiceFactory.CreateNavigationService();
        }

        public INavigationExtension CreateDataModelExtension(ApplicationDataModel appDataModel)
        {
            return _navigationServiceFactory.CreateDataModelExtension(appDataModel);
        }

        public INavigationExtension CreateDeferredExecutionExtension(IDeferredExecution deferredExecution)
        {
            return _navigationServiceFactory.CreateDeferredExecutionExtension(deferredExecution);
        }

        public INavigationExtension CreateApplicationBarExtension(IApplicationBarViewModel applicationBarViewModel)
        {
            return _navigationServiceFactory.CreateApplicationBarExtension(applicationBarViewModel);
        }
    }
}
