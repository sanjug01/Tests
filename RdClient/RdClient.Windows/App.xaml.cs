using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using RdClient.LifeTimeManagement;
using RdClient.Shared.CxWrappers;
using System.Runtime.Serialization;
using RdClient.Shared.Models;
using Windows.Storage;
using RdClient.Models;
using RdClient.Shared.ViewModels;

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

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            ActivationArgs aa = new ActivationArgs(
                e.Arguments,
                e.TileId,
                e.Kind,
                e.PreviousExecutionState,
                e.SplashScreen,
                e.CurrentlyShownApplicationViewId,
                e.PrelaunchActivated);

            //Put temp dependency injection code here

            ViewModelLocator vmLocator = new ViewModelLocator() { DataModel = dataModel };
            Application.Current.Resources["AppViewModelLocator"] = vmLocator; 

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