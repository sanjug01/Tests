using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlgoLibrary;
using System.Collections.Generic;
using System.Threading;

namespace AlGoUnitTests
{
    [TestClass]
    public class HashmapUnitTest
    {
        HashmapsAlgorithms _algo;

        [TestInitialize]
        public void Setup()
        {
            _algo = new HashmapsAlgorithms();
        }


        [TestMethod]
        public void Test_SquareNumbers()
        {
            Assert.AreEqual(2, _algo.NumSquares(101));

            // theorem indicates max 4
            Assert.IsTrue(_algo.NumSquares(2051) <= 4); 

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Test_HappyNumber()
        {
            Assert.IsTrue(_algo.IsHappy(19));

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Test_LongestContainedRange()
        {
            int[] testArray = { 3, -2, 7, 9, 8, 1, 2, 0, -1, 5, 8};

            int result = _algo.LongestContainedRange(testArray);
            Assert.AreEqual(6, result);

        }

        [TestMethod]
        public void Test_LongestDistinctSubarray()
        {
            int[] testArray = { 'f', 's', 'f', 'e', 't','w', 'e', 'n', 'w', 'e'};

            int result = _algo.LongestDistinctSubarray(testArray);
            Assert.AreEqual(5, result);

        }

        [TestMethod]
        public void Test_Collatz()
        {
            bool result = true; 
            result = _algo.TestCollatz(10);

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void Test_CollatzRangeSeq()
        {
            bool result = true;
            const int MaxN = 100;
            const int RangeSize = 10;

            // we reuse the verified set to avoid duplicating calculations
            HashSet<long> verified = new HashSet<long>();

            for(int i = 1; i < MaxN; i+=RangeSize)
            {
                result = result && _algo.TestCollatzRange(i, i+RangeSize-1, verified);
            }
            
            Assert.IsTrue(result);
        }

        
        [TestMethod]
        public void Test_CollatzRangeParallel()
        {
            const int MaxThreads = 3;
            const int MaxN = 100;
            const int RangeSize = 10;
            bool verifiedResult = true;

            // One event is used for each Worker
            ManualResetEvent[] doneEvents = new ManualResetEvent[MaxThreads];
            int[] range = new int[MaxThreads];
            int lastRange=0;
            HashSet<long> visited = new HashSet<long>();
            
            HashSet<long>[] partialVisited = new HashSet<long>[MaxThreads];

            // Configure and start threads using ThreadPool.
            Console.WriteLine("launching {0} tasks...", MaxThreads);
            for (int i = 0; i < MaxThreads; i++)
            {
                doneEvents[i] = new ManualResetEvent(false);
                range[i] = RangeSize * i + 1;
                partialVisited[i] = new HashSet<long>();

                ThreadPool.QueueUserWorkItem(
                    (o) => 
                    {
                        // calback for calculation - range[i],range[i] + RangeSize
                        int idx = (int)o;
                        verifiedResult = verifiedResult && 
                            _algo.TestCollatzRange(range[idx], range[idx] + RangeSize - 1, partialVisited[idx]);
                        doneEvents[idx].Set();
                    }, 
                    i);
            }
            lastRange = RangeSize * MaxThreads;

            while (lastRange < MaxN)
            {
                // wait any for more work
                int i = WaitHandle.WaitAny(doneEvents);
                range[i] = lastRange + 1;
                lastRange += RangeSize;
                doneEvents[i].Reset();

                // add to whole set and use t as a start point
                visited.UnionWith(partialVisited[i]);
                partialVisited[i] = visited;

                ThreadPool.QueueUserWorkItem(
                    (o) =>
                    {
                        // calback for calculation - range[i],range[i] + RangeSize
                        int idx = (int)o;
                        verifiedResult = verifiedResult &&
                             _algo.TestCollatzRange(range[idx], range[idx] + RangeSize - 1, partialVisited[idx]);
                        doneEvents[idx].Set();
                    },
                    i);
            }

            // Wait for all threads in pool to calculate.
            // WaitHandle.WaitAll(doneEvents); - not supported in STA
            Console.WriteLine("All calculations are complete.");
            int p1 = WaitHandle.WaitAny(doneEvents);
            int p2 = WaitHandle.WaitAny(doneEvents);
            int p3 = WaitHandle.WaitAny(doneEvents);

            //  results. 
            Assert.IsTrue(verifiedResult);
        }

        [TestMethod]
        public void Test_StringDecompositions()
        {
            const string sentence = "amanaplanacanal";
            string[] words = new string[] { "ana", "can", "apl"};

            HashmapsAlgorithms algorithms = new HashmapsAlgorithms();

            int[] result = algorithms.StringDecompositions(sentence, words);

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(4, result[0]);
        }

    }
}
