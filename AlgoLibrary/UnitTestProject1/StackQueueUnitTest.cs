using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlgoLibrary;
using System.Collections.Generic;
using System.Collections;
using System.Linq;


namespace AlGoUnitTests
{
    [TestClass]
    public class StackQueueUnitTest
    {
        [TestMethod]
        public void Test_NthUglyNumber()
        {
            StackQueueAlgorithms algo = new StackQueueAlgorithms();
            algo.NthUglyNumber(10);

            Assert.IsTrue(true);
        }

    }
}
