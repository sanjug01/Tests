using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.Helpers;

namespace RdClient.Shared.Test.Helpers
{
    [TestClass]

    public class ArrayRoundTests
    {
        [TestMethod]
        public void ArrayRoundTests_EmptyArray()
        {
            ArrayRound ar = new ArrayRound(new int[] { });

            int result = ar.Round(3);

            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void ArrayRoundTests_HappyCase()
        {
            ArrayRound ar = new ArrayRound(new int[] {5, 10, 15 });

            int result = ar.Round(12);

            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void ArrayRoundTests_Last()
        {
            ArrayRound ar = new ArrayRound(new int[] { 5, 10, 15 });

            int result = ar.Round(20);

            Assert.AreEqual(15, result);
        }

        [TestMethod]
        public void ArrayRoundTests_Middle()
        {
            ArrayRound ar = new ArrayRound(new int[] { 5, 11 });

            int result = ar.Round(8);

            Assert.AreEqual(5, result);
        }
    }
}
