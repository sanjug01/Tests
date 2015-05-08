namespace RdClient.Factories
{
    using RdClient.Data;
    using RdClient.Helpers;
    using RdClient.Shared.CxWrappers;
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
    using System;

    public sealed class AppInitializer
    {
        private INavigationService _navigationService;
        private DeferredCommand _applicationDataSaver;
        private BackButtonHandler _backButtonHandler;

        private readonly int SaveDataDelayMilliseconds = 100;
        private readonly NavigationServiceFactory _navigationServiceFactory = new NavigationServiceFactory();

        public IViewPresenter ViewPresenter { private get; set; }
        public string LandingPage { private get; set; }
        public ILifeTimeManager LifeTimeManager { private get; set; }
        public IRdpConnectionSource ConnectionSource { private get; set; }
        public IDeviceCapabilities DeviceCapabilities { private get; set; }

        internal void CreateBackButtonHandler(SystemNavigationManager systemNavigationManager)
        {
            _backButtonHandler = new BackButtonHandler(systemNavigationManager, _navigationService);
        }

        public void Initialize()
        {
            Contract.Assert(this.ViewPresenter != null);
            Contract.Assert(!string.IsNullOrEmpty(this.LandingPage));
            Contract.Assert(null != this.LifeTimeManager);
            Contract.Assert(null != this.ConnectionSource);
            Contract.Assert(null != this.DeviceCapabilities);

            ITimerFactory timerFactory = new WinrtThreadPoolTimerFactory();
            IDeferredExecution deferredExecution = new CoreDispatcherDeferredExecution() { Priority = CoreDispatcherPriority.Normal };

            ApplicationDataModel appDataModel = new ApplicationDataModel()
            {
                RootFolder = new ApplicationDataLocalStorageFolder() { FolderName = "RemoteDesktopData" },
                ModelSerializer = new SerializableModelSerializer(),
                DataScrambler = new Rc4DataScrambler()
            };
            appDataModel.Compose();
            //
            // initialize the loaded workspaces
            //
            foreach (IModelContainer<OnPremiseWorkspaceModel> workspace in appDataModel.OnPremWorkspaces.Models)
            {
                workspace.Model.Initialize(new RadcClient(new RadcEventSource(), new TaskExecutor()), appDataModel);
                workspace.Model.TryAndResubscribe();
            }
            //All the resources for the workspaces are cached internally by RadcClient. Here we load them into our workspaces
            RadcClient radcClient = new RadcClient(new RadcEventSource(), new TaskExecutor());
            radcClient.StartGetCachedFeeds(); 

            ISessionFactory sessionFactory = new SessionFactory(this.ConnectionSource, deferredExecution, timerFactory);

            _navigationService = this.CreateNavigationService();

            _navigationService.Presenter = this.ViewPresenter;

            _navigationService.Extensions.Add(this.CreateDataModelExtension(appDataModel));
            _navigationService.Extensions.Add(this.CreateDeferredExecutionExtension(deferredExecution));
            _navigationService.Extensions.Add(new TimerFactoryExtension(timerFactory));
            _navigationService.Extensions.Add(new SessionFactoryExtension() { SessionFactory = sessionFactory });
            _navigationService.Extensions.Add(new DeviceCapabilitiesExtension() { DeviceCapabilities = this.DeviceCapabilities });
            _navigationService.Extensions.Add(new LifeTimeExtension() { LifeTimeManager = this.LifeTimeManager });

            _applicationDataSaver = new DeferredCommand(appDataModel.Save, deferredExecution, timerFactory, SaveDataDelayMilliseconds);

            _navigationService.NavigateToView(this.LandingPage, null);
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
    }
}
