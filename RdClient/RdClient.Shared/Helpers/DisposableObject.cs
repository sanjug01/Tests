namespace RdClient.Shared.Helpers
{
    using System;
    using System.Threading;

    public abstract class DisposableObject : IDisposable
    {
        private int _disposed;

        protected DisposableObject()
        {
            _disposed = 0;
        }

        ~DisposableObject()
        {
            Dispose(false);
        }

        /// <summary>
        /// Overridable called to dispose disposable managed members and clear unnecesary references.
        /// The default implementation does nothing.
        /// </summary>
        protected virtual void DisposeManagedState()
        {
        }

        /// <summary>
        /// Overridable called to dispose native unmanaged state.
        /// </summary>
        protected virtual void DisposeNativeState()
        {
        }

        protected void ThrowIfDisposed()
        {
            if (0 != _disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (0 == Interlocked.CompareExchange(ref _disposed, 1, 0))
            {
                if (disposing)
                    DisposeManagedState();
                DisposeNativeState();
            }
            else
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}
