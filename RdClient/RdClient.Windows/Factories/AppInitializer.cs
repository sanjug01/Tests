namespace RdClient.Factories
{
    using RdClient.Data;
    using RdClient.Helpers;
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Input.Keyboard;
    using RdClient.Shared.LifeTimeManagement;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using RdClient.Shared.Telemetry;
    using RdClient.Telemetry;
    using System.Diagnostics.Contracts;
    using Windows.UI.Core;

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
        public ITelemetryClient TelemetryClient { get; set; }
        public IInputPanelFactory InputPanelFactory { private get; set; }

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
            ISessionFactory sessionFactory;

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
            //
            // All the resources for the workspaces are cached internally by RadcClient. Here we load them into our workspaces
            //
            RadcClient radcClient = new RadcClient(new RadcEventSource(), new TaskExecutor());
            radcClient.StartGetCachedFeeds();

            //
            //Setup the clipboard manager
            //
            ClipboardManager.Intialize(deferredExecution);

            _navigationService = this.CreateNavigationService(this.TelemetryClient);
            _navigationService.Presenter = this.ViewPresenter;
            //
            // Inject and enable telemetry if necessary.
            //
            if (null != this.TelemetryClient)
            {
                this.TelemetryClient.IsActive = appDataModel.Settings.SendFeedback;

                _navigationService.Extensions.Add(new TelemetryExtension() { Client = this.TelemetryClient });
                //
                // Create a session factory that uses the injected telemetry client.
                //
                sessionFactory = new SessionFactory(this.ConnectionSource, deferredExecution, timerFactory,
                    this.DeviceCapabilities, this.TelemetryClient);
                //
                // Report data model statistics telemetry
                //
                if(this.TelemetryClient.IsActive)
                {
                    ITelemetryEvent te = this.TelemetryClient.MakeEvent("LaunchConfiguration");
                    te.AddMetric("localDesktopCount", appDataModel.LocalWorkspace.Connections.Models.Count);
                    te.AddMetric("credentialsCount", appDataModel.Credentials.Models.Count);
                    te.AddMetric("gatewaysCount", appDataModel.Gateways.Models.Count);
                    te.Report();
                }
            }
            else
            {
                //
                // Create a session factory that uses the dummy telemetry client, because the session factory
                // requires a functional telemetry client.
                //
                sessionFactory = new SessionFactory(this.ConnectionSource, deferredExecution, timerFactory,
                    this.DeviceCapabilities, new DummyTelemetryClient());
            }
            Contract.Assert(null != sessionFactory);

            _navigationService.Extensions.Add(this.CreateDataModelExtension(appDataModel));
            _navigationService.Extensions.Add(this.CreateDeferredExecutionExtension(deferredExecution));
            _navigationService.Extensions.Add(new TimerFactoryExtension(timerFactory));
            _navigationService.Extensions.Add(new SessionFactoryExtension() { SessionFactory = sessionFactory });
            _navigationService.Extensions.Add(new DeviceCapabilitiesExtension() { DeviceCapabilities = this.DeviceCapabilities });
            _navigationService.Extensions.Add(new LifeTimeExtension() { LifeTimeManager = this.LifeTimeManager });
            //
            // If an input panel factory has been injected in the app initializer, create an extension that will
            // inject it in view models; otherwise, the input panel factory is injected elsewhere.
            //
            if (null != this.InputPanelFactory)
                _navigationService.Extensions.Add(new InputPanelFactoryExtension(this.InputPanelFactory));
            //
            // Set up deferred execution of the app data's Save command. As soon as the command reports that it can be executed,
            // DeferredCommand will set a timer for the specified duration and when the timer will fire it will execute the command.
            // After execution of the command DeferredCommand will expect the ICommand to disable itself.
            //
            _applicationDataSaver = new DeferredCommand(appDataModel.Save, deferredExecution, timerFactory, SaveDataDelayMilliseconds);
            //
            // Navigate to the landing view.
            //
            _navigationService.NavigateToView(this.LandingPage, null);
        }

        public INavigationService CreateNavigationService(ITelemetryClient telemetryClient)
        {
            return _navigationServiceFactory.CreateNavigationService(telemetryClient);
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
