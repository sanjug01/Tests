namespace RdClient.Shared.Navigation
{
    using RdClient.Shared.Helpers;
    using System;
    using System.Threading;

    /// <summary>
    /// Helper class that implements the IPresentationCompletion interface and emits an event
    /// when the interface is called.
    /// </summary>
    public sealed class ModalPresentationCompletion : DisposableObject, IPresentationCompletion
    {
        private readonly ReaderWriterLockSlim _monitor;
        private EventHandler<PresentationCompletionEventArgs> _handler;

        public ModalPresentationCompletion()
        {
            _monitor = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        public event EventHandler<PresentationCompletionEventArgs> Completed
        {
            add
            {
                using (ReadWriteMonitor.Write(_monitor))
                    _handler += value;
            }

            remove
            {
                using (ReadWriteMonitor.Write(_monitor))
                    _handler -= value;
            }
        }

        protected override void DisposeManagedState()
        {
            base.DisposeManagedState();
            _monitor.Dispose();
        }

        void IPresentationCompletion.Completed(IPresentableView view, object result)
        {
            EventHandler<PresentationCompletionEventArgs> handler;

            using (ReadWriteMonitor.Read(_monitor))
                handler = _handler;

            if (null != handler)
                handler(this, new PresentationCompletionEventArgs(view, result));
        }
    }
}
