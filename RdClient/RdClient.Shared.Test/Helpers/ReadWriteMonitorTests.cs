namespace RdClient.Shared.Test.Helpers
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Helpers;
    using System;
    using System.Threading;

    /// <summary>
    /// Tests for the ReadWriteMonotor helper class are needed only to bump up the overall code coverage.
    /// </summary>
    [TestClass]
    public class ReadWriteMonitorTests
    {
        [TestMethod]
        public void AqcuireAndReleaseReadLock_NothingBadHappens()
        {
            using( ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion) )
            {
                using(ReadWriteMonitor.Read(rwLock))
                {
                    // Do nothing
                }
            }
        }

        [TestMethod]
        public void AqcuireAndReleaseWriteLock_NothingBadHappens()
        {
            using (ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion))
            {
                using (ReadWriteMonitor.Write(rwLock))
                {
                    // Do nothing
                }
            }
        }

        [TestMethod]
        public void AqcuireAndReleaseUpgradeableReadeLock_NothingBadHappens()
        {
            using (ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion))
            {
                using (ReadWriteMonitor.UpgradeableRead(rwLock))
                {
                    // Do nothing
                }
            }
        }

        [TestMethod]
        public void AqcuireReadLockDisposeDispose_ExceptionThrown()
        {
            using (ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion))
            {
                try
                {
                    using (IDisposable d = ReadWriteMonitor.Read(rwLock))
                    {
                        d.Dispose();
                    }
                    Assert.Fail("Unexpected success");
                }
                catch(ObjectDisposedException)
                {
                    // Success!
                }
                catch
                {
                    Assert.Fail("Incorrect exception thrown");
                }
            }
        }
    }
}
