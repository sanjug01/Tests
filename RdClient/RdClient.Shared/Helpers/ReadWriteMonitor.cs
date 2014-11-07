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

            protected abstract IDisposable Acquire();
            protected abstract void Release();

            protected LockBase(ReaderWriterLockSlim rwLock)
            {
                Contract.Requires(null != rwLock);
                Contract.Ensures(null != _rwLock);
                _rwLock = rwLock;
            }

            protected override void DisposeManagedState()
            {
                Release();
            }
        }

        private sealed class ReadLock : LockBase
        {
            public static IDisposable Create(ReaderWriterLockSlim rwLock)
            {
                Contract.Requires(null != rwLock);
                Contract.Ensures(null != Contract.Result<IDisposable>());
                return (new ReadLock(rwLock)).Acquire();
            }

            private ReadLock(ReaderWriterLockSlim rwLock) : base(rwLock) { }

            protected override IDisposable Acquire()
            {
                Contract.Ensures(null != Contract.Result<IDisposable>());
                _rwLock.EnterReadLock();
                return this;
            }

            protected override void Release()
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
                return (new UpgradeableReadLock(rwLock)).Acquire();
            }

            private UpgradeableReadLock(ReaderWriterLockSlim rwLock) : base(rwLock) { }

            protected override IDisposable Acquire()
            {
                Contract.Ensures(null != Contract.Result<IDisposable>());
                _rwLock.EnterUpgradeableReadLock();
                return this;
            }

            protected override void Release()
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
                return (new WriteLock(rwLock)).Acquire();
            }

            private WriteLock(ReaderWriterLockSlim rwLock) : base(rwLock) { }

            protected override IDisposable Acquire()
            {
                Contract.Ensures(null != Contract.Result<IDisposable>());
                _rwLock.EnterWriteLock();
                return this;
            }

            protected override void Release()
            {
                _rwLock.ExitWriteLock();
            }
        }
    }
}
