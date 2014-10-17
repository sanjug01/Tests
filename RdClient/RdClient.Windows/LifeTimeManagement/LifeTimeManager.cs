using RdClient.LifeTimeManagement;
using System.Diagnostics.Contracts;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;

namespace RdClient
{
    public class LifeTimeManager : ILifeTimeManager
    {
        private IRootFrameManager _rootFrameManager;

        public void Initialize(IRootFrameManager rootFrameManager)
        {
            Contract.Requires(rootFrameManager != null);

            _rootFrameManager = rootFrameManager;
        }

        public void OnLaunched(IActivationArgs e)
        {
            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                // TODO: Load state from previously suspended application
            }

            _rootFrameManager.LoadRoot(e);
        }

        public void OnSuspending(object sender, ISuspensionArgs e)
        {
            var deferral = e.SuspendingOperation.Deferral;

            // TODO: Save application state and stop any background activity

            deferral.Complete();
        }

    }
}
