namespace RdClient.Shared.Test.Helpers
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Helpers;
    using System.Collections.Generic;

    [TestClass]
    public sealed class ExtensionsTests
    {
        private sealed class IntComparer : IComparer<int>
        {
            int IComparer<int>.Compare(int x, int y)
            {
                return x < y ? -1 : x > y ? 1 : 0;
            }
        }

        IComparer<int> _comparer;

        [TestInitialize]
        public void SetUpTest()
        {
            _comparer = new IntComparer();
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _comparer = null;
        }

        [TestMethod]
        public void IndexOfFirstGreaterOrEqual_InList_IndexOfEqual()
        {
            IList<int> list = new List<int>() { 1, 2, 3, 4, 5, 6 };

            Assert.AreEqual(3, list.IndexOfFirstGreaterOrEqual(4, _comparer));
            Assert.AreEqual(4, list.IndexOfFirstGreaterOrEqual(5, _comparer));
            Assert.AreEqual(0, list.IndexOfFirstGreaterOrEqual(1, _comparer));
            Assert.AreEqual(5, list.IndexOfFirstGreaterOrEqual(6, _comparer));
        }

        [TestMethod]
        public void IndexOfFirstGreaterOrEqual_GreaterThanList_Neg1()
        {
            IList<int> list = new List<int>() { 1, 2, 3, 4, 5, 6 };

            Assert.AreEqual(-1, list.IndexOfFirstGreaterOrEqual(10, _comparer));
        }

        [TestMethod]
        public void IndexOfFirstGreaterOrEqual_LessThanList_0()
        {
            IList<int> list = new List<int>() { 1, 2, 3, 4, 5, 6 };

            Assert.AreEqual(0, list.IndexOfFirstGreaterOrEqual(0, _comparer));
        }

        [TestMethod]
        public void IndexOfFirstGreaterOrEqual_NotInList_IndexOfGreater()
        {
            IList<int> list = new List<int>() { 1, 2, 4, 5, 6, 8, 10 };

            Assert.AreEqual(2, list.IndexOfFirstGreaterOrEqual(3, _comparer));
            Assert.AreEqual(5, list.IndexOfFirstGreaterOrEqual(7, _comparer));
            Assert.AreEqual(6, list.IndexOfFirstGreaterOrEqual(9, _comparer));
        }

        [TestMethod]
        public void IndexOfFirstGreaterOrEqual_SequenceOfDuplicates_FirstDuplicate()
        {
            IList<int> list = new List<int>() { 1, 1, 1, 1, 1, 1, 1, 2, 4, 5, 5, 5, 5, 6, 8, 10, 10, 10, 10 };

            Assert.AreEqual(0, list.IndexOfFirstGreaterOrEqual(1, _comparer));
            Assert.AreEqual(9, list.IndexOfFirstGreaterOrEqual(5, _comparer));
            Assert.AreEqual(15, list.IndexOfFirstGreaterOrEqual(10, _comparer));
            Assert.AreEqual(15, list.IndexOfFirstGreaterOrEqual(9, _comparer));
            Assert.AreEqual(0, list.IndexOfFirstGreaterOrEqual(-5, _comparer));
        }
    }
}
