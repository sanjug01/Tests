namespace RdClient.Shared.Test.Helpers
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Helpers;
    using System;

    [TestClass]
    public sealed class MutableObjectTests
    {
        [TestMethod]
        public void MutableObject_BumpUpCodeCoverage()
        {
            int disposedManaged = 0, disposedNative = 0;

            using (TestMulableObject mo = new TestMulableObject(() => ++disposedManaged, () => ++disposedNative))
            {
                // Do nothing
                using (IDisposable d = mo.LR()) { Assert.IsNotNull(d); }
                using (IDisposable d = mo.LUR()) { Assert.IsNotNull(d); }
                using (IDisposable d = mo.LW()) { Assert.IsNotNull(d); }
            }
        }

        [TestMethod]
        public void UseMutableObjectInUsing_BumpsUpCodeCoverage()
        {
            int disposedManaged = 0, disposedNative = 0;

            using (TestMulableObject mo = new TestMulableObject(() => ++disposedManaged, () => ++disposedNative))
            {
                // Do nothing
            }

            Assert.AreEqual(1, disposedManaged);
            Assert.AreEqual(1, disposedNative);
        }

        [TestMethod]
        public void DisposeTwice_Throws()
        {
            int disposedManaged = 0, disposedNative = 0;

            IDisposable mo = new TestMulableObject(() => ++disposedManaged, () => ++disposedNative);

            mo.Dispose();
            Assert.AreEqual(1, disposedManaged);
            Assert.AreEqual(1, disposedNative);

            try
            {
                mo.Dispose();
                Assert.Fail("Unexpected success");
            }
            catch(ObjectDisposedException)
            {
                // Success
            }
            catch(Exception ex)
            {
                Assert.Fail(string.Format("Unexpected exception {0}", ex));
            }
        }

        [TestMethod]
        public void DisposeAndUse_Throws()
        {
            int disposedManaged = 0, disposedNative = 0;

            TestMulableObject mo = new TestMulableObject(() => ++disposedManaged, () => ++disposedNative);

            mo.Use(); // bump up code coverage
            mo.Dispose();
            Assert.AreEqual(1, disposedManaged);
            Assert.AreEqual(1, disposedNative);

            try
            {
                mo.Use();
                Assert.Fail("Unexpected success");
            }
            catch (ObjectDisposedException)
            {
                // Success
            }
            catch (Exception ex)
            {
                Assert.Fail(string.Format("Unexpected exception {0}", ex));
            }
        }

        private sealed class TestMulableObject : MutableObject
        {
            private readonly Action _disposedManaged, _disposedNative;

            public TestMulableObject(Action disposedManaged, Action disposedNative)
            {
                _disposedManaged = disposedManaged;
                _disposedNative = disposedNative;
            }

            public IDisposable LR() { return LockRead(); }
            public IDisposable LUR() { return LockUpgradeableRead(); }
            public IDisposable LW() { return LockWrite(); }

            public void Use()
            {
                ThrowIfDisposed();
            }

            protected override void DisposeManagedState()
            {
                base.DisposeManagedState();
                _disposedManaged();
            }

            protected override void DisposeNativeState()
            {
                base.DisposeNativeState();
                _disposedNative();
            }
        }
    }
}
