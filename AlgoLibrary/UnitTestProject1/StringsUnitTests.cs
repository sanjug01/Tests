using System;
using System.Text;
using AlgoLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlGoUnitTests
{
    [TestClass]
    public class StringUnitTest
    {
        [TestMethod]
        public void Test_StringBuffer()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 20; i++)
                sb.Append(i);

            String result = sb.ToString();
            Assert.AreEqual(sb.Length, result.Length);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Test_MyAtoi()
        {
            StringsAlgorithms algo = new StringsAlgorithms();


            Assert.AreEqual(2, algo.MyAtoi("2"));
            Assert.AreEqual(-12, algo.MyAtoi("-12a"));
            Assert.AreEqual(2147483648, algo.MyAtoi("2147483648"));

            Assert.IsTrue(true);
        }
    }
}
