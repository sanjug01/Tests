using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using RdClient.LifeTimeManagement;

namespace RdClient
{
    public sealed partial class App : Application
    {

        private LifeTimeManager _lifeTimeManager;

        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;

            _lifeTimeManager = new LifeTimeManager();
            _lifeTimeManager.Initialize(new RootFrameManager());
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            ActivationArgs aa = new ActivationArgs();

            aa.Arguments = e.Arguments;
            aa.TileId = e.TileId;
            aa.Kind = e.Kind;
            aa.PreviousExecutionState = e.PreviousExecutionState;
            aa.SplashScreen = e.SplashScreen;
            aa.CurrentlyShownApplicationViewId = e.CurrentlyShownApplicationViewId;
            aa.PrelaunchActivated = e.PrelaunchActivated;

            _lifeTimeManager.OnLaunched(aa);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspensionArgs sa = new SuspensionArgs();
            MySuspendingOperation mso = new MySuspendingOperation();
            mso.Deadline = e.SuspendingOperation.Deadline;
            mso.Deferral = e.SuspendingOperation.GetDeferral();
            sa.SuspendingOperation = mso;

            _lifeTimeManager.OnSuspending(sender, sa);
        }
    }
}