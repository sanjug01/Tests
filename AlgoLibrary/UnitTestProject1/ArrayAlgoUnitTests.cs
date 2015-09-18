using System;
using AlgoLibrary;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArrayAlgoUnitTests
{
    public class MyComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return x.CompareTo(y);
        }
    }

    [TestClass]
    public class ArrayAlgoUnitTests
    {
        ArraysAlgorithms _algo;

        [TestInitialize]
        public void TestSetup()
        {
            _algo = new ArraysAlgorithms();
        }

        [TestMethod]
        public void Test_PassOne()
        {
            _algo.PlusOne(new int[] { 1, 3 });

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Test_ZigZagConvert()
        {
            string result;

            result = _algo.ZigZagConvert("abcd", 4);


            result = _algo.ZigZagConvert("abcde", 2);

            result = _algo.ZigZagConvert("12345678901234567890", 4);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Test_MissingNumbers()
        {
            ArraysAlgorithms algo = new ArraysAlgorithms();
            Assert.AreEqual(1,
                algo.MissingNumber(new int[] { 0, 2, 3, 5, 4 })
            );

            Assert.AreEqual(1,
                algo.MissingNumber(new int[] { 0, 2 })
            );

            Assert.AreEqual(0,
                algo.MissingNumber(new int[] { 1, 2, 3, 5, 4 })
            );

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Test_FirstMissingNumbers()
        {
            ArraysAlgorithms algo = new ArraysAlgorithms();
            Assert.AreEqual(1,
                algo.FirstMissingPositive(new int[] { 0, 2, 3, 5, 4 })
            );

            Assert.AreEqual(2,
                algo.FirstMissingPositive(new int[] { 3, 4, -1, 1 })
            );

            Assert.AreEqual(3,
                algo.FirstMissingPositive(new int[] { 1, 2, 0 })
            );

            Assert.AreEqual(3,
                algo.FirstMissingPositive(new int[] { 2, 1 })
            );

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Test_FindRange()
        {
            ArraysAlgorithms algo = new ArraysAlgorithms();
            long[] testArray = { 1, 2, 3, 3, 3, 3, 3, 5, 5, 8, 9, 11 };

            int minR, maxR;

            algo.FindRange(testArray, 3, out minR, out maxR);

            algo.FindRange(testArray, 4, out minR, out maxR);


            algo.FindRange(testArray, 5, out minR, out maxR);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Test_MatrixRotation()
        {

            long[,] matrix = {
                {0,1,2,3},
                {10, 11, 12, 13 },
                {20,21,22,23 },
                {30,31,32,33 },
            };

            int size = matrix.Length;

            ArraysAlgorithms algo = new ArraysAlgorithms();
            algo.MatrixRotation(matrix, 4);


            Assert.IsTrue(true);
         }

        [TestMethod]
        public void Test_Iterator()
        {
            ArraysAlgorithms algo = new ArraysAlgorithms();

            int[] values = new int[] { 1, 1, 2, 4, 5 };

            Array.Sort(values);
            Array.Sort(values, values);


            int current = 0;
            IEnumerator<int> myEnumerator =  ArraysAlgorithms.RunLength(values);

            // myEnumerator.Reset();
            while (myEnumerator.MoveNext())
            {
                current = myEnumerator.Current;
            }

            MyCollection collection = new MyCollection(values);

            foreach (var idx in collection)
            {
                current = idx;
            }

            Assert.IsTrue(true);
        }
    }
}
