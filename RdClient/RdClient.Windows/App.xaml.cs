namespace RdClient
{
    using RdClient.LifeTimeManagement;
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.LifeTimeManagement;
    using System.Diagnostics.Contracts;
    using Windows.ApplicationModel;
    using Windows.ApplicationModel.Activation;
    using Windows.UI.Xaml;

    public sealed partial class App : Application
    {
        private ILifeTimeManager _lifeTimeManager;

        public App()
        {
            Contract.ContractFailed += this.OnCodeContractFailed;
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;

            RdTrace.TraceNrm("Initializing Tracer");
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            ActivationArgs aa = new ActivationArgs(
                e.Arguments,
                e.TileId,
                e.Kind,
                e.PreviousExecutionState,
                e.SplashScreen,
                e.CurrentlyShownApplicationViewId,
                e.PrelaunchActivated,
                null);

            this.LifeTimeManager.OnLaunched(aa);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspensionArgs.SuspendingOperationWrapper mso = new SuspensionArgs.SuspendingOperationWrapper(e.SuspendingOperation.Deadline, e.SuspendingOperation.GetDeferral());
            SuspensionArgs sa = new SuspensionArgs(mso);

            this.LifeTimeManager.OnSuspending(sender, sa);
        }

        private ILifeTimeManager LifeTimeManager
        {
            get
            {
                Contract.Ensures(null != Contract.Result<ILifeTimeManager>());
                if(null == _lifeTimeManager)
                {
                    _lifeTimeManager = this.Resources["LifeTimeManager"] as ILifeTimeManager;
                    Contract.Assert(null != _lifeTimeManager);
                }

                return _lifeTimeManager;
            }
        }

        private void OnCodeContractFailed(object sender, ContractFailedEventArgs e)
        {
            switch(e.FailureKind)
            {
                case ContractFailureKind.Assert:
                    e.SetHandled();
#if DEBUG
                    //
                    // Throw an exception with information about the assertion.
                    //
                    throw new ContractAssertionException(e);
#else
                    //
                    // TODO: log the assertion for telemetry and perhaps trigger a runtime error.
                    //
                    break;
#endif
            }
        }
    }
}
