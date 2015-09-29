using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlgoLibrary;
using System.Collections.Generic;
using System.Collections;
using System.Linq;


namespace AlGoUnitTests
{
    public static class ArrayExtensions
    {
        public static void Swap<T>(this List<T> list, int i, int j)
        {
            T temp = list[j];
            list[j] = list[i];
            list[i] = temp;
        }
    }

    public class MyInverseComparer<T> : IComparer<T> where T:IComparable<T>
    {
        public int Compare(T x, T y)
        {
            return y.CompareTo(x);
        }
    }


    [TestClass]
    public class BaseUnitTest
    {
        [TestMethod]
        public void Test_BaseTypeExtensions()
        {
            string[] stringArray = { "one", "two", "three", "four" };

            List<string> myList = new List<string>();
            LinkedList<string> myLList = new LinkedList<string>();
            ArrayList myAList = new ArrayList(stringArray);

            myList.AddRange(stringArray);
            myLList.AddLast(stringArray[0]);
            myLList.AddLast(stringArray[1]);
            myLList.AddLast(stringArray[1]);

            myAList.Add(5);
            myAList.Add(6);

            System.Diagnostics.Debug.Write("List is: ");
            foreach (string s in myList)
            {
                System.Diagnostics.Debug.Write(s + "->");
            }
            System.Diagnostics.Debug.WriteLine(" | ");
            myList.Swap(1, 3);

            System.Diagnostics.Debug.Write("Swapped List is: ");
            foreach (string s in myList)
            {
                System.Diagnostics.Debug.Write(s + "->");
            }
            System.Diagnostics.Debug.WriteLine(" | ");

            System.Diagnostics.Debug.Write("Array List is: ");
            foreach (var s in myAList)
            {
                System.Diagnostics.Debug.Write(s + "->");
            }
            System.Diagnostics.Debug.WriteLine(" | ");


            System.Diagnostics.Debug.Write("Linked List is: ");
            foreach (var s in myLList)
            {
                System.Diagnostics.Debug.Write(s + "->");
            }
            System.Diagnostics.Debug.WriteLine(" | ");

        }


        [TestMethod]
        public void Test_BitArray()
        {

            Random rand = new Random();
            BitArray myBitArray = new BitArray(512, false);
            BitArray myBitArray2 = new BitArray(512, true);

            // Assert.AreNotEqual(myBitArray.Count, myBitArray2.Count);

            Assert.IsFalse(myBitArray[rand.Next(512)]);
            Assert.IsTrue(myBitArray2[rand.Next(256)]);

            // WARNING - they have to be the same size
            BitArray result = myBitArray.And(myBitArray2);
            result = myBitArray.Xor(myBitArray2);
            result = myBitArray2.Or(myBitArray);

            Assert.IsTrue(true);
        }


        [TestMethod]
        public void Test_BaseTypes()
        {
            string[] stringArray = {"one", "two", "three", "four"};
            int[,] moves = new int[,] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };


            Dictionary<int, string> myHashmap = new Dictionary<int, string>();
            SortedDictionary<string, string> sortedMap = new SortedDictionary<string, string>();

            LinkedList<string> linkedList = new LinkedList<string>();
            linkedList.AddFirst("zero");
            linkedList.AddLast("ten");
            string first = linkedList.First.Value;
            string second = linkedList.Last.Value;            
            
            List<string> myList = new List<string>();
            myList.AddRange(stringArray);

            HashSet<string> hashSet = new HashSet<string>();
            Queue<string> queue = new Queue<string>();
            Stack<string> stack = new Stack<string>();
            SortedSet<string> sortedSet = new SortedSet<string>();
            SortedList<long, string> sortedList = new SortedList<long, string>();

            int i = 0;
            foreach( string s in stringArray)
            {
                i++;
                myHashmap[i] = s;
                sortedMap[s] = s + "_mapped";
                linkedList.AddAfter(linkedList.First, s);
                hashSet.Add(s);

                queue.Enqueue(s);
                stack.Push(s);
                sortedSet.Add(s);
                sortedList.Add(i, s);
            }
            
            Assert.AreEqual(stringArray.Length, myHashmap.Count);
            Assert.AreEqual(stringArray.Length, sortedMap.Count);
            Assert.AreEqual(stringArray.Length + 2, linkedList.Count);
            Assert.AreEqual(stringArray.Length, hashSet.Count);
            Assert.AreEqual(stringArray.Length, queue.Count);
            Assert.AreEqual(stringArray.Length, stack.Count);
            Assert.AreEqual(stringArray.Length, sortedSet.Count);
            Assert.AreEqual(stringArray.Length, sortedList.Count);

            // these can be used as arrays
            string[] array = myList.ToArray();
            array = stack.ToArray();
            array = queue.ToArray();


            char[] charArray = stringArray[0].ToCharArray();
            string fromChar = new string(charArray, 1, charArray.Length - 1);
            string fromSubstring = stringArray[0].Substring(1, charArray.Length - 1);

            Assert.AreEqual(fromChar, fromSubstring);

            foreach(var pair in sortedMap)
            {
                // cannot remove while iterating
                // sortedMap.Remove(pair.Key);
            }
            foreach(string s in linkedList)
            {
                //linkedList.Remove(s);
                hashSet.Remove(s);
                sortedSet.Remove(s);
            }
        }

        [TestMethod]
        public void Test_BitCounts()
        {
            UInt16 chNumber = 4; // 100b
            Assert.AreEqual(4, BaseClass.LeastBitOnly(chNumber));
            Assert.AreEqual(0, BaseClass.ResetLeastBit(chNumber));
            Assert.AreEqual((uint)1, BaseClass.BitCount(chNumber));
            Assert.AreEqual((uint)1, BaseClass.BitCount2(chNumber));

            chNumber = 6; //  110b
            Assert.AreEqual(2, BaseClass.LeastBitOnly(chNumber));
            Assert.AreEqual(4, BaseClass.ResetLeastBit(chNumber));
            Assert.AreEqual((uint)2, BaseClass.BitCount(chNumber));
            Assert.AreEqual((uint)2, BaseClass.BitCount2(chNumber));

            chNumber = 7; //  111b
            Assert.AreEqual(1, BaseClass.LeastBitOnly(chNumber));
            Assert.AreEqual(6, BaseClass.ResetLeastBit(chNumber));
            Assert.AreEqual((uint)3, BaseClass.BitCount(chNumber));
            Assert.AreEqual((uint)3, BaseClass.BitCount2(chNumber));

            chNumber = 4 + 16 + 64; // 1010100
            Assert.AreEqual(4, BaseClass.LeastBitOnly(chNumber));
            Assert.AreEqual(80, BaseClass.ResetLeastBit(chNumber));
            Assert.AreEqual((uint)3, BaseClass.BitCount(chNumber));
            Assert.AreEqual((uint)3, BaseClass.BitCount2(chNumber));

            int n = chNumber;
            n = n ^ 4;


        }



        [TestMethod]
        public void Test_NumericLimits()
        {
            Assert.IsTrue(1000 < Double.MaxValue);
            Assert.IsTrue((double)int.MaxValue < Double.MaxValue);
            Assert.IsTrue((double)float.MaxValue < double.MaxValue);


            Assert.IsTrue(-1000 > Double.MinValue);
            Assert.IsTrue((double)int.MinValue > Double.MinValue);
            Assert.IsTrue((double)float.MinValue > double.MinValue);
        }


        [TestMethod]
        public void Test_Arrays()
        {
            int[,] moves2 = new int[2, 4];
            int[][] moves3 = {
                new int[3],
                new int[4],
            };


            Assert.IsTrue(moves2.Length == 8);
            Assert.IsTrue(moves3.Length == 2);
            Assert.IsTrue(moves3[0].Length == 3);
            Assert.IsTrue(moves3[1].Length == 4);   
        }

        [TestMethod]
        public void Test_ArraysSort()
        {
            int[] myArray= { 1, 2, int.MaxValue, int.MinValue, -1, -100, 100, 200 };

            Array.Sort(myArray);
            Assert.AreEqual(int.MinValue, myArray[0]);
  
            // binary search requires sorted array
            int idx100 = Array.BinarySearch(myArray, 100);

            Array.Sort(myArray, new MyInverseComparer<int>());
            idx100 = Array.BinarySearch(myArray, 100, new MyInverseComparer<int>());

            Assert.AreEqual(int.MaxValue, myArray[0]);

            // reversing array
            Array.Reverse(myArray);
            Array.Reverse(myArray, 1, myArray.Length - 2);


            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Test_SortedList()
        {
            SortedList<double, int> mySortedList = new SortedList<double, int>();

            // WARNING: sorted list does not alow duplicated keys
            // double[] costs = { Double.MinValue, 100, 0, 23, 56, Double.MaxValue, 55, 100, 0, 55 };
            double[] costs = { Double.MinValue, 100, 0, 23, 56, Double.MaxValue, 55, };

            for (int i=0;i < costs.Length; i++)
            {
                mySortedList.Add(costs[i], i);
            }

            foreach( var c in mySortedList)
            {
                System.Diagnostics.Debug.Write(c.Value + "(" + c.Key + ") -> ");
            }

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Test_SortedSet()
        {
            // WARNING: sorted set require Comparable objects
            SortedSet<CostObject<int>> mySortedSet = 
                new SortedSet<CostObject<int>>();

            double[] costs = { Double.MinValue, 100, 0, 23, 56, Double.MaxValue, 55, 100, 0, 55 };

            for (int i = 0; i < costs.Length; i++)
            {
                mySortedSet.Add(new CostObject<int>(i, costs[i]));
            }
            
            var min = mySortedSet.Min;
            var max = mySortedSet.Max;            
        }

        private HashSet<ISet<long>> powerSet(ISet<long> aSet)
        {
            HashSet < ISet<long>> pSet = new HashSet<ISet<long>>();


            if (aSet.Count == 0)
            {
                pSet.Add(aSet);
            }
            else
            {
                var elem = aSet.First();
                // foreach (var elem in aSet)
            
                HashSet<long> reducedSet = new HashSet<long>(aSet);
                reducedSet.Remove(elem);

                var pSubSet = powerSet(reducedSet);
                pSet.UnionWith(pSubSet);

                foreach(var setElem in pSubSet)
                {
                    var extraElem = new HashSet<long>(setElem);
                    extraElem.Add(elem);
                    pSet.Add(extraElem);
                }
            }
            
            return pSet;
        }

        [TestMethod]
        public void Test_PowerSet()
        {
            HashSet<long> aSet = new HashSet<long>( new long[] { 0, 1, 2 } );
            
            HashSet<ISet<long>> pSet = powerSet(aSet);
            
            Assert.AreEqual(8, pSet.Count);
        }


        //[TestMethod]
        //public void Test_Threads()
        //{
        //    System.Threading.Thread t1;
        //}

    }
}
