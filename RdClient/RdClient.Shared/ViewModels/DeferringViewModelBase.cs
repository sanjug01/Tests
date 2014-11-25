namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Navigation.Extensions;
    using System;
    using System.Threading;

    /// <summary>
    /// Base class for view models that wish to defer execution of actions delegates to the UI thread.
    /// </summary>
    public abstract class DeferringViewModelBase : ViewModelBase, IDeferredExecutionSite
    {
        private readonly ReaderWriterLockSlim _monitor;
        private IDeferredExecution _dispatcher;

        protected DeferringViewModelBase()
        {
            _monitor = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        protected void DeferToUI(Action action)
        {
            using(ReadWriteMonitor.Read(_monitor))
            {
                if (null == _dispatcher)
                    throw new DeferredExecutionExeption("Cannot defer execution from an inactive view model");

                _dispatcher.Defer(action);
            }
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
