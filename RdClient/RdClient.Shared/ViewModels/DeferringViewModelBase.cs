namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Navigation.Extensions;
    using System;
    using System.Threading;

    /// <summary>
    /// Base class for view models that wish to defer execution of actions delegates to the UI thread.
    /// </summary>
    public abstract class DeferringViewModelBase : ViewModelBase, IDeferredExecutionSite, IExecutionDeferrer
    {
        private readonly ReaderWriterLockSlim _monitor;
        private IDeferredExecution _dispatcher;

        protected DeferringViewModelBase()
        {
            _monitor = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        public void DeferToUI(Action action)
        {
            if (!TryDeferToUI(action))
            {
                throw new DeferredExecutionExeption("Cannot defer execution from an inactive view model");
            }
        }

        public bool TryDeferToUI(Action action)
        {
            bool succeeded = false;
            using (ReadWriteMonitor.Read(_monitor))
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
