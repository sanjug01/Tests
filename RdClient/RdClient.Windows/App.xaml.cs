using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using RdClient.LifeTimeManagement;
using RdClient.Shared.CxWrappers;

namespace RdClient
{
    public sealed partial class App : Application
    {

        private LifeTimeManager _lifeTimeManager;

        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;

            RdTrace.TraceNrm("Initializing Tracer");

            _lifeTimeManager = new LifeTimeManager();
            _lifeTimeManager.Initialize(new RootFrameManager());
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
                e.PrelaunchActivated);

            _lifeTimeManager.OnLaunched(aa);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspensionArgs.SuspendingOperationWrapper mso = new SuspensionArgs.SuspendingOperationWrapper(e.SuspendingOperation.Deadline, e.SuspendingOperation.GetDeferral());
            SuspensionArgs sa = new SuspensionArgs(mso);

            _lifeTimeManager.OnSuspending(sender, sa);
        }
    }
}