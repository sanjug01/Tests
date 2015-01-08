namespace RdClient.LifeTimeManagement
{
    using RdClient.Shared.LifeTimeManagement;
    using System;
    using System.Diagnostics.Contracts;
    using Windows.ApplicationModel.Activation;

    public sealed class LifeTimeManager : ILifeTimeManager
    {
        private IRootFrameManager _rootFrameManager;
        private EventHandler<LaunchEventArgs> _launched;
        private EventHandler<SuspendEventArgs> _suspending;

        public IRootFrameManager RootFrameManager
        {
            get { return _rootFrameManager; }
            set { _rootFrameManager = value; }
        }

        public void OnLaunched(IActivationArgs e)
        {
            Contract.Assert(null != _rootFrameManager);

            if (ApplicationExecutionState.Running != e.PreviousExecutionState)
            {
                _rootFrameManager.LoadRoot(e);
            }

            EmitLaunched(e);
        }

        public void OnSuspending(object sender, ISuspensionArgs e)
        {
            var deferral = e.SuspendingOperation.Deferral;

            // TODO: Save application state and stop any background activity
            EmitSuspending(e);

            deferral.Complete();
        }


        event EventHandler<LaunchEventArgs> ILifeTimeManager.Launched
        {
            add { _launched += value; }
            remove { _launched -= value; }
        }

        event EventHandler<SuspendEventArgs> ILifeTimeManager.Suspending
        {
            add { _suspending += value; }
            remove { _suspending -= value; }
        }

        private void EmitLaunched(IActivationArgs args)
        {
            if (null != _launched)
                _launched(this, new LaunchEventArgs(args));
        }

        private void EmitSuspending(ISuspensionArgs args)
        {
            if (null != _suspending)
                _suspending(this, new SuspendEventArgs(args));
        }
    }
}
