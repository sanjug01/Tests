using RdClient.Shared.Navigation.Extensions;
using System;
using System.Threading;

namespace RdClient.Shared.Helpers
{
    public class ExecutionDeferrer : ISynchronizedDeferrer
    {
        private readonly ReaderWriterLockSlim _monitor;
        private readonly IDeferredExecution _dispatcher;

        public ExecutionDeferrer(IDeferredExecution dispatcher, ReaderWriterLockSlim monitor = null)
        {
            _dispatcher = dispatcher;
            _monitor = (_monitor == null) ? new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion) : monitor;
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
    }
}
