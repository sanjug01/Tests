namespace RdClient
{
    using RdClient.LifeTimeManagement;
    using RdClient.Shared.CxWrappers;
    using System.Diagnostics.Contracts;
    using Windows.ApplicationModel;
    using Windows.ApplicationModel.Activation;
    using Windows.UI.Xaml;

    public sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;

            RdTrace.TraceNrm("Initializing Tracer");
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Contract.Assert(null != this.LifeTimeManager);

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
            Contract.Assert(null != this.LifeTimeManager);

            SuspensionArgs.SuspendingOperationWrapper mso = new SuspensionArgs.SuspendingOperationWrapper(e.SuspendingOperation.Deadline, e.SuspendingOperation.GetDeferral());
            SuspensionArgs sa = new SuspensionArgs(mso);

            this.LifeTimeManager.OnSuspending(sender, sa);
        }
    }
}