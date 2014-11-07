namespace RdClient.Shared.Helpers
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;

    /// <summary>
    /// Factory of disposable read-write monitors designed to be used with the "using" C# statement.
    /// The class guarantees that acquisition of the monitor will be matched with its release.
    /// </summary>
    public static class ReadWriteMonitor
    {
        /// <summary>
        /// Acquire the lock object for reading and create a disposable object that will release the lock
        /// upon disposing.
        /// </summary>
        /// <param name="rwLock">Reader-writer lock to aquireand release.</param>
        /// <returns>Disposable object that releases the lock upon disposing</returns>
        public static IDisposable Read(ReaderWriterLockSlim rwLock)
        {
            Contract.Requires(null != rwLock);
            Contract.Ensures(null != Contract.Result<IDisposable>());
            return ReadLock.Create(rwLock);
        }

        /// <summary>
        /// Acquire the lock object for upgradeable reading and create a disposable object that will release the lock
        /// upon disposing.
        /// </summary>
        /// <param name="rwLock">Reader-writer lock to aquireand release.</param>
        /// <returns>Disposable object that releases the lock upon disposing</returns>
        public static IDisposable UpgradeableRead(ReaderWriterLockSlim rwLock)
        {
            Contract.Requires(null != rwLock);
            Contract.Ensures(null != Contract.Result<IDisposable>());
            return UpgradeableReadLock.Create(rwLock);
        }

        /// <summary>
        /// Acquire the lock object for writing and create a disposable object that will release the lock
        /// upon disposing.
        /// </summary>
        /// <param name="rwLock">Reader-writer lock to aquireand release.</param>
        /// <returns>Disposable object that releases the lock upon disposing</returns>
        public static IDisposable Write(ReaderWriterLockSlim rwLock)
        {
            Contract.Requires(null != rwLock);
            Contract.Ensures(null != Contract.Result<IDisposable>());
            return WriteLock.Create(rwLock);
        }

        private abstract class LockBase : DisposableObject
        {
            protected readonly ReaderWriterLockSlim _rwLock;

            protected LockBase(ReaderWriterLockSlim rwLock)
            {
                Contract.Requires(null != rwLock);
                Contract.Ensures(null != _rwLock);
                _rwLock = rwLock;
            }
        }

        private sealed class ReadLock : LockBase
        {
            public static IDisposable Create(ReaderWriterLockSlim rwLock)
            {
                Contract.Requires(null != rwLock);
                Contract.Ensures(null != Contract.Result<IDisposable>());
                return new ReadLock(rwLock);
            }

            private ReadLock(ReaderWriterLockSlim rwLock) : base(rwLock)
            {
                Contract.Requires(null != rwLock);
                rwLock.EnterReadLock();
            }

            protected override void DisposeManagedState()
            {
                _rwLock.ExitReadLock();
            }
        }

        private sealed class UpgradeableReadLock : LockBase
        {
            public static IDisposable Create(ReaderWriterLockSlim rwLock)
            {
                Contract.Requires(null != rwLock);
                Contract.Ensures(null != Contract.Result<IDisposable>());
                return new UpgradeableReadLock(rwLock);
            }

            private UpgradeableReadLock(ReaderWriterLockSlim rwLock) : base(rwLock)
            {
                Contract.Requires(null != rwLock);
                rwLock.EnterUpgradeableReadLock();
            }

            protected override void DisposeManagedState()
            {
                _rwLock.ExitUpgradeableReadLock();
            }
        }

        private sealed class WriteLock : LockBase
        {
            public static IDisposable Create(ReaderWriterLockSlim rwLock)
            {
                Contract.Requires(null != rwLock);
                Contract.Ensures(null != Contract.Result<IDisposable>());
                return new WriteLock(rwLock);
            }

            private WriteLock(ReaderWriterLockSlim rwLock) : base(rwLock)
            {
                Contract.Requires(null != rwLock);
                rwLock.EnterWriteLock();
            }

            protected override void DisposeManagedState()
            {
                _rwLock.ExitWriteLock();
            }
        }
    }
}
