namespace RdClient.Shared.Test.Input.Keyboard
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Input.Keyboard;
    using System.Collections.Generic;
    using Windows.UI.Core;

    [TestClass]
    public sealed class ScanCodeComparerTests
    {
        [TestMethod]
        public void ScanCodeComparer_CompareSameObject_0()
        {
            IComparer<CorePhysicalKeyStatus> scc = new ScanCodeComparer();
            CorePhysicalKeyStatus x = new CorePhysicalKeyStatus();

            x.ScanCode = 1;
            x.IsExtendedKey = false;

            Assert.AreEqual(0, scc.Compare(x, x));
        }

        [TestMethod]
        public void ScanCodeComparer_CompareSameDataNotExtended_0()
        {
            IComparer<CorePhysicalKeyStatus> scc = new ScanCodeComparer();
            CorePhysicalKeyStatus x = new CorePhysicalKeyStatus(), y = new CorePhysicalKeyStatus();

            x.ScanCode = 1;
            y.ScanCode = 1;
            x.IsExtendedKey = false;
            y.IsExtendedKey = false;

            Assert.AreEqual(0, scc.Compare(x, y));
        }

        [TestMethod]
        public void ScanCodeComparer_CompareSameDataExtended_0()
        {
            IComparer<CorePhysicalKeyStatus> scc = new ScanCodeComparer();
            CorePhysicalKeyStatus x = new CorePhysicalKeyStatus(), y = new CorePhysicalKeyStatus();

            x.ScanCode = 1;
            y.ScanCode = 1;
            x.IsExtendedKey = true;
            y.IsExtendedKey = true;

            Assert.AreEqual(0, scc.Compare(x, y));
        }

        [TestMethod]
        public void ScanCodeComparer_SameScanCodeExtNonExt_1()
        {
            IComparer<CorePhysicalKeyStatus> scc = new ScanCodeComparer();
            CorePhysicalKeyStatus x = new CorePhysicalKeyStatus(), y = new CorePhysicalKeyStatus();

            x.ScanCode = 1;
            y.ScanCode = 1;
            x.IsExtendedKey = true;
            y.IsExtendedKey = false;

            Assert.AreEqual(1, scc.Compare(x, y));
        }

        [TestMethod]
        public void ScanCodeComparer_SameScanCodeNonExtExt_Minus1()
        {
            IComparer<CorePhysicalKeyStatus> scc = new ScanCodeComparer();
            CorePhysicalKeyStatus x = new CorePhysicalKeyStatus(), y = new CorePhysicalKeyStatus();

            x.ScanCode = 1;
            y.ScanCode = 1;
            x.IsExtendedKey = false;
            y.IsExtendedKey = true;

            Assert.AreEqual(-1, scc.Compare(x, y));
        }

        [TestMethod]
        public void ScanCodeComparer_2_1_Minus1()
        {
            IComparer<CorePhysicalKeyStatus> scc = new ScanCodeComparer();
            CorePhysicalKeyStatus x = new CorePhysicalKeyStatus(), y = new CorePhysicalKeyStatus();

            x.ScanCode = 2;
            y.ScanCode = 1;
            x.IsExtendedKey = false;
            y.IsExtendedKey = false;

            Assert.AreEqual(1, scc.Compare(x, y));
        }

        [TestMethod]
        public void ScanCodeComparer_1_2_Minus1()
        {
            IComparer<CorePhysicalKeyStatus> scc = new ScanCodeComparer();
            CorePhysicalKeyStatus x = new CorePhysicalKeyStatus(), y = new CorePhysicalKeyStatus();

            x.ScanCode = 1;
            y.ScanCode = 2;
            x.IsExtendedKey = false;
            y.IsExtendedKey = false;

            Assert.AreEqual(-1, scc.Compare(x, y));
        }
    }
}
