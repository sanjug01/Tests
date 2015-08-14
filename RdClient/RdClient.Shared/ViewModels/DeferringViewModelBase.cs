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
    public abstract class DeferringViewModelBase : ViewModelBase, IDeferredExecutionSite, IExecutionDeferrer
    {
        private readonly ReaderWriterLockSlim _monitor;
        private IDeferredExecution _dispatcher;

        protected IDeferredExecution Dispatcher
        {
            get { return _dispatcher; }
        }


        protected DeferringViewModelBase()
        {
            _monitor = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            this.ExecutionDeferrer = this;
        }

        public void DeferToUI(Action action)
        {
            if (!TryDeferToUI(action))
            {
                throw new DeferredExecutionException("Cannot defer execution from an inactive view model");
            }
        }

        public bool TryDeferToUI(Action action)
        {
            bool succeeded = false;
            //
            // Enter the upgradeable read lock because the dispatcher may execute the action delegate
            // immediately, and the delegate may dismiss the view model, in which case SetDeferredExecution
            // will be called and try enter the write lock.
            //
            using (ReadWriteMonitor.UpgradeableRead(_monitor))
            {
                if (null != _dispatcher)
                {
                    _dispatcher.Defer(action);
                    succeeded = true;
                }
            }
            return succeeded;
        }

        protected override void DisposeManagedState()
        {
            base.DisposeManagedState();
            _monitor.Dispose();
        }

        void IDeferredExecutionSite.SetDeferredExecution(IDeferredExecution defEx)
        {
            using (ReadWriteMonitor.Write(_monitor))
                _dispatcher = defEx;
        }
    }
}
