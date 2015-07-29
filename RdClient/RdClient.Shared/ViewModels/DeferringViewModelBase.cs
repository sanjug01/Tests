namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Navigation.Extensions;
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;

    /// <summary>
    /// Base class for view models that wish to defer execution of actions delegates to the UI thread.
    /// </summary>
    public abstract class DeferringViewModelBase : ViewModelBase, IDeferredExecutionSite, ISynchronizedDeferrer
    {
        private readonly ReaderWriterLockSlim _monitor;


        protected DeferringViewModelBase()
        {
            _monitor = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        public void DeferToUI(Action action)
        {
            this.ExecutionDeferrer.DeferToUI(action);
        }

        public bool TryDeferToUI(Action action)
        {
            return this.ExecutionDeferrer.TryDeferToUI(action);
        }

        protected override void DisposeManagedState()
        {
            base.DisposeManagedState();
            _monitor.Dispose();
        }

        void IDeferredExecutionSite.SetDeferredExecution(IExeucutionDeferrer dispatcher)
        {
            this.ExecutionDeferrer = new ExecutionDeferrer(dispatcher);
        }
    }
}
