using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlgoLibrary;
using System.Collections.Generic;

namespace AlGoUnitTests
{
    [TestClass]
    public class SortingUnitTest
    {
        SortingAlgorithms _algo;
        int[] _intArray;

        [TestInitialize]
        public void Setup()
        {
            _algo = new SortingAlgorithms();
            _intArray = new int[] { 2, 89, 350, 1, 25, 44, 120, 89, 55, 500, 20};
        }


        [TestMethod]
        public void Test_InsertSort()
        {
            _algo.InsertSort(_intArray);
            Assert.AreEqual(1, _intArray[0]);
            Assert.IsTrue(true, "Test done!");
        }

        [TestMethod]
        public void Test_RadixSort()
        {
            _algo.RadixSort(_intArray, 4);



            Assert.IsTrue(true, "Test done!");
        }

        [TestMethod]
        public void Test_ScheduledEvents()
        {
            Interval[] events1 = new Interval[] {
                new Interval() { start = 1, end = 4  },
                new Interval() { start = 12, end = 15  },
                new Interval() { start = 5, end = 8  },
                new Interval() { start = 9, end = 10  },
                new Interval() { start = 2, end = 7  },
                new Interval() { start = 9, end = 11  },
            };

            SortingAlgorithms algo = new SortingAlgorithms();

            List < Interval > listEvents = new List<Interval>(events1);
            Assert.AreEqual(2, algo.MaxScheduledEvents(listEvents));

            listEvents.Add(
                new Interval() { start = 6, end = 11 }
                );

            Assert.AreEqual(3, algo.MaxScheduledEvents(listEvents));
        }

        [TestMethod]
        public void Test_AddInterval()
        {
            Interval[] empty = new Interval[] { };
            Interval[] result;
            SortingAlgorithms algo = new SortingAlgorithms();

            result = algo.AddInterval(empty,
                new Interval { start = 0, end = 2 }
                );
            Assert.AreEqual(1, result.Length);

            result = algo.AddInterval(result,
                new Interval { start = 3, end = 5 }
                );
            Assert.AreEqual(2, result.Length);

            result = algo.AddInterval(result,
                new Interval { start = 4, end = 6 }
                );
            Assert.AreEqual(2, result.Length);

            result = algo.AddInterval(result,
                new Interval { start = 1, end = 4 }
                );
            Assert.AreEqual(1, result.Length);

        }

    }
}
